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
using Microsoft.Expression.Shapes;

namespace CustomControls.CircularProgressBar_Style
{
    /// <summary>
    /// Interaction logic for CircleProgressBar_01.xaml
    /// </summary>
    public partial class CircleProgressBar_01 : UserControl
    {
        double progressNum = 0;

        public CircleProgressBar_01()
        {
            InitializeComponent();

            btnAdd01.Click += BtnAdd01_Click;
            btnReset01.Click += (s1, e1) =>
            {
                DoubleCollection dcl = new DoubleCollection() { 0 };
                circle01.StrokeDashArray = dcl;
                progressNum = 0;
            };

            btn21.Click += (s2, e2) =>
            {
                progressNum += 10;
                arc21.EndAngle = progressNum;
            };

        }

        private void BtnAdd01_Click(object sender, RoutedEventArgs e)
        {
            if (progressNum < 101)
                progressNum += 10;

            DoubleCollection dc = new DoubleCollection();

            // 已经完成的进度
            dc.Add(74 * Math.PI * progressNum / 100 / 6);

            // 未完成的进度
            dc.Add(74 * Math.PI * (100 - progressNum) / 100 / 6);

            circle01.StrokeDashArray = dc;

        }
    }
}
