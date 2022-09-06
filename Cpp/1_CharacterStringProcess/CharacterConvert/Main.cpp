#include "CharStringConvert.h"

int main()
{
    ProcessCharacter* convertChar = new ProcessCharacter();

    // 1. �ַ���ת���ַ���
    string str1 = "Image";
    wstring dest1;
    convertChar->StringToWString(str1, dest1);

    // 2. ���ַ���ת�ַ���
    wstring wstr2 = L"����Image";
    string retStr2;
    convertChar->WStringToString(wstr2,retStr2);

    // 3. �ַ�����ת���ַ�����
    const char* pChar3 = "hello,Test";
    wchar_t* pWchar3 = convertChar->CharToWchar(pChar3);
    delete[] pWchar3; // ��Ҫ�ͷ�ת�������� new ������ռ䣬���������ڴ�й©��
    pWchar3 = nullptr;

    // 4. ���ַ�����ת�ַ�����
    const wchar_t* pWchar4 = L"Hello,Test";
    char* pChar4 = convertChar->WcharToChar(pWchar4);
    delete[] pChar4; // ��Ҫ�ͷ�ת�������� new ������ռ䣬���������ڴ�й©��
    pChar4 = nullptr;


}