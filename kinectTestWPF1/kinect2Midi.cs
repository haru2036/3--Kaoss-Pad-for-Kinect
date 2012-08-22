using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Midi;
namespace kinectTestWPF1
{
    class kinect2Midi
    {
        public static Pitch sendingPitch = Pitch.A0;
        bool isSending=false;
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
        public void sendX()
        {
            outputDevice.SendControlChange(Channel.Channel1, Midi.Control.ModulationWheel, 63);
        }
        
        public void sendY()
        {
            outputDevice.SendControlChange(Channel.Channel3, Midi.Control.ModulationWheel, 63);
        }
        
        public void sendZ()
        {
            outputDevice.SendControlChange(Channel.Channel2, Midi.Control.ModulationWheel, 63);
        }

        public void sendNoteOn(float position, int scale,Channel channel)
        {
            int note = get127(position);
            Pitch pitch =getPitch(position,scale);
            if (isSending==true)
            {
                if (sendingPitch != pitch)
                {
                    sendNoteOff(channel);
                    
                }
                
            }
            if (sendingPitch != pitch)
            {
                sendingPitch = pitch;
                isSending = true;
                outputDevice.SendNoteOn(channel, pitch, 64);
            }
            
        }
        public void sendNoteOff(Channel channel)
        {
         
            outputDevice.SendNoteOff(channel, sendingPitch, 64);
            isSending = false;
        }

        private Pitch getPitch(float position , int Scale)
        {
            Pitch pitch=Pitch.A0;
            int divide=(int)((position + 1 )* 5);
            switch (divide){
                case 0:
                    pitch =Pitch.A4;
                    break;
                case 1:
                    pitch =Pitch.B4;
                    break;
                case 2:
                    pitch =Pitch.CSharp4;
                    break;
                case 3:
                    pitch =Pitch.E4;
                    break;
                case 4:
                    pitch =Pitch.FSharp4;
                    break;
                case 5:
                    pitch = Pitch.GSharp4;
                    break;
                case 6:
                    pitch =Pitch.A5;
                    break;
                case 7:
                    pitch =Pitch.B5;
                    break;
                case 8:
                    pitch =Pitch.CSharp5;
                    break;
                case 9:
                    pitch =Pitch.E5;
                    break;
                case 10:
                    pitch =Pitch.GSharp5;
                    break;
                default:
                    break;
            }
            return pitch;
            
        }

        private int get127(float position)
        {
           return Math.Max(0, Math.Min(127, (int)(63.5 * (position + 1))));
        }

        ~kinect2Midi()
        {
            outputDevice.Close();
        }
    }
}
