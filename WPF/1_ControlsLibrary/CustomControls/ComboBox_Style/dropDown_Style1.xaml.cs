using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CustomControls.ComboBox_Style
{
    /// <summary>
    /// Interaction logic for dropDown_Style1.xaml
    /// </summary>
    public partial class dropDown_Style1 : UserControl
    {
        public dropDown_Style1()
        {
            InitializeComponent();

            
        }
    }

    public class DataModel_01 : ObservableCollection<string>
    {
        public DataModel_01()
        {
            Add("简体中文");
            Add("繁體中文");
            Add("English");
            Add("한국인");       // Korean
            Add("український");  // Ukrainian
            Add("やまと");       // Japanese
            Add("Español");     // Spanish
            Add("Deutsch");     // German
            Add("Français");    // French
            Add("Русский");     // Russian
            Add("Português");   // Portuguese Brazilian
        }
    }
}
