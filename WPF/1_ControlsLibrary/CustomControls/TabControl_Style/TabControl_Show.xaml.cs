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

/****************************************************************
 * TabControl is a ItemsControl, so it can have many items.
 * TabItem is a ContentControl, so it can have only one children
 ***************************************************************/

namespace CustomControls.TabControl_Style
{
    /// <summary>
    /// Interaction logic for TabControl_Show.xaml
    /// </summary>
    public partial class TabControl_Show : UserControl
    {
        public TabControl_Show()
        {
            InitializeComponent();
        }
    }
}
