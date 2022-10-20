using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace LoadAssembly
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //string assemblyFile = "..\\..\\..\\..\\..\\ExternalDll\\SystemDiagnosis\\AsusSystemDiagnosis.dll";
            string assemblyFile = @"D:\Projects\CodeSnippets\WPF\2_LoadAssembly\ExternalDll\SystemDiagnosis\AsusSystemDiagnosis.dll";
            LoadDll(assemblyFile);
        }

        private void LoadDll(string assemblyFile)
        {
            Assembly dllAssembly = Assembly.LoadFrom(assemblyFile);

            //Type dllClassType = dllAssembly.GetType("AsusSystemDiagnosis.HwInspectUserControl");
            Type[] dllClassType = dllAssembly.GetTypes();
            object obj = Activator.CreateInstance(dllClassType[26]);
            myFrame.Content = obj as FrameworkElement;

        }
    }
}
