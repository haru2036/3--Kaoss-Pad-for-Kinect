using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Midi;
using System.Reflection;
using System.Windows;

namespace kinectTestWPF1
{
    class kinect2Midi : IDisposable
    {
        static int deviceId = kinectTestWPF1.App.deviceId;
        public OutputDevice outputDevice = OutputDevice.InstalledDevices[deviceId];
        int vx, vy, vz;

        public kinect2Midi()
        {
            outputDevice.Open();
        }

        //実際に使うのはこっち
        public void sendAll(float X, float Y, float Z)
        {
            vx = Math.Max(0,Math.Min(127, (int)(63.5 * (X + 1))));
            vy = Math.Max(0, Math.Min(127, (int)(63.5 * (Y + 1))));
            vz = Math.Max(0,Math.Min(127, (int)(127 * (Z-1))));
            int cutoff, type, reso;
            type = vz;
            cutoff = vx;
            reso = vy;

            outputDevice.SendControlChange(Channel.Channel1, Midi.Control.ModulationWheel, cutoff);
            outputDevice.SendControlChange(Channel.Channel2, Midi.Control.ModulationWheel, type);
            outputDevice.SendControlChange(Channel.Channel3, Midi.Control.ModulationWheel, reso);
        }
        
        //ここからの３つはMidi Learnで覚えさせるためのテスト信号
        public void sendX(string chstr, string ctrlstr)
        {
            midiTest(chstr, ctrlstr, 63);
        }

        public void sendY(string chstr, string ctrlstr)
        {
            midiTest(chstr, ctrlstr, 63);
        }

        public void sendZ(string chstr, string ctrlstr)
        {
            midiTest(chstr, ctrlstr, 63);
        }

        private void midiTest(string chstr, string ctrlstr, int value)
        {
            Object ctrlObj = new Midi.Control();
            Type ctrlType = ctrlObj.GetType();
            FieldInfo ctrlFld = ctrlType.GetField(ctrlstr);
            Midi.Control control = (Midi.Control)ctrlFld.GetValue(ctrlObj);

            Object chObj = new Channel();
            Type chType = chObj.GetType();
            FieldInfo chFld = chType.GetField(chstr);
            Channel channel = (Channel)chFld.GetValue(chObj);

            outputDevice.SendControlChange(channel, control, value);
        }
        
        public void Dispose()
        {
            outputDevice.Close();
        }

        /*
        ~kinect2Midi()
        {
            outputDevice.Close();
        }
        */
    }
}
