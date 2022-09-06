#include "CharStringConvert.h"

int main()
{
    ProcessCharacter* convertChar = new ProcessCharacter();

    // 1. 字符串转宽字符串
    string str1 = "Image";
    wstring dest1;
    convertChar->StringToWString(str1, dest1);

    // 2. 宽字符串转字符串
    wstring wstr2 = L"测试Image";
    string retStr2;
    convertChar->WStringToString(wstr2,retStr2);

    // 3. 字符数组转宽字符数组
    const char* pChar3 = "hello,Test";
    wchar_t* pWchar3 = convertChar->CharToWchar(pChar3);
    delete[] pWchar3; // 需要释放转换过程中 new 的数组空间，否则会出现内存泄漏。
    pWchar3 = nullptr;

    // 4. 宽字符数组转字符数组
    const wchar_t* pWchar4 = L"Hello,Test";
    char* pChar4 = convertChar->WcharToChar(pWchar4);
    delete[] pChar4; // 需要释放转换过程中 new 的数组空间，否则会出现内存泄漏。
    pChar4 = nullptr;


}