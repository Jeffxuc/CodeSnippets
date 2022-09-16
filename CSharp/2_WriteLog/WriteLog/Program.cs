using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteLog
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFilePath = "C:\\ProgramData\\test.log";
            string msg_1 = "just test write a log for win32 App.";

            LogManager.filePath = logFilePath;
            LogManager.WinLog(msg_1);
        }
    }
}
