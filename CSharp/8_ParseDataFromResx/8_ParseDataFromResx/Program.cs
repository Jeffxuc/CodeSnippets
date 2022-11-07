using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Collections;
using System.IO;

namespace _8_ParseDataFromResx
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath = @"D:\AsusProject\AsusSystemDiagnosis\Tmp\Languages\Stage03\ToGet\";
            string filePath = @"D:\AsusProject\AsusSystemDiagnosis\Tmp\Languages\Stage03\ToGet\AsusSystemDiagnosis.en-US.resx";


            //GetAllString(folderPath);
            //GetDataFromResx_01(filePath);
            GetDataFromResx_02(filePath);

            MapLanguageCode();

            Console.ReadKey();
        }

        private static void GetDataFromResx_01(string filePath)
        {
            try
            {
                ResXResourceReader rsxr = new ResXResourceReader(filePath);

                int count = 0;
                foreach(DictionaryEntry d in rsxr)
                {
                    count++;
                    Console.WriteLine(">>>> " + count + " : " + d.Key.ToString() + ":\t" + d.Value.ToString());
                    
                }

                rsxr.Close();

                Console.WriteLine($"***** Number of string is : {count}");

            }
            catch(Exception exp)
            {
                Console.WriteLine("GetDataFromResx_01 Exception: " + exp.Message);
            }
        }


        private static void GetDataFromResx_02(string filePath)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string langCode = fileName.Split('.')[1];

                ResXResourceReader rr = new ResXResourceReader(filePath);
                IDictionaryEnumerator dict = rr.GetEnumerator();

                int count = 0;
                while(dict.MoveNext())
                {
                    //Console.WriteLine($"{dict.Key}: {dict.Value}");
                    count++;
                }

                Console.WriteLine($"[{langCode}] have string : {count}");
            }
            catch(Exception exp)
            {
                Console.WriteLine("GetDataFromResx_02 Exception: " + exp.Message);
            }
        }


        private static void GetAllString(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            string fileName;
            string langCode;

            foreach (string file in files)
            {

                //fileName = Path.GetFileNameWithoutExtension(file);
                //langCode = fileName.Split('.')[1];

                GetDataFromResx_02(file);

            }


        }


        private static void MapLanguageCode()
        {
            Dictionary<string, string> langCodeDic = new Dictionary<string, string>();

            langCodeDic.Add("en-US", "Edited English");
            langCodeDic.Add("ar-SA", "Arabic");
            langCodeDic.Add("cs-CZ", "Czech");
            langCodeDic.Add("da-DK", "Danish");
            langCodeDic.Add("de-DE", "German");
            langCodeDic.Add("el-GR", "Greek");
            langCodeDic.Add("es-ES", "Spanish");
            langCodeDic.Add("fi-FI", "Finnish");
            langCodeDic.Add("fr-FR", "French");
            langCodeDic.Add("he-IL", "Hebrew");
            langCodeDic.Add("hu-HU", "Hungarian");
            langCodeDic.Add("id-ID", "Indonesian");
            langCodeDic.Add("it-IT", "Italian");
            langCodeDic.Add("ja-JP", "Japanese");
            langCodeDic.Add("ko-KR", "Korean");
            langCodeDic.Add("nb-NO", "Norwegian");
            langCodeDic.Add("nl-NL", "Dutch");
            langCodeDic.Add("pl-PL", "Polish");
            langCodeDic.Add("pt-BR", "Portuguese Brazilian");
            langCodeDic.Add("pt-PT", "Portuguese Europe");
            langCodeDic.Add("ro-RO", "Romanian");
            langCodeDic.Add("ru-RU", "Russian");
            langCodeDic.Add("sk-SK", "Slovak");
            langCodeDic.Add("sv-SE", "Swedish");
            langCodeDic.Add("th-TH", "Thai");
            langCodeDic.Add("tr-TR", "Turkish");
            langCodeDic.Add("uk-UA", "Ukrainian");
            langCodeDic.Add("vi-VN", "Vietnamese");
            langCodeDic.Add("zh-CN", "Simplified Chinese");
            langCodeDic.Add("zh-TW", "Traditional Chinese");

        }




    }
}
