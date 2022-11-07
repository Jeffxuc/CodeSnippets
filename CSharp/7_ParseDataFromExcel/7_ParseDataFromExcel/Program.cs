using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Collections;

using NPOI.HSSF.UserModel;  // Only support excel 2007 previous version
using NPOI.XSSF.UserModel;  // Used for excel version 2007 and later, Recommend..
using NPOI.SS.UserModel;

namespace _7_ParseDataFromExcel
{
    /********************************
     * 1. HSSFWorkbook: used for suffix with ".xls", this is an earlier version of excel.
     * 2. XSSFWorkbook: used for suffix with "xlsx", this is new version of excel as Execel 2007 and later.
     *******************************/
    class Program
    {
        static string filePath01 = @"D:\AsusProject\AsusSystemDiagnosis\Tmp\String_US_Test01.xlsx";

        static Dictionary<string, string> rawData01 = new Dictionary<string, string>();
        static Dictionary<string, string> duplStrDic = new Dictionary<string, string>();

        static Dictionary<string, string> langCodeDic = new Dictionary<string, string>(); // Language code ----> country Region.
        static Dictionary<string, int> langDic = new Dictionary<string, int>();   // language ---> column
        static bool firstWrite = true;

        static void Main(string[] args)
        {

            //GetExcelDataM01();

            //SetExcelData();

            string sourceFile = @"D:\AsusProject\AsusSystemDiagnosis\Tmp\Languages\Stage02\AppUS.xlsx";
            string destFile = @"D:\AsusProject\AsusSystemDiagnosis\Tmp\Languages\Stage02\System Diagnosis_20221104.xlsx";

            //CheckDuplicateDataColumn(sourceFile, 0, 1);
            //CheckDuplicateDataColumn(destFile, 1, 3);

            //CheckDuplicateData_02(sourceFile, 0);
            //ReadExcelData(sourceFile, 1, 0);
            //WriteExcelData(destFile);

            #region Handle Multilanguage.

            string directPath = @"D:\AsusProject\AsusSystemDiagnosis\Tmp\Languages\Stage03\ToGet";
            string destFilePath = @"D:\AsusProject\AsusSystemDiagnosis\Tmp\Languages\Stage03\SystemDiagnosis_Dest_V01.xlsx";
            GetAllResxWriteToExcel(directPath, destFilePath);

            #endregion
            Console.ReadKey();
        }


        private static void EditExcelM01()
        {
            try
            {
                DataTable dtTable = new DataTable();
                List<string> rowList = new List<string>();
                ISheet sheet;



            }
            catch(Exception exp)
            {
                Console.WriteLine("Exception " + exp.Message);
            }
        }


        /// <summary>
        /// Get the the detail data of excel.
        /// </summary>
        private static void GetExcelDataM01()
        {
            try
            {
                
                XSSFWorkbook xssfworkbook; // XSSFWorkbook: Used for excel version 2007 and later.

                using (FileStream fileStream = new FileStream(filePath01, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    xssfworkbook = new XSSFWorkbook(fileStream);
                }

                // sheetNum_1/sheetNum_2 is the number of sheet in a Excel fileStream.
                int sheetNum_1 = xssfworkbook.NumberOfSheets;
                int sheetNum_2 = xssfworkbook.Count;


                // Get the content of specified sheet in a excel fileStream.
                ISheet sheet = xssfworkbook.GetSheet("Sheet1");
                for(int row=0; row <= sheet.LastRowNum; row++)
                {
                    // Check if the Specified row is empty.
                    if (sheet.GetRow(row) != null)
                    {
                        // GetRow: get a whole row content; GetCell: get a whole column content.
                        string valCol_0 = sheet.GetRow(row).GetCell(0).StringCellValue;
                        string valCol_1 = sheet.GetRow(row).GetCell(1).StringCellValue;
                        Console.WriteLine($"row {row}: {valCol_0} = {valCol_1}");

                        if (row > 10)
                            break;
                    }
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception " + exp.Message);
            }
        }

        /// <summary>
        /// Set the excel data, the excel with suffix ".xlsx"
        /// </summary>
        private static void SetExcelData()
        {
            try
            {
                XSSFWorkbook xworkbook;
                // 1.读取 Excel 的时候要是 FileMode.Open，FileAccess.Read来创建Stream，然后要关闭该Stream
                //   否则后面再写入数据的时候会出现问题
                using (FileStream fileStream = new FileStream(filePath01, FileMode.Open, FileAccess.Read))
                {
                    xworkbook = new XSSFWorkbook(fileStream);
                    fileStream.Close();
                }

                Console.WriteLine("Begin write data......\n");
                ISheet sheet = xworkbook.GetSheetAt(0);

                
                ISheet sheet2 = xworkbook.GetSheet("noFindStr01");
                // if the sheet2 not exist we will create a new.
                if(sheet2 == null)
                    sheet2 = xworkbook.CreateSheet("noFindStr01"); // Create a new sheet with specified name.

                for (int row = 0; row <= sheet.LastRowNum; ++row)
                {
                    if (sheet.GetRow(row) != null)
                    {
                        string setVal = row.ToString() + " - string...";

                        // If the cell is empty, GetCell will return null.
                        sheet.GetRow(row).CreateCell(2).SetCellValue(setVal);
                    }

                    sheet2.CreateRow(row).CreateCell(1).SetCellValue(row.ToString() + "  Write data");
                }

                //2. 以该文件来创建Stream，然后将xworkbook中的数据写入对应Stream中，写入完成后要关闭对应的Stream，
                //   必须要注意写入数据的模式为 FileMode.Create，FileAccess.Write ，否则会出错
                using (FileStream fileStream = new FileStream(filePath01, FileMode.Create, FileAccess.Write))
                {
                    xworkbook.Write(fileStream);
                    fileStream.Close();
                }

                Console.WriteLine("Complete write data operation!\n");

            }
            catch(Exception exp)
            {
                Console.WriteLine("Set ExcelData Exception " + exp.Message);
            }
        }


        /// <summary>
        /// Check the duplicate data in a specified coulumn.
        /// </summary>
        /// <param name="filePath">the .xlsx file path</param>
        /// <param name="sheetIndex">the sheet index of excel file</param>
        /// <param name="colNum">the specified column</param>
        private static void CheckDuplicateDataColumn(string filePath, int sheetIndex, int colNum)
        {
            try
            {
                Console.WriteLine("Begin check up data duplicate .....");

                Dictionary<string, int> dataDic = new Dictionary<string, int>();

                XSSFWorkbook xworkbook;
                using(FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    xworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                int duplicateCount = 0;
                ISheet sheet = xworkbook.GetSheetAt(sheetIndex);

                string firstVal= sheet.GetRow(0).GetCell(colNum).StringCellValue;
                Console.WriteLine($"The Title is: {firstVal}");
                for(int row = 1; row <= sheet.LastRowNum; ++row)
                {
                    string val = sheet.GetRow(row).GetCell(colNum).StringCellValue;
                    if(dataDic.ContainsKey(val))
                    {
                        Console.WriteLine($">>>[{++duplicateCount}] Duplicate: row = {row+1}, key = {val}\n    First occurrence: row = {dataDic[val]+1}");
                    }
                    else
                    {
                        dataDic.Add(val, row);
                    }

                }

                Console.WriteLine("Complete Check up");

            }
            catch(Exception exp)
            {
                Console.WriteLine("CheckDuplicateDataColumn exception :" + exp.Message);
            }
        }


        /// <summary>
        /// Check if the specified excel all columns have duplicate data.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="col"></param>
        private static void CheckDuplicateData_02(string filePath, int col)
        {
            try
            {
                XSSFWorkbook xworkboox;
                using(FileStream fs=new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    xworkboox = new XSSFWorkbook(fs);
                    fs.Close();
                }

                Dictionary<string, int> datas = new Dictionary<string, int>();

                int sheetCount = xworkboox.NumberOfSheets;
                for(int i=0; i<sheetCount; ++i)
                {

                    int duplicateCount = 1;
                    datas.Clear();
                    ISheet sheet = xworkboox.GetSheetAt(i);
                    Console.WriteLine($"### Begin Check Sheet: {i}  = {sheet.SheetName} ......");

                    for (int row=1; row <= sheet.LastRowNum; ++row)
                    {
                        IRow RowData = sheet.GetRow(row);
                        if (RowData != null)
                        {
                            string resVal = RowData.GetCell(col).StringCellValue;
                            if (resVal == "")
                                continue;

                            if(datas.ContainsKey(resVal))
                            {
                                Console.WriteLine($">>> [{duplicateCount}] - String: {resVal} \n   First occur : {datas[resVal]}");
                                duplicateCount++;
                            }
                            else
                            {
                                datas.Add(resVal, row);
                            }
                        }
                    }

                    Console.WriteLine($"==> String count : {datas.Count}");

                }

                Console.WriteLine("Complete All Check.");

            }
            catch(Exception exp)
            {
                Console.WriteLine("CheckDuplicateData_02 Exception : " + exp.Message);
            }
        }



        /// <summary>
        /// Use the excel data initial a Dictionary, and Check for duplicate values in the column as key
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="keyColumn">the column value set as key of the dictionary</param>
        /// <param name="valColumn">the column value set as value of the dictionary</param>
        private static void ReadExcelData(string filePath, int keyColumn, int valColumn)
        { 
            try
            {

                XSSFWorkbook workbook;
                using(FileStream fs =new FileStream(filePath,FileMode.Open,FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                rawData01.Clear();

                ISheet sheet = workbook.GetSheetAt(1);

                for(int row=1; row <= sheet.LastRowNum; ++row)
                {
                    string valStr = sheet.GetRow(row).GetCell(valColumn).StringCellValue;

                    if (valStr == "")
                        return;

                    string keyStr = sheet.GetRow(row).GetCell(keyColumn).StringCellValue;

                    if (rawData01.ContainsKey(keyStr))
                    {
                        duplStrDic.Add(keyStr, valStr);
                        Console.WriteLine($"Duplicate row {row} >>> {valStr} = {keyStr};\n    First position --- {rawData01[keyStr]}");
                    }
                    else
                    {
                        rawData01.Add(keyStr, valStr);
                    }
                    
                }

            }
            catch(Exception exp)
            {
                Console.WriteLine("**** ReadExcelData Exception : " + exp.Message);
            }
        }


        private static void WriteExcelData(string filePath)
        {
            try
            {
                if(rawData01.Count == 0)
                {
                    Console.WriteLine("The data in Dictionary is null");
                    return;
                }
                else
                {
                    Console.WriteLine("The Dictionary contain = " + rawData01.Count+" string , Begin write data");
                }

                XSSFWorkbook xworkbook;
                using(FileStream fs=new FileStream(filePath,FileMode.Open, FileAccess.Read))
                {
                    xworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet1 = xworkbook.GetSheetAt(1);
                ISheet sheet2 = xworkbook.GetSheet("noFindStr");
                if(sheet2 == null)
                    sheet2 = xworkbook.CreateSheet("noFindStr");

                for(int row=1; row <= sheet1.LastRowNum; ++row)
                {
                    string keyVal = sheet1.GetRow(row).GetCell(3).StringCellValue;
                    if(rawData01.ContainsKey(keyVal))
                    {
                        sheet1.GetRow(row).CreateCell(0).SetCellValue(rawData01[keyVal]);
                        rawData01.Remove(keyVal);
                    }
                }

                if(rawData01.Count > 0)
                {
                    int i = 0;
                    foreach(string keyStr in rawData01.Keys)
                    {
                        // CreateRow, CreateCell 都是新创建行或列，如果已存在该行或列，就会用新创建的覆盖之前的。这里要格外注意。
                        sheet2.CreateRow(i).CreateCell(0).SetCellValue(rawData01[keyStr]);
                        // 由于上面已经 CreateRow(i) ，如果想在该行后面继续添加数据，就应该用GetRow(i)，如果在此用CreateRow(i)，就会覆盖之前的内容
                        // 导致之前的内容丢失
                        sheet2.GetRow(i).CreateCell(1).SetCellValue(keyStr);
                        i++;
                    }
                }

                if(duplStrDic.Count > 0)
                {
                    int i = sheet1.LastRowNum + 1;
                    foreach(string keyStr in duplStrDic.Keys)
                    {
                        sheet1.CreateRow(i).CreateCell(0).SetCellValue(duplStrDic[keyStr]);
                        sheet1.GetRow(i).CreateCell(3).SetCellValue(keyStr);
                        i++;
                    }

                }

                using(FileStream fs = new FileStream(filePath,FileMode.Create,FileAccess.Write))
                {
                    xworkbook.Write(fs);
                    fs.Close();
                }

                Console.WriteLine("Complete wirte data");
            }
            catch (Exception exp)
            {
                Console.WriteLine("**** WriteExcelData Exception : " + exp.Message);
            }

        }


        /// <summary>
        /// Read data from a source excel file and write this data to another excel file.
        /// </summary>
        /// <param name="srcFile">the excel file where to read data</param>
        private static void ReadWriteDatas(string srcFile, string destFile)
        {
            try
            {
                XSSFWorkbook xworkRead;
                using(FileStream fs=new FileStream(srcFile, FileMode.Open, FileAccess.Read))
                {
                    xworkRead = new XSSFWorkbook(fs);
                    fs.Close();
                }

                Dictionary<string, string> dataSrc = new Dictionary<string, string>();

                int sheetCount = xworkRead.NumberOfSheets;
                string sheetName;
                for(int sheetIndex=0; sheetIndex < sheetCount; ++sheetIndex)
                {
                    dataSrc.Clear();
                    ISheet sheetSrc = xworkRead.GetSheetAt(sheetIndex);
                    sheetName = sheetSrc.SheetName;

                    #region 1. Read a sheet data and use it to Initial a Dictionary.
                    for (int row=0; row<= sheetSrc.LastRowNum; ++row)
                    {
                        IRow rowData = sheetSrc.GetRow(row);
                        if(rowData != null)
                        {
                            string keyStr = rowData.GetCell(0).StringCellValue;
                            if (keyStr == "")
                                continue;

                            if(!dataSrc.ContainsKey(keyStr))
                            {
                                string valStr = rowData.GetCell(1).StringCellValue;
                                dataSrc.Add(keyStr, valStr);
                            }
                            
                        }
                    }

                    #endregion

                    #region 2. Write the data of Dictionary to destination excel file.

                    WriteDataToExcel(destFile, dataSrc);


                    Console.WriteLine($">>> Complete write language: {sheetName}, Number: {sheetIndex}");
                    #endregion

                }



            }
            catch(Exception exp)
            {
                Console.WriteLine("ReadWriteDatas Exception " + exp.Message);
            }

        }


        /// <summary>
        /// 获取 language 及其对应的列数值
        /// </summary>
        /// <param name="destFile"></param>
        private static void GetTitleToDic(string destFile)
        {
            try
            {
                XSSFWorkbook xworkWrite;
                using (FileStream fs = new FileStream(destFile, FileMode.Open, FileAccess.Read))
                {
                    xworkWrite = new XSSFWorkbook(fs);
                    fs.Close();
                }

                langDic.Clear();

                ISheet sheetWrite = xworkWrite.GetSheetAt(1);

                // Attention : LastCellNum is 1-based start.
                for (int col = 0; col < sheetWrite.GetRow(0).LastCellNum; ++col)
                {
                    string langStr = sheetWrite.GetRow(0).GetCell(col).StringCellValue;
                    if (langStr == "" || langStr == null)
                        continue;

                    if (langDic.ContainsKey(langStr))
                    {
                        Console.WriteLine("The langStr has exist..");
                    }
                    else
                    {
                        langDic.Add(langStr, col);
                    }
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine("GetTitleToDic Exception " + exp.Message);
            }
        }


        private static void WriteDataToExcel(string destFile, Dictionary<string,string> srcDic)
        {
            try
            {
                XSSFWorkbook xworkWrite;
                using (FileStream fs = new FileStream(destFile, FileMode.Open, FileAccess.Read))
                {
                    xworkWrite = new XSSFWorkbook(fs);
                    fs.Close();
                }
   
                Dictionary<string, int> idDic = new Dictionary<string, int>();     // id --- row

                ISheet sheetWrite = xworkWrite.GetSheetAt(1);

                // 1. get the column number of current language.
                string curLangStr = srcDic["ID"];
                int curCol = 0;
                if (langDic.ContainsKey(curLangStr))
                {
                    curCol = langDic[curLangStr];
                    srcDic.Remove("ID");
                }
                else
                {
                    Console.WriteLine($"Can't find language: {curLangStr}" );
                    return;
                }

                // 2. get the current ID in sheet.
                if(!firstWrite)
                {
                    for (int row = 1; row <= sheetWrite.LastRowNum; ++row)
                    {
                        IRow rowData = sheetWrite.GetRow(row);
                        string idKey = rowData.GetCell(0).StringCellValue;
                        if (idKey == "")
                            continue;

                        if (!idDic.ContainsKey(idKey))
                        {
                            idDic.Add(idKey, row);
                        }

                    }
                }


                // 3. Write the srcDic data to correspond Excel file.
                int rowAdd = idDic.Count + 1;
                foreach(string srcKey in srcDic.Keys)
                {
                    if(idDic.ContainsKey(srcKey))
                    {
                        int rowNum = idDic[srcKey];
                        sheetWrite.GetRow(rowNum).GetCell(curCol).SetCellValue(srcDic[srcKey]);
                    }
                    else
                    {
                        sheetWrite.GetRow(rowAdd).GetCell(0).SetCellValue(srcKey);
                        sheetWrite.GetRow(rowAdd).GetCell(curCol).SetCellValue(srcDic[srcKey]);
                        rowAdd++;
                        //Console.WriteLine($"Add new String : {srcKey} = {srcDic[srcKey]}");
                    }
                }

                using(FileStream fs=new FileStream(destFile, FileMode.Create, FileAccess.Write))
                {
                    xworkWrite.Write(fs);
                    fs.Close();
                }

                firstWrite = false;

            }
            catch (Exception exp)
            {
                Console.WriteLine("WriteDataToExcel Exception " + exp.Message);
            }
        }

        /// <summary>
        /// Get a resx file key-value and store it in a dictionary.
        /// </summary>
        /// <param name="file"></param>
        private static Dictionary<string,string> GetDataFromResx(string file)
        {
            Dictionary<string, string> strDic = new Dictionary<string, string>();
            try
            {

                ResXResourceReader rr = new ResXResourceReader(file);
                IDictionaryEnumerator dict = rr.GetEnumerator();

                while (dict.MoveNext())
                {
                    if (strDic.ContainsKey(dict.Key.ToString()))
                        Console.WriteLine("The Key has exist : " + dict.Key.ToString());

                    strDic.Add(dict.Key.ToString(), dict.Value.ToString());
                    
                }


            }
            catch(Exception exp)
            {
                Console.WriteLine("GetDataFromResx Exception " + exp.Message);
            }

            return strDic;

        }


        private static void MapLanguageCode()
        {
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


        private static void GetAllResxWriteToExcel(string directPath, string destFilePath)
        {
            try
            {
                GetTitleToDic(destFilePath);
                MapLanguageCode();


                string[] files = Directory.GetFiles(directPath);
                foreach(string file in files)
                {

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string langCode = fileName.Split('.')[1];

                    Dictionary<string, string> resDic = GetDataFromResx(file);
                    if(resDic.Count > 0)
                    {
                        Console.WriteLine($"[{langCode}] has key-value number: {resDic.Count}");
                    }
                    else
                    {
                        Console.WriteLine($"[{langCode}] is empty !!!!");
                    }

                    WriteDicDataToExcel(langCode, destFilePath, resDic);

                }

            }
            catch(Exception exp)
            {
                Console.WriteLine("GetAllResxWriteToExcel Exception " + exp.Message);
            }
        }


        private static void WriteDicDataToExcel(string srcFile, string destFile, Dictionary<string,string> resDic)
        {
            try
            {
                XSSFWorkbook xworkWrite;
                using (FileStream fs = new FileStream(destFile, FileMode.Open, FileAccess.Read))
                {
                    xworkWrite = new XSSFWorkbook(fs);
                    fs.Close();
                }

                //1. Get current column number where data will be wrote.
                string langRegion = langCodeDic[srcFile];
                int curColumn = langDic[langRegion];

                //2. Get current row number where data will be wrote.
                Dictionary<string, int> idRowDic = new Dictionary<string, int>();     // id --- row
                ISheet sheetWrite = xworkWrite.GetSheetAt(1);
                for (int row = 1; row <= sheetWrite.LastRowNum; ++row)
                {
                    IRow rowData = sheetWrite.GetRow(row);
                    string idKey = rowData.GetCell(0).StringCellValue;
                    if (idKey == "")
                        continue;

                    if (!idRowDic.ContainsKey(idKey))
                    {
                        idRowDic.Add(idKey, row);
                    }

                }

                int rowAdd = idRowDic.Count + 1;
                foreach (string srcKey in resDic.Keys)
                {
                    if (idRowDic.ContainsKey(srcKey))
                    {
                        int rowNum = idRowDic[srcKey];
                        sheetWrite.GetRow(rowNum).GetCell(curColumn).SetCellValue(resDic[srcKey]);
                    }
                    else
                    {
                        sheetWrite.GetRow(rowAdd).GetCell(0).SetCellValue(srcKey);
                        sheetWrite.GetRow(rowAdd).GetCell(curColumn).SetCellValue(resDic[srcKey]);
                        rowAdd++;
                    }
                }

                using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                {
                    xworkWrite.Write(fs);
                    fs.Close();
                }


            }
            catch(Exception exp)
            {
                Console.WriteLine("WriteDicDataToExcel Exception " + exp.Message);
            }

        }

    }
}
