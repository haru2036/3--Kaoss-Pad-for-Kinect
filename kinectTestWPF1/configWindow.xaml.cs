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
using Midi;

namespace kinectTestWPF1
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            // 出力先のMIDIデバイス一覧を取得
            int deviceNum = OutputDevice.InstalledDevices.Count;
            string[] deviceList = new string[deviceNum];

            for (int i = 0; i < deviceNum; i++)
            {
                OutputDevice outputDevice = OutputDevice.InstalledDevices[i];
                deviceList[i] = outputDevice.Name;
            }

            for (int i = 0; i < deviceNum; i++)
            {
                listBox1.Items.Add(deviceList[i]);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("出力先のデバイスを選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            kinectTestWPF1.App.deviceId = listBox1.SelectedIndex;

            MainWindow mainWindow = new MainWindow();
            //DummyWindow mainWindow = new DummyWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
