#pragma once

class CActivePowerScheme;

/// <SUMMARY>
/// 当前激活的电源计划
/// </SUMMARY>
class LActivePowerScheme
{
public:
    /// <SUMMARY>
    /// 构造函数
    /// </SUMMARY>
    LActivePowerScheme();
    /// <SUMMARY>
    /// 析构函数
    /// </SUMMARY>
    ~LActivePowerScheme();

    /// <SUMMARY>
    /// 获取交流电下盖子关闭时的动作值
    /// </SUMMARY>
    /// <RETURN>
    /// 0 Do Nothing, 1 Sleep, 2 Hibernate, 3 Shut Down
    /// </RETURN>
    unsigned long GetLIDCloseACAction();

    /// <SUMMARY>
    /// 获取直流电下盖子关闭时的动作值
    /// </SUMMARY>
    /// <RETURN>
    /// 0 Do Nothing, 1 Sleep, 2 Hibernate, 3 Shut Down
    /// </RETURN>
    unsigned long GetLIDCloseDCAction();

    /// <SUMMARY>
    /// 设置交流电下盖子关闭时的动作值
    /// </SUMMARY>
    /// <PARAM name="action" dir="IN">
    /// 0 Do Nothing, 1 Sleep, 2 Hibernate, 3 Shut Down
    /// </PARAM>
    void SetLIDCloseACAction(unsigned long action);

    /// <SUMMARY>
    /// 设置直流电下盖子关闭时的动作值
    /// </SUMMARY>
    /// <PARAM name="action" dir="IN">
    /// 0 Do Nothing, 1 Sleep, 2 Hibernate, 3 Shut Down
    /// </PARAM>
    void SetLIDCloseDCAction(unsigned long action);

    /// <SUMMARY>
    /// 设置电源计划
    /// </SUMMARY>
    /// <PARAM name="action" dir="IN">
    /// 0 Maximum Power Savings, 1 Typical Power Savings, 2 No Power Savings
    /// </PARAM>
    void SetPowerScheme(unsigned long level);

    /// <SUMMARY>
    /// 获取电源计划
    /// </SUMMARY>
    /// <RETURN>
    /// 0 Maximum Power Savings, 1 Typical Power Savings, 2 No Power Savings
    /// </RETURN>
    unsigned long GetPowerScheme();

    /// <SUMMARY>
    /// 获取交流电下关闭屏幕的超时时间
    /// </SUMMARY>
    /// <RETURN>
    /// 时间按秒单位
    /// </RETURN>
    unsigned long GetScreenCloseACTimeout();

    /// <SUMMARY>
    /// 获取直流电下关闭屏幕的超时时间
    /// </SUMMARY>
    /// <RETURN>
    /// 时间按秒单位
    /// </RETURN>
    unsigned long GetScreenCloseDCTimeout();

    /// <SUMMARY>
    /// 设置交流电下关闭屏幕的超时时间
    /// </SUMMARY>
    /// <PARAM name="sec" dir="IN">
    /// 时间按秒单位
    /// </PARAM>
    void SetScreenCloseACTimeout(unsigned long sec);

    /// <SUMMARY>
    /// 设置直流电下关闭屏幕的超时时间
    /// </SUMMARY>
    /// <PARAM name="sec" dir="IN">
    /// 时间按秒单位
    /// </PARAM>
    void SetScreenCloseDCTimeout(unsigned long sec);

    /// <SUMMARY>
    /// 获取交流电下电脑睡眠的超时时间
    /// </SUMMARY>
    /// <RETURN>
    /// 时间按秒单位
    /// </RETURN>
    unsigned long GetSleepACTimeout();

    /// <SUMMARY>
    /// 获取直流电下电脑睡眠的超时时间
    /// </SUMMARY>
    /// <RETURN>
    /// 时间按秒单位
    /// </RETURN>
    unsigned long GetSleepDCTimeout();

    /// <SUMMARY>
    /// 设置交流电下电脑睡眠的超时时间
    /// </SUMMARY>
    /// <PARAM name="sec" dir="IN">
    /// 时间按秒单位
    /// </PARAM>
    void SetSleepACTimeout(unsigned long sec);

    /// <SUMMARY>
    /// 设置直流电下电脑睡眠的超时时间
    /// </SUMMARY>
    /// <PARAM name="sec" dir="IN">
    /// 时间按秒单位
    /// </PARAM>
    void SetSleepDCTimeout(unsigned long sec);

private:
    CActivePowerScheme* m_pPowerScheme;         // 电源计划实现对象
};