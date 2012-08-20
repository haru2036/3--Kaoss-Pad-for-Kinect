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
        KinectSensor kinect;
        kinect2Midi midi1;
        bool isNotConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            midi1 = new kinect2Midi();
            try
            {
                if ( KinectSensor.KinectSensors.Count == 0 ) {
                    isNotConnected = true;
                    throw new Exception( "Kinectが接続されていません" );
                }

                // Kinectインスタンスを取得する
                kinect = KinectSensor.KinectSensors[0];

                // すべてのフレーム更新通知をもらう
                kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>( kinect_AllFramesReady );

                // Color,Depth,Skeletonを有効にする
                kinect.ColorStream.Enable();
                kinect.DepthStream.Enable();
                kinect.SkeletonStream.Enable();

                // Kinectの動作を開始する
                kinect.Start();
            }
            catch ( Exception ex ) {
                MessageBox.Show( ex.Message );
                //Close();
            }
        }

        // すべてのデータの更新通知を受け取る
        void kinect_AllFramesReady( object sender, AllFramesReadyEventArgs e )
        {
            imageRgbCamera.Source = e.OpenColorImageFrame().ToBitmapSource();
            //imageDepthCamera.Source = e.OpenDepthImageFrame().ToBitmapSource();

            // 骨格位置の表示
            ShowSkeleton( e );
        }

        private void ShowSkeleton( AllFramesReadyEventArgs e )
        {
            // キャンバスをクリアする
            canvas1.Children.Clear();

            // スケルトンフレームを取得する
            SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();
            if ( skeletonFrame != null ) {
                // スケルトンデータを取得する
                Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo( skeletonData );

                // プレーヤーごとのスケルトンを描画する
                foreach ( var skeleton in skeletonData ) {
                    if ( skeleton.TrackingState == SkeletonTrackingState.Tracked ) {
                        // 骨格を描画する
                        foreach (Joint joint in skeleton.Joints)
                        {
                            if (joint.JointType == JointType.HandRight)
                            {
                                midi1.sendAll(joint.Position.X, joint.Position.Y, joint.Position.Z);
                                drawCircle(joint, Colors.Red, 5);
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
                    }
                }
            }
        }
       

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            midi1.sendX();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            midi1.sendY();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            midi1.sendZ();
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
                offsetX = (canvas1.ActualWidth- ((rgbHeight / 3) * 4)) / 2;
            }else{
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
    }
}

