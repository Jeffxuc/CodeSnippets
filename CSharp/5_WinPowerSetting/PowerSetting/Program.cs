using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSetting
{
    class Program
    {
        static void Main(string[] args)
        {

            //PowerSettings.SetACHibernateTime(3600);
            //PowerSettings.SetDCHibernateTime(3600);


            Console.WriteLine("AC Timeout = "+PowerSettings.GetScreenDisplayACTimeOut());
            Console.WriteLine("DC Timeout = "+PowerSettings.GetScreenDisplayDCTimeOut());

            PowerSettings.SetScreenOffACTime(0);
            PowerSettings.SetScreenOffDCTime(3600);

            //Console.ReadKey();
        }
    }
}
