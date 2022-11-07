using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _6_CheckBitLockerEnable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CheckIsBitLockerEnable_01();

        }


        /*
         * Check the BitLocker in cmd(need administrator privileges): manage-bde status
         * 
         * 
         * https://stackoverflow.com/questions/23841973/how-to-tell-if-drive-is-bitlocker-encrypted-without-admin-privilege
         * 
         * https://stackoverflow.com/questions/41308245/detect-bitlocker-programmatically-from-c-sharp-without-admin
         */

        /// <summary>
        /// Use the Nuget : WindowsAPICodePack
        /// </summary>
        private void CheckIsBitLockerEnable_01()
        {

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = "-command (New-Object -ComObject Shell.Application).NameSpace('C:').Self.ExtendedProperty('System.Volume.BitLockerProtection')";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                StreamReader reader = process.StandardOutput;
                string output = reader.ReadToEnd().Substring(0, 1); //needed as output would otherwise be 1\r\n (if encrypted)
               
                process.WaitForExit();

                // 1,3,5 ：BitLocker -> On
                // 2: BitLocker -> Off
                // 4: BitLocker -> BitLocker Decrypting
                res.Content = output;

            }
            catch(Exception exp)
            {
                exceptionMsg.Content = "Exception info : " + exp.Message;
            }

        }
    }
}
