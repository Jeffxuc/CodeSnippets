#include "CharStringConvert.h"

ProcessCharacter::ProcessCharacter()
{
}

ProcessCharacter::~ProcessCharacter()
{
}

bool ProcessCharacter::StringToWString(const string& strSrc, wstring& strDest)
{
    int nSize = MultiByteToWideChar(CP_ACP, 0, strSrc.c_str(), (int)strSrc.length(), 0, 0);
    if (nSize <= 0)
        return false;

    wchar_t* pwszDst = new wchar_t[nSize + 1];
    int iRet = MultiByteToWideChar(CP_ACP, 0, strSrc.c_str(), (int)strSrc.length(), pwszDst, nSize);
    pwszDst[nSize] = 0;
    strDest.clear();
    strDest.assign(pwszDst);
    delete[] pwszDst;

    return true;
}


bool ProcessCharacter::WStringToString(IN const wstring& strSrc, OUT string& strDest)
{
    int nLen = WideCharToMultiByte(CP_ACP, 0, strSrc.c_str(), -1, NULL, 0, NULL, NULL);
    if (nLen <= 0)
        return false;

    char* pszDst = new char[nLen];
    int iRet = WideCharToMultiByte(CP_ACP, 0, strSrc.c_str(), -1, pszDst, nLen, NULL, NULL);
    pszDst[nLen - 1] = 0;
    strDest.clear();
    strDest.assign(pszDst);

    delete[] pszDst;

    return true;
}

wchar_t* ProcessCharacter::CharToWchar(const char* charSrc)
{
    size_t newsize = strlen(charSrc) + 1; //strlen不计算C-Style字串结尾的空字符
    wchar_t* wcString = new wchar_t[newsize];
    size_t convertedChars = 0;
    mbstowcs_s(&convertedChars, wcString, newsize, charSrc, _TRUNCATE);

    // 用于接受返回的 wchar_t* 指针需要在使用结束后，释放在此处所分配的内存，否则会造成内存泄漏
    return wcString;
}

char* ProcessCharacter::WcharToChar(const wchar_t* wchSrc)
{
    size_t origSize = wcslen(wchSrc) + 1;
    size_t convertedChars = 0;

    const size_t newSize = origSize * 2;
    char* retStr = new char[newSize];
    wcstombs_s(&convertedChars, retStr, newSize, wchSrc, _TRUNCATE);

    //使用完后，需要将返回的指针所指的数组进行释放
    return retStr;
}