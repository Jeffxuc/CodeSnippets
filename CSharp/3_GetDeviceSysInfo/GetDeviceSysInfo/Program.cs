using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDeviceSysInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ProductName = " + GetDeviceInfo.ProductName);
            Console.WriteLine("SerialNumber = " + GetDeviceInfo.SerialNumber);

            //GetDeviceInfo.GetBaseBoardProperty();
            //GetDeviceInfo.GetMotherboardProperty();
            //GetDeviceInfo.GetWin32_Bios();
            //GetDeviceInfo.GetWin32_ComputerSystem();
            GetDeviceInfo.GetWin32_Processor();

            Console.ReadKey();
        }
    }
}
