using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReadWriteIni
{
    public static class ReadWriteIniFile
    {
        #region Import Dll
        // Only Get the value of a Specified key in a section.
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        // Get all key-value from a Specified section.
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileSection(string section, char[] retVal, int nSize, string lpFileName);

        /// <summary>
        /// Only write a value to the specified key in a section.
        /// 1). if "Key=null" the entire section, including all entries within the section, is deleted.
        /// 2). if "Value=null" the Key-Value pair will be deleted.
        /// </summary>
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        // Write key-values to a section and delete all original key-values if exist.
        [DllImport("kernel32", CharSet=CharSet.Unicode)]
        private static extern int WritePrivateProfileSection(string section, string lpString, string filePath);
        #endregion


        #region Implemented in C#
        /// <summary>
        /// 获取指定Section中指定Key对应的Value
        /// 注意其对读取字符的最大数量有限制，如下设置为1024字符，超出的部分将无法读取
        /// </summary>
        public static string GetValueByKey(string sectionName, string key, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    StringBuilder tmp = new StringBuilder(1024);
                    int getCharNum = GetPrivateProfileString(sectionName, key, "", tmp, 1024, filePath);
                    return tmp.ToString();
                }
                else
                {
                    Console.WriteLine("Can't find :" + filePath);
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            return "";

        }

        /// <summary>
        /// 获取一个Section中所有的 Key-Value 值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string[] GetSectionValues(string section, string filePath)
        {
            if (File.Exists(filePath))
            {
                char[] keyValues = new char[4096];
                GetPrivateProfileSection(section, keyValues, 4096, filePath);
                string[] retArr = new string(keyValues).Trim('\0').Split('\0');
                return retArr;
            }
            return null;
        }

        /// <summary>
        /// 将一个Section中的所有 Key-Value 解析成Dictionary中键值对
        /// </summary>
        /// <param name="section"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseKeyValueToDictionary(string section, string filePath)
        {
            Dictionary<string, string> keyValDic = new Dictionary<string, string>();

            string[] keyValStr = GetSectionValues(section, filePath);
            if(keyValStr != null)
            {
                string[] pairs = new string[2];
                foreach (string keyVal in keyValStr)
                {
                    if (keyVal.Contains("="))
                    {
                        pairs = keyVal.Split('=');
                        if (!keyValDic.ContainsKey(pairs[0]))
                            keyValDic.Add(pairs[0], pairs[1]);

                    }
                }
 
            }

            return keyValDic;
        }

        /// <summary>
        /// Write a pair value and key to a specified section.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        public static void WriteValueToKey(string section, string key, string value, string filePath)
        {
            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            WritePrivateProfileString(section, key, value, filePath);
        }

        /// <summary>
        /// Write one or more pairs key-value to a section.
        /// it will first delete entire original data in this section and then write new data.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="keyValStr"></param>
        /// <param name="filePath"></param>
        public static void WriteKeyValToSection(string section, string keyValStr, string filePath)
        {
            // 该函数会将原本是 UTF-8 编码的文本转换成 ASNI 格式
            WritePrivateProfileSection(section, keyValStr, filePath);
        }

        /// <summary>
        /// Delete the a Key-Value pair by a key.
        /// if set the Value=null, it will delete the Key-Value pair.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="Key"></param>
        /// <param name="filePath"></param>
        public static void DeleteKeyValPair(string section, string Key, string filePath)
        {
            if (File.Exists(filePath))
                WritePrivateProfileString(section, Key, null, filePath);
        }

        /// <summary>
        /// Delete the entire section
        /// set the key=null
        /// </summary>
        /// <param name="section"></param>
        /// <param name="filePath"></param>
        public static void DeleteEntireSection(string section,string filePath)
        {
            if (File.Exists(filePath))
                WritePrivateProfileString(section, null, null, filePath);
        }

        #endregion


    }
}
