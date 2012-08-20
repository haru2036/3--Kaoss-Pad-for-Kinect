using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace kinectTestWPF1
{
    /// <summary>
    /// DummyWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DummyWindow : Window
    {
        public DummyWindow()
        {
            InitializeComponent();

            label1.Content = "Device ID : " + kinectTestWPF1.App.deviceId.ToString();
        }
    }
}
