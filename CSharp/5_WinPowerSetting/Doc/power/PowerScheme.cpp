
#include "PowerScheme.h"

#include <Windows.h>
#include <powrprof.h>

#pragma comment(lib, "powrprof.lib")

class CActivePowerScheme
{
public:
    CActivePowerScheme()
    {
        m_pPowerScheme = nullptr;
        PowerGetActiveScheme(NULL, &m_pPowerScheme); 
    }

    ~CActivePowerScheme()
    {
        if (m_pPowerScheme != nullptr)
        {
            LocalFree(m_pPowerScheme);
            m_pPowerScheme = nullptr;
        }
    }

    unsigned long GetLIDCloseACAction()
    {
        DWORD acAction;
        PowerReadACValueIndex(NULL, m_pPowerScheme, &GUID_SYSTEM_BUTTON_SUBGROUP, &GUID_LIDCLOSE_ACTION, &acAction);
        return acAction;
    }

    unsigned long GetLIDCloseDCAction()
    {
        DWORD acAction;
        PowerReadDCValueIndex(NULL, m_pPowerScheme, &GUID_SYSTEM_BUTTON_SUBGROUP, &GUID_LIDCLOSE_ACTION, &acAction);
        return acAction;
    }

    void SetLIDCloseACAction(unsigned long action)
    {
        PowerWriteACValueIndex(NULL, m_pPowerScheme, &GUID_SYSTEM_BUTTON_SUBGROUP, &GUID_LIDCLOSE_ACTION, action);
        PowerSetActiveScheme(NULL, m_pPowerScheme);
    }

    void SetLIDCloseDCAction(unsigned long action)
    {
        PowerWriteDCValueIndex(NULL, m_pPowerScheme, &GUID_SYSTEM_BUTTON_SUBGROUP, &GUID_LIDCLOSE_ACTION, action);
        PowerSetActiveScheme(NULL, m_pPowerScheme);
    }

    void SetPowerScheme(unsigned long level)
    {
        if (level == 0)
        {
            PowerSetActiveScheme(NULL, &GUID_MAX_POWER_SAVINGS);
        }
        else if (level == 1)
        {
            PowerSetActiveScheme(NULL, &GUID_TYPICAL_POWER_SAVINGS);
        }
        else if (level == 2)
        {
            PowerSetActiveScheme(NULL, &GUID_MIN_POWER_SAVINGS);
        }
    }

    unsigned long GetPowerScheme()
    {
        if (GUID_MAX_POWER_SAVINGS == *m_pPowerScheme)
        {
            return 0;
        }
        else if (GUID_TYPICAL_POWER_SAVINGS == *m_pPowerScheme)
        {
            return 1;
        }
        else if (GUID_MIN_POWER_SAVINGS == *m_pPowerScheme)
        {
            return 2;
        }

        return 1;
    }

    unsigned long GetScreenCloseACTimeout()
    {
        DWORD acSeconds;
        PowerReadACValueIndex(NULL, m_pPowerScheme, &GUID_VIDEO_SUBGROUP, &GUID_VIDEO_POWERDOWN_TIMEOUT, &acSeconds);
        return acSeconds;
    }

    unsigned long  GetScreenCloseDCTimeout()
    {
        DWORD acSeconds;
        PowerReadDCValueIndex(NULL, m_pPowerScheme, &GUID_VIDEO_SUBGROUP, &GUID_VIDEO_POWERDOWN_TIMEOUT, &acSeconds);
        return acSeconds;
    }

    void SetScreenCloseACTimeout(unsigned long sec)
    {
        PowerWriteACValueIndex(NULL, m_pPowerScheme, &GUID_VIDEO_SUBGROUP, &GUID_VIDEO_POWERDOWN_TIMEOUT, sec);
        PowerSetActiveScheme(NULL, m_pPowerScheme);
    }

    void SetScreenCloseDCTimeout(unsigned long sec)
    {
        PowerWriteDCValueIndex(NULL, m_pPowerScheme, &GUID_VIDEO_SUBGROUP, &GUID_VIDEO_POWERDOWN_TIMEOUT, sec);
        PowerSetActiveScheme(NULL, m_pPowerScheme);
    }

    unsigned long GetSleepACTimeout()
    {
        DWORD acSeconds;
        PowerReadACValueIndex(NULL, m_pPowerScheme, &GUID_SLEEP_SUBGROUP, &GUID_STANDBY_TIMEOUT, &acSeconds);
        return acSeconds;
    }

    unsigned long  GetSleepDCTimeout()
    {
        DWORD acSeconds;
        PowerReadDCValueIndex(NULL, m_pPowerScheme, &GUID_SLEEP_SUBGROUP, &GUID_STANDBY_TIMEOUT, &acSeconds);
        return acSeconds;
    }

    void SetSleepACTimeout(unsigned long sec)
    {
        PowerWriteACValueIndex(NULL, m_pPowerScheme, &GUID_SLEEP_SUBGROUP, &GUID_STANDBY_TIMEOUT, sec);
        PowerSetActiveScheme(NULL, m_pPowerScheme);
    }

    void SetSleepDCTimeout(unsigned long sec)
    {
        PowerWriteDCValueIndex(NULL, m_pPowerScheme, &GUID_SLEEP_SUBGROUP, &GUID_STANDBY_TIMEOUT, sec);
        PowerSetActiveScheme(NULL, m_pPowerScheme);
    }
    
private:
    GUID* m_pPowerScheme;
};


LActivePowerScheme::LActivePowerScheme()
{
    m_pPowerScheme = new CActivePowerScheme();
}

LActivePowerScheme::~LActivePowerScheme()
{
    if (m_pPowerScheme != nullptr)
    {
        delete m_pPowerScheme;
        m_pPowerScheme = nullptr;
    }
}

unsigned long LActivePowerScheme::GetLIDCloseACAction()
{
    return m_pPowerScheme->GetLIDCloseACAction();
}

unsigned long LActivePowerScheme::GetLIDCloseDCAction()
{
    return m_pPowerScheme->GetLIDCloseDCAction();
}

void LActivePowerScheme::SetLIDCloseACAction(unsigned long action)
{
    return m_pPowerScheme->SetLIDCloseACAction(action);
}

void LActivePowerScheme::SetLIDCloseDCAction(unsigned long action)
{
    return m_pPowerScheme->SetLIDCloseDCAction(action);
}

void LActivePowerScheme::SetPowerScheme(unsigned long level)
{
    return m_pPowerScheme->SetPowerScheme(level);
}

unsigned long LActivePowerScheme::GetPowerScheme()
{
    return m_pPowerScheme->GetPowerScheme();
}

unsigned long LActivePowerScheme::GetScreenCloseACTimeout()
{
    return m_pPowerScheme->GetScreenCloseACTimeout();
}

unsigned long LActivePowerScheme::GetScreenCloseDCTimeout()
{
    return m_pPowerScheme->GetScreenCloseDCTimeout();
}

void LActivePowerScheme::SetScreenCloseACTimeout(unsigned long sec)
{
    return m_pPowerScheme->SetScreenCloseACTimeout(sec);
}

void LActivePowerScheme::SetScreenCloseDCTimeout(unsigned long sec)
{
    return m_pPowerScheme->SetScreenCloseDCTimeout(sec);
}

unsigned long LActivePowerScheme::GetSleepACTimeout()
{
    return m_pPowerScheme->GetSleepACTimeout();
}

unsigned long LActivePowerScheme::GetSleepDCTimeout()
{
    return m_pPowerScheme->GetSleepDCTimeout();
}

void LActivePowerScheme::SetSleepACTimeout(unsigned long sec)
{
    return m_pPowerScheme->SetSleepACTimeout(sec);
}

void LActivePowerScheme::SetSleepDCTimeout(unsigned long sec)
{
    return m_pPowerScheme->SetSleepDCTimeout(sec);
}

