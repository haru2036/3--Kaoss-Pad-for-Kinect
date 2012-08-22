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
using System.Reflection;
using Midi;

namespace kinectTestWPF1
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window
    {
        static int deviceId = kinectTestWPF1.App.deviceId;

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

            //kinectTestWPF1.App.deviceId = listBox1.SelectedIndex;

            MainWindow mainWindow = new MainWindow();
            //DummyWindow mainWindow = new DummyWindow();
            mainWindow.Show();
            this.Close();
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            kinectTestWPF1.App.isSeated = false;
        }

        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {
            kinectTestWPF1.App.isSeated = true;
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            groupBox2.IsEnabled = true;
            kinectTestWPF1.App.deviceId = listBox1.SelectedIndex;
        }

        private void button_xSend_Click(object sender, RoutedEventArgs e)
        {
            string channel = comboBox_xChannel.SelectionBoxItem.ToString();
            string control = comboBox_xMidiSignal.SelectionBoxItem.ToString();

            kinect2Midi midi1 = new kinect2Midi();
            midi1.sendX(channel, control);
            midi1.Dispose();
        }

        private void button_ySend_Click(object sender, RoutedEventArgs e)
        {
            string channel = comboBox_yChannel.SelectionBoxItem.ToString();
            string control = comboBox_yMidiSignal.SelectionBoxItem.ToString();

            kinect2Midi midi1 = new kinect2Midi();
            midi1.sendY(channel, control);
            midi1.Dispose();
        }

        private void button_zSend_Click(object sender, RoutedEventArgs e)
        {
            string channel = comboBox_zChannel.SelectionBoxItem.ToString();
            string control = comboBox_zMidiSignal.SelectionBoxItem.ToString();

            kinect2Midi midi1 = new kinect2Midi();
            midi1.sendZ(channel, control);
            midi1.Dispose();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
