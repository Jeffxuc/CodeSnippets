using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WriteLog
{
    class LogManager
    {
        
        public static string filePath ;

        /// <summary>
        /// Write log for win32 App. eg: WPF.
        /// </summary>
        public static void WinLog(string message,
                                  [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                                  [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                string logMsg = String.Format("[{0}][{1}][{2}]: {3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), memberName, sourceLineNumber, message);

                if (!File.Exists(filePath))
                {
                    using (StreamWriter sw = File.CreateText(filePath))
                    {
                        sw.Close();
                    }

                }

                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(logMsg);
                    sw.Close();
                }


            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
