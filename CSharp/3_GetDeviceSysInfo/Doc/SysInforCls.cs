using Home.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Home.SystemInformation
{
    //public enum ChasisTypes
    //{
    //    DESKTOP = 3,
    //    NOTEBOOK = 10,
    //    ALLINONE = 13,
    //    TABLET = 30,
    //    CONVERTIBLE = 31,
    //    DETACHABLE = 32
    //};

    public class OSCls
    {
        public string Name = "";
        public string Arch = "";
        public string Version = "";
    }

    public class BiosCls
    {
        public string Name = "";
        public string ReleaseData = "";
    }

    public class HddCls
    {
        public string Model = "";
        public string Size = "";
        public string MediaType = "";
    }

    public class SysInforCls
    {
        public string ModelName
        {
            get; set;
        }

        public string FamilyName
        {
            get; set;
        }

        public string ProductName
        {
            get; set;
        }

        public string SN
        {
            get; set;
        }

        public string PPID
        {
            get; set;
        }

        public string DeviceType
        {
            get; set;
        }

        public OSCls OsInfor
        {
            get
            {
                return _OsInfor;
            }
            set
            {
                if (_OsInfor != value)
                {
                    _OsInfor = value;
                }
            }
        }
        private OSCls _OsInfor = new OSCls();

        public BiosCls BiosInfo
        {
            get
            {
                return _BiosInfo;
            }
            set
            {
                if (_BiosInfo != value)
                {
                    _BiosInfo = value;
                }
            }
        }
        private BiosCls _BiosInfo = new BiosCls();

        public string CPUName
        {
            get; set;
        }

        public List<string> GPUNameLst
        {
            get
            {
                return _GPUNameLst;
            }
            set
            {
                if (_GPUNameLst != value)
                {
                    _GPUNameLst = value;
                }
            }
        }
        private List<string> _GPUNameLst = new List<string>();

        public List<HddCls> HddsInfoLst
        {
            get
            {
                return _HddsInfoLst;
            }
            set
            {
                if (_HddsInfoLst != value)
                {
                    _HddsInfoLst = value;
                }
            }
        }
        private List<HddCls> _HddsInfoLst = new List<HddCls>();

        public List<string> MemSizeLst
        {
            get
            {
                return _MemSizeLst;
            }
            set
            {
                if (_MemSizeLst != value)
                {
                    _MemSizeLst = value;
                }
            }
        }
        private List<string> _MemSizeLst = new List<string>();

        public List<string> WirelessCardInfoLst
        {
            get
            {
                return _WirelessCardInfoLst;
            }
            set
            {
                if (_WirelessCardInfoLst != value)
                {
                    _WirelessCardInfoLst = value;
                }
            }
        }
        private List<string> _WirelessCardInfoLst = new List<string>();

        public void GetSysInfor()
        {
            ModelName = GetModelNameFunc();
            ProductName = GetProductNameFunc();
            // special feature for mini pc from pm
            FamilyName = ProductName.ToUpper().Contains("MINIPC") ? "Mini PC" : GetFamilyNameFunc();
            //SN = GetSNFunc();
            PPID = GetPPIDFunc();
            DeviceType = GetDeviceTypeFunc();
        }

        public void GetOsInfor()
        {
            OsInfor = GetOsInforFunc();
        }

        public void GetBiosInfor()
        {
            BiosInfo = GetBiosInforFunc();
        }

        public void GetCpuInfor()
        {
            CPUName = GetCPUInforFunc();
        }

        public void GetGpuInfor()
        {
            if (GPUNameLst.Count > 0)
                GPUNameLst.Clear();

            GPUNameLst = GetGPUInforFunc();

        }

        public void GetHddInfor()
        {
            if (HddsInfoLst.Count > 0)
                HddsInfoLst.Clear();
            HddsInfoLst = GetHddInforFunc();
        }

        public void GetWireCdInfor()
        {
            if (WirelessCardInfoLst.Count > 0)
                WirelessCardInfoLst.Clear();

            WirelessCardInfoLst = GetWirelessCardsInforFunc();
        }

        public void GetMemInfor()
        {
            if (MemSizeLst.Count > 0)
            {
                MemSizeLst.Clear();
            }

            MemSizeLst = GetMemsSizeFunc();
        }

        /// <summary>
        /// 获得ModelName
        /// </summary>
        /// <returns></returns>
        public string GetModelNameFunc()
        {
            string str = "";
            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from Win32_Baseboard");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    str = process["Product"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }

            return str;
        }

        public bool IsSystemRegionTW()
        {
            bool bResult = false;
            string systemRegionValue = GetRegistryProperty<string>(@"HKEY_CURRENT_USER\Control Panel\International\Geo", "Name");
            if (String.IsNullOrEmpty(systemRegionValue) == false && systemRegionValue == "TW")
            {
                bResult = true;
            }
            return bResult;
        }

        public static T GetRegistryProperty<T>(string path, string property)
        {
            object v = Registry.GetValue(path, property, null);
            if (v != null && typeof(T).IsInstanceOfType(v))
            {
                return (T)v;
            }
            else
            {
                return default(T);
            }
        }

    /// <summary>
    /// 获得FamilyName
    /// </summary>
    /// <returns></returns>
        public string GetFamilyNameFunc()
        {
            string str = "";
            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from win32_ComputerSystem");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    str = process["SystemFamily"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }

            return str;
        }

        public string GetProductNameFunc()
        {
            string str = "";
            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from win32_ComputerSystem");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    str = process["Model"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }

            return str;
        }

        public string GetSNFunc()
        {
            string str = "";
            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from Win32_Bios");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    str = process["SerialNumber"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }

            return str;
        }

        public string GetPPIDFunc()
        {
            string str = "";
            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from Win32_Baseboard");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    str = process["SerialNumber"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }

            return str;
        }

        public string GetDeviceTypeFunc()
        {
            string deviceType = "unknown";
            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from Win32_SystemEnclosure");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    UInt16[] type = (UInt16[])(process["ChassisTypes"]);

                    if (type.Count() > 0)
                    {
                        switch (type[0])
                        {
                            case 3:
                                deviceType = "Desktop";
                                break;
                            case 10:
                            case 30:
                            case 31:
                            case 32:
                                deviceType = "Notebook";
                                break;
                            case 13:
                                deviceType = "All in One";
                                break;
                            default:
                                deviceType = "unknown";
                                break;
                        }
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }
            return deviceType;
        }

        public OSCls GetOsInforFunc()
        {
            OSCls os = new OSCls();

            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    os.Name = process["Caption"].ToString();
                    os.Arch = process["OSArchitecture"].ToString();
                    os.Version = process["Version"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }
            return os;
        }

        public BiosCls GetBiosInforFunc()
        {
            BiosCls bios = new BiosCls();

            try
            {
                CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from Win32_Bios");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    bios.Name = process["Name"].ToString();
                    bios.ReleaseData = ManagementDateTimeConverter.ToDateTime(process["ReleaseDate"].ToString()).ToString("yyyyMMdd", cultureInfo);
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }
            return bios;
        }

        public string GetCPUInforFunc()
        {
            string str = "";
            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from Win32_Processor");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    str = process["Name"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }

            return str;
        }

        public List<string> GetGPUInforFunc()
        {
            List<string> lst = new List<string>();
            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from win32_VideoController");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    lst.Add(process["Caption"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }

            return lst;
        }

        public List<string> GetMemsSizeFunc()
        {
            List<string> lst = new List<string>();
            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    lst.Add(process["Capacity"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }

            return lst;
        }

        public List<HddCls> GetHddInforFunc()
        {
            List<HddCls> lst = new List<HddCls>();

            try
            {
                ManagementObjectSearcher search1
                                  = new ManagementObjectSearcher("root\\Microsoft\\Windows\\Storage", "select * from MSFT_PhysicalDisk");
                foreach (ManagementObject process in search1.Get())
                {
                    process.Get();
                    HddCls hdd = new HddCls();
                    hdd.Model = process["Model"].ToString();
                    hdd.MediaType = process["MediaType"].ToString();
                    hdd.Size = process["Size"].ToString();
                    lst.Add(hdd);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteHomeExceptionLog(ex);
            }

            return lst;
        }


        public List<string> GetWirelessCardsInforFunc()
        {
            List<string> lst = new List<string>();

            ManagementObjectSearcher search1
                                 = new ManagementObjectSearcher("select * from win32_NetworkAdapter where physicalAdapter='true'");
            foreach (ManagementObject process in search1.Get())
            {
                process.Get();

                string lowerName = process["Name"].ToString().ToLower();

                if (process["PNPDeviceID"].ToString().ToLower().Contains("pci") == true
                    && (lowerName.Contains("wireless") || lowerName.Contains("wlan") ||
                    lowerName.Contains("wifi") || lowerName.Contains("wi-fi") ||
                    lowerName.Contains("wi fi")))
                {
                    lst.Add(process["Name"].ToString());
                }
            }

            return lst;
        }


    }
}
