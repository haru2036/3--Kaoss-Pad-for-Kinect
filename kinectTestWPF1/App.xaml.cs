using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using kinectTestWPF1;
namespace kinectTestWPF1
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static int deviceId;
        public static bool isSeated;
        public static Midi.Channel noteChannel;
        protected override void OnExit(ExitEventArgs e)
        {
            
            base.OnExit(e);
        }
    }
}
