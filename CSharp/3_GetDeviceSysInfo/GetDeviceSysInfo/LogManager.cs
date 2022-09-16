using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDeviceSysInfo
{
    class LogManager
    {
        static string filename = "C:\\ProgramData\\test.log";

        public static void Log(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                string logMsg = String.Format("[{0}][{1}][{2}]: {3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), memberName, sourceLineNumber, message);

                if (!File.Exists(filename))
                {
                    using (StreamWriter sw = File.CreateText(filename))
                    {
                        sw.Close();
                    }

                }

                using (StreamWriter sw = File.AppendText(filename))
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
