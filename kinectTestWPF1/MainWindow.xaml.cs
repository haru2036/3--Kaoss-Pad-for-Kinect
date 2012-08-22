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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using Midi;
using System.Diagnostics;


namespace kinectTestWPF1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        IEnumerable<Skeleton> rawSkeleton;
        Skeleton skeleton= new Skeleton();
        float flicker = 1;
        KinectSensor kinect;
        kinect2Midi midi1;
        bool isNotConnected = false;
        double angle = 0;

        public MainWindow()
        {
            InitializeComponent();
            midi1 = new kinect2Midi();
            try
            {
                if (KinectSensor.KinectSensors.Count == 0)
                {
                    isNotConnected = true;
                    throw new Exception("Kinectが接続されていません");
                }

                // Kinectインスタンスを取得する
                kinect = KinectSensor.KinectSensors[0];

                //Seated modeの設定を反映する
                if (kinectTestWPF1.App.isSeated == true)
                {
                    kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }

                // すべてのフレーム更新通知をもらう
                kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);

                // Color,Depth,Skeletonを有効にする
                kinect.ColorStream.Enable();
                kinect.DepthStream.Enable();
                kinect.SkeletonStream.Enable();

                // Kinectの動作を開始する
                kinect.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //Close();
            }
        }

        // すべてのデータの更新通知を受け取る
        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            imageRgbCamera.Source = e.OpenColorImageFrame().ToBitmapSource();
            //imageDepthCamera.Source = e.OpenDepthImageFrame().ToBitmapSource();

            // 骨格位置の表示
            ShowSkeleton(e);
        }

        private void ShowSkeleton(AllFramesReadyEventArgs e)
        {
            // キャンバスをクリアする
            canvas1.Children.Clear();

            // スケルトンフレームを取得する
            SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();
            if (skeletonFrame != null)
            {
                // スケルトンデータを取得する
                Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletonData);

                // プレーヤーごとのスケルトンを描画する
                switch (skeletonMax(skeletonData))
                {
                    case 0:
                        drawString("画面の中に入ってください！");
                        if (flicker <= 0)
                    {
                            flicker = flicker + (float)0.05;
                        }
                        else if (flicker >= 1)
                        {
                            flicker = flicker - (float)0.05;
                        }
                        break;
                    case 1:
                        skeleton = getSkeleton(skeletonData).First();
                        foreach (Joint joint in skeleton.Joints)
                        {
                            if (joint.JointType == JointType.HandLeft)
                            {

                                if (Math.Max(0, Math.Min(127, (int)(127 * (joint.Position.Z - 1)))) >= 63)
                                {
                                    //drawCoordinate(joint, Colors.Pink);
                                    midi1.sendNoteOff(kinectTestWPF1.App.noteChannel);
                                    kinectTestWPF1.kinect2Midi.sendingPitch = Pitch.A0;
                                }
                                else
                                {
                                    drawLine(get10(joint.Position.Y));
                                    //drawCoordinate(joint, Colors.Aqua);
                                    midi1.sendNoteOn(joint.Position.Y, 0, kinectTestWPF1.App.noteChannel);
                                }
                            }
                            if (joint.JointType == JointType.HandRight)
                            {
                                midi1.sendAll(joint.Position.X, joint.Position.Y, joint.Position.Z);
                                drawCircle(joint, Colors.Red, 5);

                                if (Math.Max(0, Math.Min(127, (int)(127 * (joint.Position.Z - 1)))) <= 63)
                                {
                                    //drawCoordinate(joint, Colors.Aqua);
                                    drawImage(joint, "images/blue.png");
                                }
                                else
                                {
                                    //drawCoordinate(joint, Colors.Pink);
                                    drawImage(joint, "images/pink.png");
                                }
                            }
                            else if (joint.JointType == JointType.Head)
                            {
                                drawCircle(joint, Colors.Aqua, 5);
                            }
                            else
                            {
                                drawCircle(joint, Colors.Blue, 5);
                            }
                        }

                        break;
                    case 2:
                        skeleton = getSkeleton(skeletonData).First();
                        
                        foreach (Joint joint in skeleton.Joints)
                        {
                            if (joint.JointType == JointType.HandRight)
                            {
                                midi1.sendAll(joint.Position.X, joint.Position.Y, joint.Position.Z);
                                drawCircle(joint, Colors.Red, 5);
                                if (Math.Max(0, Math.Min(127, (int)(127 * (joint.Position.Z - 1)))) <= 63)
                                {
                                    //drawCoordinate(joint, Colors.Aqua);
                    }
                                else
                                {
                                    //drawCoordinate(joint, Colors.Pink);
                    }
                            }
                        }
                        skeleton = getSkeleton(skeletonData).Skip(1).First();
                        
                        foreach (Joint joint in skeleton.Joints)
                        {
                            if (joint.JointType == JointType.HandLeft)
                            {

                                if (Math.Max(0, Math.Min(127, (int)(127 * (joint.Position.Z - 1)))) >= 63)
                                {

                                    //drawCoordinate(joint, Colors.Pink);
                                    midi1.sendNoteOff(kinectTestWPF1.App.noteChannel);
                                    kinectTestWPF1.kinect2Midi.sendingPitch = Pitch.A0;
                                }
                                else
                                {
                                    drawLine(get10(joint.Position.Y));
                                    //drawCoordinate(joint, Colors.Aqua);
                                    midi1.sendNoteOn(joint.Position.Y, 0, kinectTestWPF1.App.noteChannel);
                                }
                            }
                        }

                        break;
                    default:
                        break;
                }
            }
        }



        private void drawCircle(Joint joint, Color color, int cir)
        {
            // 骨格の座標をカラー座標に変換する
            ColorImagePoint point = kinect.MapSkeletonPointToColor(joint.Position, kinect.ColorStream.Format);
            double[] multiMargin1;
            multiMargin1 = getMargin();
            // 円を書く
            canvas1.Children.Add(new Ellipse()
            {
                Margin = new Thickness(multiMargin1[2] + multiMargin1[0] * point.X, multiMargin1[3] + multiMargin1[1] * point.Y, 0, 0),
                Fill = new SolidColorBrush(color),
                Width = cir,
                Height = cir,
            });
        }

        private double[] getMargin()
        {
            double rgbHeight, rgbWidth;
            double offsetX, offsetY;
            offsetX = 0;
            offsetY = 0;
            rgbHeight = imageRgbCamera.ActualHeight;
            rgbWidth = imageRgbCamera.ActualWidth;
            double rgbMin = Math.Min(rgbHeight, rgbWidth);
            double multiWidth = rgbWidth / 640;
            double multiHeight = rgbHeight / 480;
            double[] multiAndOffset;
            multiAndOffset = new double[4]; //0:横方向の倍率,1:縦方向の倍率,2:横方向の余白(左側のみ),3:縦方向の余白(上がわのみ)
            multiAndOffset[0] = multiWidth;
            multiAndOffset[1] = multiHeight;
            if (rgbWidth > rgbHeight)//横に余白がついている場合
            {
                offsetX = (canvas1.ActualWidth - ((rgbHeight / 3) * 4)) / 2;
            }
            else
            {
                offsetY = (canvas1.ActualHeight - ((rgbWidth / 4) * 3)) / 2;
            }
            multiAndOffset[2] = (offsetX);
            multiAndOffset[3] = (offsetY);
            return multiAndOffset;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isNotConnected == true)
            {
                Close();
            }
        }
        private void drawCoordinate(Joint joint, Color col)
        {
            Label text1 = new Label();
            text1.Content = "(" + joint.Position.X.ToString() + "," + joint.Position.Y.ToString() + "," + joint.Position.Z.ToString() + ")";

            ColorImagePoint point = kinect.MapSkeletonPointToColor(joint.Position, kinect.ColorStream.Format);
            double[] multiMargin1 = getMargin();
            text1.Margin = new Thickness(multiMargin1[2] + multiMargin1[0] * point.X, multiMargin1[3] + multiMargin1[1] * point.Y, 0, 0);
            text1.FontSize = 30;
            text1.Foreground = new SolidColorBrush(col);
            System.Windows.Media.Effects.DropShadowEffect dshadow = new System.Windows.Media.Effects.DropShadowEffect();
            text1.Effect = dshadow;
            text1.FontFamily = new System.Windows.Media.FontFamily("Agency FB");
            canvas1.Children.Add(text1);
        }
        public void drawString(string str)
        {
            Label label = new Label();
            label.Content = str;
            label.Foreground = new SolidColorBrush(Colors.Red);
            label.Opacity = flicker;
            label.FontSize = 30;
            label.FontFamily = new System.Windows.Media.FontFamily("メイリオ");
            label.Effect = new System.Windows.Media.Effects.DropShadowEffect();
            canvas1.Children.Add(label);
        }
        private int get10(float position)
        {
            return (int)((position + 1) * 5);
        }
        private void drawLine(int position)
        {
            double[] offsets = getMargin();
            float segmentSize = (float)imageRgbCamera.ActualHeight / 10;
            float linePosition = (float)imageRgbCamera.ActualHeight - (position * segmentSize);
            Line line = new Line();
            line.StrokeThickness = segmentSize;
            line.Stroke = new SolidColorBrush(Colors.Aqua);
            line.Y1 = linePosition;
            line.Y2 = linePosition;
            line.X1 = offsets[3] * offsets[1];
            line.X2 = offsets[3] + imageRgbCamera.ActualWidth * offsets[1];
            line.Effect = new System.Windows.Media.Effects.DropShadowEffect();
            line.Opacity = 0.3;
            canvas1.Children.Add(line);
        }
        private int skeletonMax(Skeleton[] data)
        {
            int max =
            (from TrackingState in data
             where TrackingState.TrackingState == SkeletonTrackingState.Tracked
             select TrackingState).Count();
            return max;
        }
        private IEnumerable< Skeleton> getSkeleton(Skeleton[] data)
        {
            IEnumerable<Skeleton> skels=(from skel in data
                    where skel.TrackingId >0
                    select skel);
            return skels;
       }
        private void drawImage(Joint joint, string file)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(file, UriKind.Relative));
            
            img.Opacity = 0.8;
            img.Width = 100;
            img.Height = 100;

            Matrix imgMatrix = ((MatrixTransform)img.RenderTransform).Matrix;
            imgMatrix.RotateAt(angle, img.Width / 2, img.Height / 2);
            img.RenderTransform = new MatrixTransform(imgMatrix);

            if (angle < 360)
            {
                angle += 10;
            }
            else
            {
                angle = 0;
            }

            ColorImagePoint point = kinect.MapSkeletonPointToColor(joint.Position, kinect.ColorStream.Format);
            double[] multiMargin1 = getMargin();
            img.Margin = new Thickness((multiMargin1[2] + multiMargin1[0] * point.X) - img.Width / 2 , 
                (multiMargin1[3] + multiMargin1[1] * point.Y) - img.Height / 2, 0, 0);

            canvas1.Children.Add(img);
        }
    }
}

