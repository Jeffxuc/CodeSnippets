/****************************************************************************
* Used for character and string handling,
* eg: Character, string, wstring, wchar convert.
****************************************************************************/
#pragma once

#include <iostream>
#include <string>
#include <windows.h>

using namespace std;

class ProcessCharacter 
{
public:
    ProcessCharacter();
    ~ProcessCharacter();

    /// <SUMMARY>
    /// 1. 字符串转换成宽字符串
    /// </SUMMARY>
    bool StringToWString(const string& strSrc, wstring& strDest);

    /// <SUMMARY>
    /// 2. 宽字符串转换成字符串
    /// </SUMMARY>
    bool WStringToString(IN const wstring& strSrc, OUT string& strDest);

    /// <SUMMARY>
    /// 3. char 数组转换成 wchar_t 数组
    /// </SUMMARY>
    wchar_t* CharToWchar(const char* charSrc);

    /// <SUMMARY>
    /// 4. wchar_t 数组转换成 char 数组
    ///     可能会失败：因为 wchSrc 为2字节，其表示的范围大于char类型
    /// </SUMMARY>
    char* WcharToChar(const wchar_t* wchSrc);


};
