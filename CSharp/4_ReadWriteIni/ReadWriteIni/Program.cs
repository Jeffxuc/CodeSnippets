using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadWriteIni
{
    class Program
    {
        static void Main(string[] args)
        {
            //string iniFile = @"C:\ProgramData\myTest.ini";
            string iniFile1 = @"C:\ProgramData\myTest01.ini";

            //ReadWriteIniFile.WriteValueToKey("CHT", "key02", "value09992", iniFile1);
            string val = ReadWriteIniFile.GetValueByKey("CHT", "key02", iniFile1);

            //Dictionary<string, string> resDic = ReadWriteIniFile.ParseKeyValueToDictionary("CHS", iniFile);

            //ReadWriteIniFile.DeleteKeyValPair("CHT", "ID_PreparePrompt", iniFile);
            //ReadWriteIniFile.DeleteEntireSection("Japanese", iniFile);

            //ReadWriteIniFile.WriteValueToKey("CHT", "key02", "value02", iniFile1);

            // 写入整节数据时，要注意每两对 Key=Value 之间有截止符 '\0'
            string entireSectionData = "MyKey01=Value01" + '\0' +
                                        "测试Key=\"键值对02   \"" + '\0' + 
                                        "TestKey03" + "=" + "FinalValue03";

            ReadWriteIniFile.WriteKeyValToSection("03_TestSection", entireSectionData, iniFile1);

            //string val = ReadWriteIniFile.GetValueByKey("03_TestSection", "测试Key", iniFile1);

        }
    }
}
