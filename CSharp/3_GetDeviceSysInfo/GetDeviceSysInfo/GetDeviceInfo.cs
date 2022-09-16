/****************************************************************
 * 需要添加 Management Reference，方法如下：
 * 右键"Reference" --> "Add Reference..." --> "Assemblies" --> 勾选"System.Management" --> "OK"
 *****************************************************************/

using System;
using System.Management;

namespace GetDeviceSysInfo
{
    public static class GetDeviceInfo
    {
        private static ManagementObjectSearcher baseboard = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
        private static ManagementObjectSearcher motherboard = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_MotherboardDevice");
        private static ManagementObjectSearcher computerSystem = new ManagementObjectSearcher("select * from win32_ComputerSystem");
        private static ManagementObjectSearcher bios = new ManagementObjectSearcher("select * from Win32_Bios");
        private static ManagementObjectSearcher systemEnclosure = new ManagementObjectSearcher("select * from Win32_SystemEnclosure");
        private static ManagementObjectSearcher operatingSystem = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
        private static ManagementObjectSearcher processor = new ManagementObjectSearcher("select * from Win32_Processor");
        private static ManagementObjectSearcher videoController = new ManagementObjectSearcher("select * from Win32_VideoController");
        private static ManagementObjectSearcher physicalMemory = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");



        private static int baseBoardNum = 1;
        private static int motherBoardNum = 1;
        private static int biosNum = 1;
        private static int computerSystemNum = 1;
        private static int processorNum = 1;

        #region Win32_BaseBoard
        public static void GetBaseBoardProperty()
        {
            foreach (ManagementObject obj in baseboard.Get())
            {
                foreach (PropertyData tmp in obj.Properties)
                {
                    LogManager.Log($" {baseBoardNum}. {tmp.Name} = {tmp.Value}");
                    baseBoardNum++;
                }
            }

            LogManager.Log("********************************************************");
        }

        public static string ProductName
        {
            get
            {
                try
                {
                    foreach (ManagementObject obj in baseboard.Get())
                    {
                        return obj["Product"].ToString();
                    }
                    return "";
                }
                catch (Exception exp)
                {
                    return "";
                }
            }
        }
        #endregion

        #region Win32_MotherboardDevice
        public static void GetMotherboardProperty()
        {
            foreach (ManagementBaseObject obj in motherboard.Get())
            {
                foreach (PropertyData tmp in obj.Properties)
                {
                    LogManager.Log($" {motherBoardNum}. {tmp.Name} = {tmp.Value}");
                    motherBoardNum++;
                }
            }
            LogManager.Log("********************************************************");
        }

        public static string Manufacturer
        {
            get
            {
                try
                {
                    foreach (ManagementObject obj in motherboard.Get())
                    {
                        return obj["Manufacturer"].ToString();
                    }
                    return "";
                }
                catch (Exception exp)
                {
                    return "";
                }

            }
        }

        public static string ModelName
        {
            get
            {
                try
                {
                    foreach (ManagementObject obj in motherboard.Get())
                    {
                        return obj["Model"].ToString();
                    }
                    return "";
                }
                catch (Exception exp)
                {
                    return "";
                }
            }
        }

        #endregion

        #region Win32_Bios
        public static void GetWin32_Bios()
        {
            foreach (ManagementBaseObject obj in bios.Get())
            {
                foreach (PropertyData tmp in obj.Properties)
                {
                    LogManager.Log($" {biosNum}. {tmp.Name} = {tmp.Value}");
                    biosNum++;
                }
            }
            LogManager.Log("********************************************************");
        }

        /// <summary>
        /// Get the device SN
        /// </summary>
        public static string SerialNumber
        {
            get
            {
                try
                {
                    foreach(ManagementObject obj in bios.Get())
                    {
                        return obj["SerialNumber"].ToString();
                    }
                    return null;
                }
                catch(Exception exp)
                {
                    return null;
                }
            }
        }


        #endregion

        #region win32_ComputerSystem
        public static void GetWin32_ComputerSystem()
        {
            foreach (ManagementBaseObject obj in computerSystem.Get())
            {
                foreach (PropertyData tmp in obj.Properties)
                {
                    LogManager.Log($" {computerSystemNum}. {tmp.Name} = {tmp.Value}");
                    computerSystemNum++;
                }
            }
            LogManager.Log("********************************************************");
        }
        #endregion

        #region Win32_Processor
        public static void GetWin32_Processor()
        {
            foreach (ManagementBaseObject obj in processor.Get())
            {
                foreach (PropertyData tmp in obj.Properties)
                {
                    Console.WriteLine($" {processorNum}. {tmp.Name} = {tmp.Value}");
                    processorNum++;
                }
            }
            LogManager.Log("********************************************************");
        }
        #endregion

    }
}
