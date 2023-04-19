#include <iostream>
#include <algorithm>

#include "./SetupApi/LSetupAPI.h"

int main()
{

    LSetupBluetooth bluetooth;
    LSetupNet netDevices;

    wstring btdate;
    wstring wlandate;
    wstring btDriverVersion;
    wstring wlanDriverVersion;

    for (int i = 0; i < bluetooth.GetDevNum(); i++)
    {
        wstring instanceID;
        bluetooth.GetInstanceID(i, instanceID);
        std::transform(instanceID.begin(), instanceID.end(), instanceID.begin(), toupper);

        if (wcsncmp(instanceID.c_str(), L"PCI\\", 4) == 0 ||
            wcsncmp(instanceID.c_str(), L"USB\\", 4) == 0)
        {
            wstring desc;
            //bluetooth.GetDevDesc(i, desc);
            //bluetooth.GetDriverVersion(i, btDriverVersion);
            //bluetooth.GetDriverDate(i, btdate);

            bluetooth.GetDriverData_01(i, btdate);
            bluetooth.GetDriverVersion_01(i, btDriverVersion);
            
            break;
        }
    }

    for (int i = 0; i < netDevices.GetDevNum(); i++)
    {
        wstring instanceID;
        netDevices.GetInstanceID(i, instanceID);
        std::transform(instanceID.begin(), instanceID.end(), instanceID.begin(), toupper);

        if (wcsncmp(instanceID.c_str(), L"PCI\\", 4) == 0 ||
            wcsncmp(instanceID.c_str(), L"USB\\", 4) == 0)
        {
            wstring devDesc;
            wstring devName;
            netDevices.GetDevDesc(i, devDesc);
            netDevices.GetFriendlyName(i, devName);
            std::transform(devDesc.begin(), devDesc.end(), devDesc.begin(), toupper);
            std::transform(devName.begin(), devName.end(), devName.begin(), toupper);

            if (devDesc.find(L"WIRELESS") != std::wstring::npos ||
                devName.find(L"WIRELESS") != std::wstring::npos ||
                devDesc.find(L"WIFI") != std::wstring::npos ||
                devName.find(L"WIFI") != std::wstring::npos ||
                devDesc.find(L"WI-FI") != std::wstring::npos ||
                devName.find(L"WI-FI") != std::wstring::npos ||
                devDesc.find(L"WI FI") != std::wstring::npos ||
                devName.find(L"WI FI") != std::wstring::npos)
            {
                netDevices.GetDriverData_01(i, wlandate);
                netDevices.GetDriverVersion_01(i, wlanDriverVersion);
                break;
            }
        }

    }

    
}

