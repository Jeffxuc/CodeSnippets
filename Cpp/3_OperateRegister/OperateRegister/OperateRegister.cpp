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
/// 向 Register的指定 Key 中写入 REG_DWORD 类型数值，该值包含
/// </summary>
/// <returns></returns>
void SetRegValue()
{
    HKEY hkey;

    // 1. Open the registry subKey (64位的软件注册表位于64位的部分，所以需要使用到 KEY_WOW64_64KEY； 权限中包括写入修改，因此需要管理员权限)
    LONG openRes = RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\ASUS\\ASUS System Control Interface\\Tmp"), 0, KEY_ALL_ACCESS | KEY_WOW64_64KEY, &hkey);


    if (openRes == ERROR_SUCCESS)
    {
        printf("Open Reg key Successful.\n");

    }
    else
    {
        printf("Open Reg key Failed.\n");
    }

    // 2. 向 subKey 中写入数值，如果 valueName 不存在，则会创建对应的valueName ，并写入 valueData，最后一个参数的单位是字节数 bytes
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

    // 3. 关闭 Register Key的句柄.
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
    // [注：最后一个参数为字节数，包含末尾的空字符，单位是Byte，而不是字符个数。如果是 wstring，其byte要乘以2，因为一个宽字符占2个字节]
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
/// 删除注册表的 SubKey
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

