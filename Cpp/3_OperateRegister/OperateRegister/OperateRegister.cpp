#include <iostream>
#include "OperateRegister.h"


int main()
{
    //SetRegValue();

    //CreateRegKey();

    DeleteRegKey();



    std::cin.get();
}


/// <summary>
/// �� Register��ָ�� Key ��д�� REG_DWORD ������ֵ����ֵ����
/// </summary>
/// <returns></returns>
void SetRegValue()
{
    HKEY hkey;

    // 1. Open the registry subKey (64λ�����ע���λ��64λ�Ĳ��֣�������Ҫʹ�õ� KEY_WOW64_64KEY�� Ȩ���а���д���޸ģ������Ҫ����ԱȨ��)
    LONG openRes = RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\ASUS\\ASUS System Control Interface\\Tmp"), 0, KEY_ALL_ACCESS | KEY_WOW64_64KEY, &hkey);


    if (openRes == ERROR_SUCCESS)
    {
        printf("Open Reg key Successful.\n");

    }
    else
    {
        printf("Open Reg key Failed.\n");
    }

    // 2. �� subKey ��д����ֵ����� valueName �����ڣ���ᴴ����Ӧ��valueName ����д�� valueData�����һ�������ĵ�λ���ֽ��� bytes
    LPCTSTR valueName = TEXT("CityPort");
    DWORD valueData = 0x99;
    LONG setRes = RegSetValueEx(hkey, valueName, 0, REG_DWORD, (LPBYTE)&valueData, sizeof(DWORD));

    if (setRes == ERROR_SUCCESS)
    {
        printf("Set value to key Sucessful\n");
    }
    else
    {
        printf("Set value to key Failed\n");
    }

    // 3. �ر� Register Key�ľ��.
    LONG closeRes = RegCloseKey(hkey);

    if (closeRes == ERROR_SUCCESS) 
    {
        printf("Close Reg key Successful\n");
    }
    else
    {
        printf("Close Reg key Failed\n");
    }

}


/// <summary>
/// Create Key for Register and set value - data.
/// </summary>
void CreateRegKey() 
{
    HKEY hKey;
    DWORD dwDisposition;

    // 1. Create Register SubKey if existed open it.
    LONG createRes = RegCreateKeyEx(HKEY_LOCAL_MACHINE,
                                    TEXT("SOFTWARE\\ASUS\\ASUS System Control Interface\\AsusDiagnosis"),
                                    0,
                                    NULL,
                                    0,
                                    KEY_ALL_ACCESS|KEY_WOW64_64KEY,
                                    NULL,
                                    &hKey,
                                    &dwDisposition);

    if (createRes == ERROR_SUCCESS)
    {
        printf("Create Reg Key Successful.\n");
    }
    else
    {
        printf("Create Reg Key Failed.\n");
    }

    // 2. Set value for it. 
    // [ע�����һ������Ϊ�ֽ���������ĩβ�Ŀ��ַ�����λ��Byte���������ַ������������ wstring����byteҪ����2����Ϊһ�����ַ�ռ2���ֽ�]
    LPCWSTR valueName = L"TestVal";
    LPCTSTR valueData = TEXT("YouMakeIt");
    LONG setRes = RegSetValueEx(hKey, valueName, 0, REG_SZ, (LPBYTE)valueData, 2*(wcslen(valueData) + 1));
    if (setRes == ERROR_SUCCESS) 
    {
        printf("Set Value Successful.\n");
    }
    else
    {
        printf("Set Value Failed.\n");
    }

    // 3.Close the Key Handle.
    if (RegCloseKey(hKey) == ERROR_SUCCESS)
    {
        printf("Close Reg key Successful\n");
    }
    else
    {
        printf("Close Reg key Failed\n");
    }


}


/// <summary>
/// ɾ��ע���� SubKey
/// </summary>
void DeleteRegKey()
{
    LONG deleteRes = RegDeleteKeyEx(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\ASUS\\ASUS System Control Interface\\TmpCreate01"), KEY_ALL_ACCESS | KEY_WOW64_64KEY, 0);
    if (deleteRes == ERROR_SUCCESS) 
    {
        printf("Delete Reg Key Successful.\n");
    }
    else
    {
        printf("Delete Reg Key Failed.\n");
    }

}


void GetRegValue()
{


}

