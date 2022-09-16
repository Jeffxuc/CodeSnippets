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

namespace CustomControls.ScrollBar_Style
{
    /// <summary>
    /// Interaction logic for ScrollBar_Style.xaml
    /// </summary>
    public partial class ScrollBar_Custom : UserControl
    {
        public ScrollBar_Custom()
        {
            InitializeComponent();
        }
    }

    public class DataModel_02 : ObservableCollection<string>
    {
        public DataModel_02()
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
