using System;
using System.Collections.Generic;
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

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //ComboBox_Style.dropDown_Style1 dropDown_Style1 = new ComboBox_Style.dropDown_Style1();
            ScrollBar_Style.ScrollBar_Custom scrollBar_Custom = new ScrollBar_Style.ScrollBar_Custom();


            mainGird.Children.Add(scrollBar_Custom);
        }
    }
}
