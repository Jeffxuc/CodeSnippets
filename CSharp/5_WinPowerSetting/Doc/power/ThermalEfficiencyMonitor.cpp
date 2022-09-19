#include "ThermalEfficiencyMonitor.h"

#include <time.h>

#include <QtGui/QPainter>
#include <QtCore/QTextStream>
#include <QtCore/QTime>
#include <QtCore/QSettings>

#include "DialogWarning.h"
#include "DialogPCType.h"
#include "DialogRepairerInput.h"
#include "BoardUploadReport.h"

#include ".\\Share\\GlobalData.h"
#include ".\\Log\\FileLog.h"

#include "..\\ShareModule\\Conf\\DiagCfgParam.h"
#include "..\\ShareModule\\ECController.h"
#include "..\\ShareModule\\IntelCPUPower.h"
#include "..\\ShareModule\\Api\\LHardwareInfor.h"
#include "..\\ShareModule\\Api\\Cpu\\LCPUID.h"
#include "..\\ShareModule\\Api\\SystemInfo\\SystemInfo.h"
#include "..\\ShareModule\\Api\\CPUUsage.h"
#include "..\\ShareModule\\Api\\PowerMgmt\\\PowerScheme.h"
#include "..\\ShareModule\\Asus\\ASUSHealthyTable.h"
#include "..\\ShareModule\\ThirdParty\\ThirdParty.h"

#define THERMAL_EFFICIENCY_MONITOR_ENABLE               "MonitorEnable/TestEnable"
#define THERMAL_EFFICIENCY_MONITOR_RESULT               "MonitorResult/TestResult"

#define EVENT_TEST_THERMAL_EFFICIENCY_MONITOR_AUTO_START    QEvent::Type(QEvent::User+4001)
#define EVENT_TEST_THERMAL_EFFICIENCY_MONITOR_END           QEvent::Type(QEvent::User+4002)

DialogThermalEfficiencyMonitor::DialogThermalEfficiencyMonitor(QWidget* pParent, bool runAllMode)
    : QDialog(pParent)
{
    LogWriteLineW(L"[ThermalEfficiencyMonitor] Start Thermal Efficiency Monitor Tool");

    this->m_mousePressed = false;
    this->m_runAllMode = runAllMode;

    if (!CheckIfNeedThermalEfficiencyMonitor())
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] No need to thermal efficiency monitor");

        LogWriteLineW(L"[ThermalEfficiencyMonitor] End Thermal Efficiency Monitor Tool");
        
        this->SetTestEnableForIni(false);
        exit(0);
    }
    else
    {
        this->SetTestEnableForIni(true);
    }

    bool getValue = true;
    this->m_tdp = 0;
    this->m_recommendPower = 0;
    this->m_energyUnits = 0;

    if ((!IntelCPUPower::GetPkgTDP(this->m_tdp) || (this->m_tdp == 0))
        || (this->m_recommendPower = this->GetRecommendPowerSpec(this->m_tdp)) == 0
        || (!IntelCPUPower::GetEnergyUnits(this->m_energyUnits) || (this->m_energyUnits == 0)))
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Get value failed, Get TDP is %d, recommend power is %d, energy units is %d", this->m_tdp, this->m_recommendPower, this->m_energyUnits);

        DialogWarning warning(
            nullptr,
            QObject::tr("Warning"),
            QString(QObject::tr("<font color=red>Thermal Efficiency Monitor not support.</font>")));
        warning.exec();

        if (this->m_tdp == 0)
        {
            this->m_result = CPU_THERMAL_NO_SUPPORT;
            this->m_noSupportReason = "NOSUPPORT,TDP 0W";
        }
        else if (this->m_recommendPower == 0)
        {
            this->m_result = CPU_THERMAL_NO_SUPPORT;
            this->m_noSupportReason = "NOSUPPORT,Spec Power 0W";
        }
        else if (this->m_energyUnits == 0)
        {
            this->m_result = CPU_THERMAL_NO_SUPPORT;
            this->m_noSupportReason = "NOSUPPORT,Energy Units: 0";
        }

        LogWriteLineW(L"[ThermalEfficiencyMonitor] End Thermal Efficiency Monitor Tool");
        this->SetTestResultForIni(this->m_result);
        exit(0);
    }

    LogWriteLineW(L"[ThermalEfficiencyMonitor] Get TDP is %d, recommend power is %d, energy units is %d", this->m_tdp, this->m_recommendPower, this->m_energyUnits);

    this->setupUi(this);

    // 隐藏默认窗口边框和标题栏
    this->setWindowFlags(Qt::Window | Qt::FramelessWindowHint | Qt::WindowSystemMenuHint
        | Qt::WindowMinimizeButtonHint | Qt::WindowMaximizeButtonHint);

    // 获取当前系统DPI, 当前系统DPI除以设计时DPI值, 则得到UI放大系数
    const float DESIGN_DPI = 96.0f; // 设计时DPI
    QPainter painter(this);
    QPaintDevice* pDevice = painter.device();
    float ratioX = pDevice->logicalDpiX() / DESIGN_DPI;
    float ratioY = pDevice->logicalDpiY() / DESIGN_DPI;
    float uiRatio = ratioX > ratioY ? ratioX : ratioY;
    if (uiRatio < 1.0f)
        uiRatio = 1.0f;
    // 根据比例重新调整主UI大小
    int width = this->geometry().width() * uiRatio;
    int height = this->geometry().height() * uiRatio;
    this->setFixedSize(width, height);

    this->m_pPowerScheme = new LActivePowerScheme();
    this->m_powerSchemeValue = this->m_pPowerScheme->GetPowerScheme();
    this->m_ScreenCloseACTimeout = this->m_pPowerScheme->GetScreenCloseACTimeout();
    this->m_ScreenCloseDCTimeout = this->m_pPowerScheme->GetScreenCloseDCTimeout();
    this->m_SleepACTimeout = this->m_pPowerScheme->GetSleepACTimeout();
    this->m_SleepDCTimeout = this->m_pPowerScheme->GetSleepDCTimeout();

    this->m_hyperFanSwitchMode = 0;
    this->m_checkCount = 0;
    this->m_checkSeq = 0;

    if (!ThirdPart::GetHyperFanMode(this->m_hyperFanSwitchMode))
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Get hyper fan mode failed");
    }
    else
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Get hyper fan mode is %d", this->m_hyperFanSwitchMode);
    }

    this->m_pThermalEfficiencyMonitorStartTimer = new QTimer();
    this->m_pThermalEfficiencyMonitorStartTimer->setInterval(10000);
    this->m_pThermalEfficiencyMonitorStartTimer->stop();

    this->m_pThermalEfficiencyMonitorTimer = new QTimer();
    this->m_pThermalEfficiencyMonitorTimer->setInterval(1000);
    this->m_pThermalEfficiencyMonitorTimer->stop();

    this->m_pUploadReportBoard = new BoardUploadReport(this);

    this->Initialize(uiRatio, this->m_archInfo);

    connect(this->pushButtonStart, SIGNAL(clicked()), this, SLOT(StartMonitorButtonClicked()));
    connect(this->pushButtonStop, SIGNAL(clicked()), this, SLOT(StopMonitorButtonClicked()));
    connect(this->pushButtonClose, SIGNAL(clicked()), this, SLOT(CloseButtonClicked()));
    connect(m_pThermalEfficiencyMonitorStartTimer, SIGNAL(timeout()), this, SLOT(TimerThermalEfficiencyMonitorStartSlot()));
    connect(m_pThermalEfficiencyMonitorTimer, SIGNAL(timeout()), this, SLOT(TimerThermalEfficiencyMonitorSlot()));

    // Run all不需要点击按钮, 自动化运行
    if (this->m_runAllMode)
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Auto thermal efficiency monitor");
        QApplication::postEvent(this, new QEvent(EVENT_TEST_THERMAL_EFFICIENCY_MONITOR_AUTO_START));
    }
}

DialogThermalEfficiencyMonitor::~DialogThermalEfficiencyMonitor()
{
    if (m_pThermalEfficiencyMonitorTimer != nullptr)
    {
        delete m_pThermalEfficiencyMonitorTimer;
        m_pThermalEfficiencyMonitorTimer = nullptr;
    }

    if (m_pThermalEfficiencyMonitorStartTimer != nullptr)
    {
        delete m_pThermalEfficiencyMonitorStartTimer;
        m_pThermalEfficiencyMonitorStartTimer = nullptr;
    }

    if (m_pUploadReportBoard != nullptr)
    {
        delete m_pUploadReportBoard;
        m_pUploadReportBoard = nullptr;
    }
}

void DialogThermalEfficiencyMonitor::Initialize(float uiRatio, QString archInfo)
{
    // CPU信息
    const ProcessorInfor& processorInfo = LHardwareInfor::GetProcessorInfor();
    QString cpuType = QString::fromStdWString(processorInfo.Name);
    this->labelCpuType->setText(cpuType);
    QString cpuSpeed = QString("%1 MHz").arg(processorInfo.MaxClockSpeed);
    this->labelCpuSpeed->setText(cpuSpeed);

    this->labelCpuArchitecture->setText(archInfo);

    // 获取计算机类型
    const SMBIOSInfor& infor = LHardwareInfor::GetSMBIOSInfor();
    PC_TYPE pcType = (PC_TYPE)CfgParam::GetPCType(infor.BoardProductName.c_str(), infor.SysModelName.c_str());
    if (pcType == PC_UNKNOWN)
    {
        // 配置文件中未记录该机种的类型, 则弹出选择对话框
        DialogPCType dialog(nullptr, uiRatio);
        pcType = (PC_TYPE)dialog.exec();
    }
    GlobalData::PCTypeSet(pcType);

    // 初始化EC控制器
    ECController::ECControllerPrepare(LogWriteLineW, (int)pcType);

    // 获取CPU温度
    unsigned long temp = ECController::GetCPUTemp();
    QString strCpuTemp = QString(" %1 C").arg((unsigned long)temp);

    this->labelCpuTemp->setText(strCpuTemp);

    // 输入的测试时长
    if (this->m_runAllMode)
    {
        this->TimelineEdit->setText(QString("%1").arg(1));
        this->TimelineEdit->setEnabled(false);
        // Run all不需要点击按钮, 自动化运行, 隐藏按钮
        this->pushButtonStart->setVisible(false);
        this->pushButtonStop->setVisible(false);
    }
    else
    {
        this->pushButtonStart->setEnabled(true);
        this->TimelineEdit->setEnabled(true);
        this->TimelineEdit->setMaxLength(2);
        QIntValidator *validator = new QIntValidator(this->TimelineEdit);
        validator->setRange(1,20);
        this->TimelineEdit->setValidator(validator);
        this->TimelineEdit->setText(QString("%1").arg(1));
    }

    this->labelSpecPower->setText(QString(" %1 W").arg(this->m_recommendPower));
}

void DialogThermalEfficiencyMonitor::ReInitialize()
{
    // 还原电源计划模式
    this->m_pPowerScheme->SetPowerScheme(this->m_powerSchemeValue);

    // 还原关闭屏幕时间
    this->m_pPowerScheme->SetScreenCloseACTimeout(this->m_ScreenCloseACTimeout);
    this->m_pPowerScheme->SetScreenCloseDCTimeout(this->m_ScreenCloseDCTimeout);

    // 还原电脑睡眠时间
    this->m_pPowerScheme->SetSleepACTimeout(this->m_SleepACTimeout);
    this->m_pPowerScheme->SetSleepDCTimeout(this->m_SleepDCTimeout);

    // 还原风扇模式
    if (this->m_hyperFanSwitchMode != 0)
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Start hyper fan switch to default");

        ThirdPart::StartHyperFanSwitch(this->m_hyperFanSwitchMode);
    }

    // 调整CPU FAN自动模式
    ECController::SetCPUFanManual(false);

    // 关闭计时器
    this->m_pThermalEfficiencyMonitorStartTimer->stop();
    this->m_pThermalEfficiencyMonitorTimer->stop();
    killTimer(this->m_thermalEfficiencyMonitorTimeoutTimer);

    // CPU释放
    SetCPUUsage(this->m_coreNo, -1);

    this->pushButtonStart->setEnabled(true);
}

bool DialogThermalEfficiencyMonitor::CheckIfNeedThermalEfficiencyMonitor()
{
    unsigned char healthyTableVer = 0;
    bool ret = true;

    // 是否不支援该model
    QString modelName;
    QString valueStr;
    SMBIOSInfor biosInfo = LHardwareInfor::GetSMBIOSInfor();
    QString sysProductName = biosInfo.SysModelName.c_str();
    sysProductName = sysProductName.trimmed();
    QString boardProductName = biosInfo.BoardProductName.c_str();
    boardProductName = boardProductName.trimmed();
    if (!boardProductName.isEmpty())
        modelName = boardProductName;
    else
        modelName = sysProductName;

    // 保存Model Name
    m_modelName = modelName;

    QString series = this->GetNoNeedSeriesFromConfFile();

    if (!series.isEmpty())
    {
        QStringList seriesList = series.split(",");
        for (int i = 0; i < seriesList.size(); i++)
        {
            if (modelName.contains(seriesList[i], Qt::CaseInsensitive))
            {
                if (!this->m_runAllMode)
                {
                    DialogWarning warning(nullptr, QObject::tr("Warning"), QObject::tr("No need for this model."));
                    warning.exec();
                }

                LogWriteLineW(L"[ThermalEfficiencyMonitor] No need for this model");

                return false;
            }
        }
    }

    // 是否为Intel CPU
    LCPUID cpuID;
    string cpuVendor;
    cpuID.GetVendor(cpuVendor);
    if (cpuVendor.compare("GenuineIntel") != 0)
    {
        if (!this->m_runAllMode)
        {
            DialogWarning warning(
                nullptr,
                QObject::tr("Warning"),
                QString(QObject::tr("<font color=red> Not Intel CPU.</font>")));
            warning.exec();
        }

        LogWriteLineW(L"[ThermalEfficiencyMonitor] Not Intel CPU");

        return false;
    }

    LCPUIDProInfo cpuidProInfo;
    cpuID.GetProcessorInfo(cpuidProInfo);
    string cpuBrand;
    cpuID.GetBrand(cpuBrand);

    QString archName = CfgParam::GetIntelCPUCodeName(cpuBrand.c_str());

    if (archName.isEmpty())
    {
        archName = CfgParam::GetIntelCPUArchitectureName(
            cpuidProInfo.Model,
            cpuidProInfo.Stepping,
            cpuBrand.c_str());
    }
    QString archInfo = "";
    if (archName.isEmpty())
        archInfo = "Unknown";
    else
        archInfo = archName;

    this->m_archInfo = archInfo;

    // 是否支援Healthy table
    if (ASUSHealthyTable::GetHealthyTableVer(healthyTableVer))
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Get healthy table Ver is %d", healthyTableVer);

        if (healthyTableVer >= 2)
        {
            ;
        }
        else
        {
            ret = false;
        }
    }
    else
    {
        ret = false;
    }

    if (!ret)
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Not support healthy table");

        if (archName == "IceLake" || archName == "CoffeeLake" ||
            archName == "KabyLake" || archName == "ApolloLake" ||
            archName == "GeminiLake" || archName == "WhiskeyLake" || 
            archName == "CometLake")
        {
            return true;
        }
        else
        {
            if (!this->m_runAllMode)
            {
                DialogWarning warning(
                    nullptr,
                    QObject::tr("Warning"),
                    QString(QObject::tr("<font color=red>The EC and CPU platform of this model not support thermal efficiency monitor.</font>")));
                warning.exec();
            }

            LogWriteLineW(L"[ThermalEfficiencyMonitor] Not support CPU platform");

            return false;
        }
    }

    return true;
}

ulong DialogThermalEfficiencyMonitor::GetRecommendPowerSpec(const ulong tdp)
{
    ulong specPower = 0;

    if (this->GetRecommendPowerSpecFromSpecialModelsFile(tdp, specPower))
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Get recommand power from SpecialModelsSpec.csv");
    }
    else
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Get recommand power from ThermalEfficiencyMonitor.ini");
        specPower = this->GetRecommendPowerSpecFromConfFile(tdp);
    }

    return specPower;
}

ulong DialogThermalEfficiencyMonitor::GetRecommendPowerSpecFromConfFile(const ulong tdp)
{
    QString valueStr = "0";
    CfgParam::ThermalEfficiencyMonitorConfGet(QString("RecommandPowerSpec/%1").arg(tdp), valueStr);
    ulong specPower = valueStr.toInt();
 
    return specPower;
}

bool DialogThermalEfficiencyMonitor::GetRecommendPowerSpecFromSpecialModelsFile(const ulong tdp, ulong &specPower)
{
    QString modelNameU = m_modelName.toUpper();
    bool isExist = false;

    QFile file(".\\conf\\thermaltool\\SpecialModelsSpec.csv");
    if (!file.exists())
        return false;

    if (!file.open(QIODevice::ReadOnly | QIODevice::Text))
        return false;

    QTextStream in(&file);
    QString line = in.readLine();
    while (!line.isNull())
    {
        QStringList items = line.split(",");
        if (items.size() != 3)
        {
            line = in.readLine();
            continue;
        }

        QString item0 = items[0].trimmed();
        QString item1 = items[1].trimmed();
        QString item2 = items[2].trimmed();

        item0 = item0.toUpper();
        // 判定model名是否为该系列
        if (modelNameU.contains(item0))
        {
            isExist = true;

            if (item1.isEmpty() || item2.isEmpty())
            {
                break;
            }

            if (tdp != item1.toUInt())
            {
                LogWriteLineW(L"The TDP not set right");
            }

            specPower = item2.toUInt();

            break;
        }

        line = in.readLine();
    }

    file.close();

    return isExist;
}

QString DialogThermalEfficiencyMonitor::GetNoNeedSeriesFromConfFile()
{
    QString valueStr = "0";
    CfgParam::ThermalEfficiencyMonitorConfGet(QString("NoNeedSeries/Series"), valueStr);

    return valueStr;
}

QString DialogThermalEfficiencyMonitor::TimeSecondConvert(const ulong seconds)
{
    int hour = (seconds / 3600) % 60;
    int minute = (seconds / 60) % 60;
    int second = seconds % 60;

    QString strTime = QString("%1:%2:%3")
        .arg(hour, 2, 10, QChar('0'))
        .arg(minute, 2, 10, QChar('0'))
        .arg(second, 2, 10, QChar('0'));

    return strTime;
}

bool DialogThermalEfficiencyMonitor::ShowRepairInput()
{
    DialogRepairerInput diaglog(nullptr);
    int iRet = diaglog.exec();
    if (iRet == REPAIRER_ABORT)
    {
        LogWriteLineW(L"Repairer Input Dialog Is Aborted");
        return false;
    }

    RepairerInput inputDate = GlobalData::RepairerInputGet();

    LogWriteLineW(L"Repairer Input SN: %s", inputDate.SN.toStdWString().c_str());
    LogWriteLineW(L"Repairer Input ID: %s", inputDate.ID.toStdWString().c_str());

    // 单板测试下将SN写入BIOS中
    if (GlobalData::MachineStateGet() == MACHINE_MB)
    {
        bool bRet = ThirdPart::UseWBTWriteISN(inputDate.SN.toStdString());
        if (bRet)
            LogWriteLineW(L"MB Test Write ISN SUCCESS");
        else
            LogWriteLineW(L"MB Test Write ISN FAIL");

        if (LHardwareInfor::GetSMBIOSInfor().BoardProductName.find("T103HAF"))
        {
            DialogWarning warning(
                nullptr,
                QObject::tr("Warning"),
                QString(QObject::tr("<font color=red>Please check whether the current machine is LTE SKU or not. "
                    "\nIf so, please update ICCID after finish repair.</font>")));
            warning.exec();
        }

    }

    return true;
}

void DialogThermalEfficiencyMonitor::ShowUploadReportBoard(int testResult)
{
    if (testResult == CPU_THERMAL_PASS)
    {
        GlobalData::UploadInfoSet("test_result", "PASS");
    }
    else if(testResult == CPU_THERMAL_FAIL)
    {
        GlobalData::UploadInfoSet("test_result", "FAIL");
    }
    else if (testResult == CPU_THERMAL_NO_SUPPORT)
    {
        GlobalData::UploadInfoSet("test_result", "NOSUPPORT");
    }

    QString testType = "Thermal Efficiency Monitor";
    GlobalData::UploadInfoSet("test_type", testType);

    QString time = QDate::currentDate().toString("yyyy-M-d") + 
        QTime::currentTime().toString(" hh:mm:ss");
    GlobalData::UploadInfoSet("test_time", time);

    QString agingTime = "0:0";
    GlobalData::UploadInfoSet("aging_time", agingTime);

    // Thermal monitor trail run test
    //QString wtpVersion = GlobalData::GetWTPExeVersion();
    QString wtpVersion = "1.1.T.0";
    GlobalData::UploadInfoSet("version", wtpVersion);

    QString passItem = QString("Spec Power: %1 W, Package Power: %2").arg(this->m_recommendPower).arg(this->labelPkgPower->text());
    GlobalData::UploadInfoSet("pass_item", passItem);

    // 显示上传报告页面
    m_pUploadReportBoard->Init();
}

void DialogThermalEfficiencyMonitor::SetTestResultForIni(int testResult)
{
    if (testResult == CPU_THERMAL_FAIL)
    {
        CfgParam::ThermalEfficiencyMonitorConfSet(THERMAL_EFFICIENCY_MONITOR_RESULT, "FAIL");
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Set monitor test result FAIL");
    }
    else if(testResult == CPU_THERMAL_PASS)
    {
        QString passItem = QString("PASS,Spec Power: %1 W, Package Power: %2").arg(this->m_recommendPower).arg(this->labelPkgPower->text());
        CfgParam::ThermalEfficiencyMonitorConfSet(THERMAL_EFFICIENCY_MONITOR_RESULT, passItem);
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Set monitor test result %s", passItem.toStdWString().c_str());
    }
    else if (testResult == CPU_THERMAL_NO_SUPPORT)
    {
        CfgParam::ThermalEfficiencyMonitorConfSet(THERMAL_EFFICIENCY_MONITOR_RESULT, this->m_noSupportReason);
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Set monitor test result %s", this->m_noSupportReason.toStdWString().c_str());
    }
}

void DialogThermalEfficiencyMonitor::SetTestEnableForIni(bool testEnable)
{
    if (testEnable)
    {
        CfgParam::ThermalEfficiencyMonitorConfSet(THERMAL_EFFICIENCY_MONITOR_ENABLE, "true");
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Set monitor test enable true");
    }
    else
    {
        CfgParam::ThermalEfficiencyMonitorConfSet(THERMAL_EFFICIENCY_MONITOR_ENABLE, "false");
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Set monitor test enable false");
    }
}

void DialogThermalEfficiencyMonitor::SetCpuFanMaxSpeed()
{
    unsigned long range = ECController::GetCPUFanRange();

    if (range > 0)
    {
        unsigned long maxValue = 0;
        // The NB Wistron's fan speed from 2400 to 4400 
        if (range == 4400)
        {
            maxValue = 4400;
        }
        else
        {
            maxValue = range;
        }

        ECController::SetCPUFanManual(true);

        LogWriteLineW(L"[ThermalEfficiencyMonitor] Set CPU fan speed max value %d", maxValue);
        if (!ECController::SetCPUFanSpeed(maxValue))
        {
            LogWriteLineW(L"[ThermalEfficiencyMonitor] SetCPUFanSpeed FAIL");
            ECController::SetCPUFanManual(false);
        }
        Win32Sleep(1000);
    }
}

void DialogThermalEfficiencyMonitor::mousePressEvent(QMouseEvent *e)
{
    m_mousePressed = true;
    m_mouseLastPos = e->globalPos();
}

void DialogThermalEfficiencyMonitor::mouseMoveEvent(QMouseEvent *e)
{
    if (!m_mousePressed)
        return;

    int dx = e->globalX() - m_mouseLastPos.x();
    int dy = e->globalY() - m_mouseLastPos.y();
    m_mouseLastPos = e->globalPos();
    move(this->x() + dx, this->y() + dy);
}

void DialogThermalEfficiencyMonitor::mouseReleaseEvent(QMouseEvent *e)
{
    m_mousePressed = false;
}

void DialogThermalEfficiencyMonitor::keyPressEvent(QKeyEvent* e)
{
    // 屏蔽按下ESC关闭对话框的动作
    switch (e->key())
    {
    case Qt::Key_Escape:
        break;
    default:
        QDialog::keyPressEvent(e);
    }
}

void DialogThermalEfficiencyMonitor::StartMonitorButtonClicked()
{
    // 界面清空动态信息
    this->labelPkgPower->setText("");
    this->labelTestTime->setText("");
    this->labelRemainingTime->setText("");

    this->m_miniutes = this->TimelineEdit->text().toInt();

    // 是否有插入电源适配器
    bool isACOnline = true;
    SysPowerStatus powerStatus = GetSysPowerStatus();

    if (!powerStatus.ACOnLine)
    {
        DialogWarning warning(
            nullptr,
            QObject::tr("Warning"),
            QString(QObject::tr("<font color=red>Current AC not online, please plug in AC adapter.</font>")));
        warning.exec();

        return;
    }

    if (!this->m_runAllMode)
    {
        if (!this->ShowRepairInput())
            return;
    }

    // 电源计划设置为高性能模式
    this->m_pPowerScheme->SetPowerScheme(2);

    // 切换风扇模式为turbo模式
    LogWriteLineW(L"[ThermalEfficiencyMonitor] Start hyper fan switch to turbo");
    ThirdPart::StartHyperFanSwitch(4);

    // 调整CPU FAN转速最大值
    SetCpuFanMaxSpeed();

    // 关闭屏幕睡眠
    this->m_pPowerScheme->SetScreenCloseACTimeout(0);
    this->m_pPowerScheme->SetScreenCloseDCTimeout(0);

    // 关闭电脑睡眠
    this->m_pPowerScheme->SetSleepACTimeout(0);
    this->m_pPowerScheme->SetSleepDCTimeout(0);

    int coreNum = GetCpuCoreNum();
    LogWriteLineW(L"[ThermalEfficiencyMonitor] CPU Logical Core Num: %d", coreNum);

    unsigned int coreNo = 0;
    for (int i = 0; i < coreNum; i++)
    {
        int mask = 1;
        mask = 1 << i;
        coreNo = coreNo | mask;
    }

    SetCPUUsage(coreNo, 100);
    this->m_coreNo = coreNo;
    this->m_result = CPU_THERMAL_PASS;
    this->m_noSupportReason = "";
    this->m_pThermalEfficiencyMonitorStartTimer->start();

    this->pushButtonStart->setEnabled(false);
}

void DialogThermalEfficiencyMonitor::StopMonitorButtonClicked()
{
    this->ReInitialize();

    this->pushButtonStart->setEnabled(true);
}

void DialogThermalEfficiencyMonitor::CloseButtonClicked()
{
    this->ReInitialize();

    this->done(0);
}

void DialogThermalEfficiencyMonitor::TimerThermalEfficiencyMonitorStartSlot()
{
    this->m_pThermalEfficiencyMonitorStartTimer->stop();
    LogWriteLineW(L"[ThermalEfficiencyMonitor] Thermal efficiency monitor start");

    if (!IntelCPUPower::GetEnergyValue(this->m_energyLastValue))
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Get last energy value error");
        this->m_result = CPU_THERMAL_NO_SUPPORT;
        this->m_noSupportReason = "NOSUPPORT,Energy Value: 0";
        QApplication::postEvent(this, new QEvent(EVENT_TEST_THERMAL_EFFICIENCY_MONITOR_END));
        return;
    }
    else
    {
        this->m_timeLastValue = clock();
        this->m_timeStartValue = this->m_timeLastValue;
    }

    this->labelRemainingTime->setText(this->TimeSecondConvert(this->m_miniutes * 60));
    this->labelTestTime->setText("0");
    this->m_pThermalEfficiencyMonitorTimer->start();
    this->m_thermalEfficiencyMonitorTimeoutTimer = startTimer(this->m_miniutes * 60 * 1000);

}

void DialogThermalEfficiencyMonitor::TimerThermalEfficiencyMonitorSlot()
{
    bool checkRet = true;
    if (!IntelCPUPower::GetEnergyValue(this->m_energyCurValue))
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Get current energy value error");
        this->ReInitialize();
        this->m_result = CPU_THERMAL_NO_SUPPORT;
        this->m_noSupportReason = "NOSUPPORT,Energy Value: 0";
        checkRet = false;
    }
    else
    {
        this->m_timeCurValue = clock();
        double timeDvalue = this->m_timeCurValue - this->m_timeLastValue;
        double energyDvalue = this->m_energyCurValue - this->m_energyLastValue;
        ulong totalTimeDvalue = this->m_timeCurValue - this->m_timeStartValue;

        // Package Power = △(Energy Value / 2^ Energy Units) per second
        double cmpPower = energyDvalue / pow(2, this->m_energyUnits) / timeDvalue * 1000;
        this->labelPkgPower->setText(QString(" %1 W").arg((ulong)cmpPower));

        LogWriteLineW(L"[ThermalEfficiencyMonitor] package power %u",
            (ulong)cmpPower);

        // 显示时长
        this->labelRemainingTime->setText(this->TimeSecondConvert(this->m_miniutes * 60 - totalTimeDvalue / 1000));
        this->labelTestTime->setText(this->TimeSecondConvert(totalTimeDvalue / 1000));

        // 获取CPU温度
        unsigned long temp = ECController::GetCPUTemp();
        QString strCpuTemp = QString(" %1 C").arg((unsigned long)temp);

        this->labelCpuTemp->setText(strCpuTemp);
        LogWriteLineW(L"[ThermalEfficiencyMonitor] cpu temp %u",
            (ulong)temp);

        if (cmpPower < this->m_recommendPower)
        {
            LogWriteLineW(L"[ThermalEfficiencyMonitor] package power less than recommand power");

            LogWriteLineW(L"[ThermalEfficiencyMonitor] recommand power %u",
                this->m_recommendPower);

            this->m_checkCount++;
            this->m_checkSeq++;

            if (this->m_checkCount == 1)
            {
                this->m_timeFirstFailed = this->m_timeCurValue;
            }
        }
        else
        {
            this->m_checkSeq = 0;
        }

        if (this->m_checkCount == 5 && this->m_checkSeq == 5)
        {
            ulong failedTimeDvalue = (this->m_timeCurValue - this->m_timeFirstFailed) / 1000;
            LogWriteLineW(L"[ThermalEfficiencyMonitor] package power less than recommand power five times during time %u seconds",
                (ulong)failedTimeDvalue);

            this->ReInitialize();

            QString text = "";
            if (this->m_runAllMode)
            {
                text = QObject::tr("<font color=red>1.Please make sure the current bios version is latest version or not."
                    "\n2.Please check whether the thermal grease is applied correctly."
                    "\nIf the above is invalid, replace the CPU thermal module."
                    "\nRun all test must thermal efficiency monitor test passed.</font>");
            }
            else
            {
                text = QObject::tr("<font color=red>1.Please make sure the current bios version is latest version or not."
                    "\n2.Please check whether the thermal grease is applied correctly."
                    "\nIf the above is invalid, replace the CPU thermal module.</font>");
            }

            DialogWarning warning(
                nullptr,
                QObject::tr("Warning"),
                text);
            warning.exec();

            this->m_result = CPU_THERMAL_FAIL;
            checkRet = false;

            this->m_checkCount = 0;
            this->m_checkSeq = 0;
        }
        else if (this->m_checkCount >= 5 && this->m_checkSeq < 5)
        {
            this->m_checkCount = 0;
            this->m_checkSeq = 0;
        }
    }
    if (!checkRet)
    {
        QApplication::postEvent(this, new QEvent(EVENT_TEST_THERMAL_EFFICIENCY_MONITOR_END));

        return;
    }

    this->m_energyLastValue = this->m_energyCurValue;
    this->m_timeLastValue = this->m_timeCurValue;
}

void DialogThermalEfficiencyMonitor::timerEvent(QTimerEvent * e)
{
    if (e->timerId() == this->m_thermalEfficiencyMonitorTimeoutTimer)
    {
        LogWriteLineW(L"[ThermalEfficiencyMonitor] Thermal efficiency monitor timeout after %d miniutes", this->m_miniutes);
        this->ReInitialize();
        QApplication::postEvent(this, new QEvent(EVENT_TEST_THERMAL_EFFICIENCY_MONITOR_END));
    }
}

void DialogThermalEfficiencyMonitor::customEvent(QEvent *event)
{
    switch (event->type())
    {
    case EVENT_TEST_THERMAL_EFFICIENCY_MONITOR_AUTO_START:
        this->StartMonitorButtonClicked();
        break;
    case EVENT_TEST_THERMAL_EFFICIENCY_MONITOR_END:
        if (this->m_result == CPU_THERMAL_PASS)
        {
            DialogWarning warning(
                nullptr,
                QObject::tr("Warning"),
                QString(QObject::tr("Thermal Efficiency Monitor succeed.")));
            warning.exec();
        }
        if (this->m_result == CPU_THERMAL_NO_SUPPORT)
        {
            DialogWarning warning(
                nullptr,
                QObject::tr("Warning"),
                QString(QObject::tr("<font color=red>Thermal Efficiency Monitor not support.</font>")));
            warning.exec();
        }
        else if(this->m_result == CPU_THERMAL_FAIL)
        {
            DialogWarning warning(
                nullptr,
                QObject::tr("Warning"),
                QString(QObject::tr("<font color=red>Thermal Efficiency Monitor failed.</font>")));
            warning.exec();
        }
        if (!this->m_runAllMode)
        {
            this->ShowUploadReportBoard(this->m_result);
        }
        else
        {
            this->SetTestResultForIni(this->m_result);
            // Run all模式下测试结束, 界面自动退出
            exit(0);
        }
        break;
    default:
        break;
    }
}