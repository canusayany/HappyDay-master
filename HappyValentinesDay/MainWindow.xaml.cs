using Mindscape.WpfElements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Serialization;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace HappyValentinesDay
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
        }
     

        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    

    MediaPlayer mp = new MediaPlayer();
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Q)
                Close();
        }


        Task task;
        Class_Setting setting = new Class_Setting() { SnowflakeType = 3,SnowflakeCount=20,SnowflakeSize=45 ,WordColor=Colors.YellowGreen};
       
        UserControl userControl = new UserControl() ;
        void Start(Canvas panel)
        {
            // SerializSettingClass(setting);


            panel.Children.Clear();

              Random random = new Random();
            task = Task.Factory.StartNew(new Action(() =>
               {

                   for (int j = 0; j < 50; j++)
                   {
                       Thread.Sleep(j * 200);
                    //Dispatcher.BeginInvoke(new Action(() =>
                    //{
                    int snowCount = random.Next(2, setting.SnowflakeCount);//Count
                       for (int i = 0; i < snowCount; i++)
                       {

                           Dispatcher.BeginInvoke(new Action(() =>
                           {
                               int width = random.Next(5, setting.SnowflakeSize);//size

                               if (setting.SnowflakeType == 1)
                               {
                                   Petal pack = new Petal();
                                   userControl = pack;

                               }
                               else if (setting.SnowflakeType == 2)
                               {
                                   BGCFu bGCFu = new BGCFu();
                                   userControl = bGCFu;
                               }
                               else if (setting.SnowflakeType == 3)
                               {
                                   BGCSnow bGCSnow = new BGCSnow();
                                   userControl = bGCSnow;

                               }
                               else if (setting.SnowflakeType == 4)
                               {
                                   BGCStarxaml bGCStarxaml = new BGCStarxaml();
                                   userControl = bGCStarxaml;
                               }
                               else
                               {
                                   BGCSnow bGCFu = new BGCSnow();
                                   userControl = bGCFu;
                               }
                               userControl.Width = width;
                               userControl.Height = width;
                               userControl.RenderTransform = new RotateTransform();

                               int left = random.Next(0, (int)panel.ActualWidth);
                               Canvas.SetLeft(userControl, left);
                               panel.Children.Add(userControl);
                               int seconds = random.Next(20, 30);
                               userControl.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1)));
                               DoubleAnimationUsingPath doubleAnimation = new DoubleAnimationUsingPath()        //下降动画
                               {
                                   Duration = new Duration(new TimeSpan(0, 0, seconds)),
                                   RepeatBehavior = RepeatBehavior.Forever,
                                   PathGeometry = new PathGeometry(new List<PathFigure>() { new PathFigure(new Point(left, 0), new List<PathSegment>() { new LineSegment(new Point(left, panel.ActualHeight), false) }, false) }),
                                   Source = PathAnimationSource.Y
                               };
                               userControl.BeginAnimation(Canvas.TopProperty, doubleAnimation);
                               DoubleAnimation doubleAnimation1 = new DoubleAnimation(360, new Duration(new TimeSpan(0, 0, 12)))              //旋转动画
                               {
                                   RepeatBehavior = RepeatBehavior.Forever,
                               };

                               userControl.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation1);
                           }));
                       }

                       
                   }
               }));
          
        }
        private async void StartTextAsync(String[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Tb.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.2)));
                Tb.Text = data[i];
                if (i != data.Length - 1)
                {
                    await Task.Delay(3000);
                    Tb.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1.5)));
                    await Task.Delay(1500);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //SerializSettingClass(setting);
           
            setting = DeserializSettingClass();
            ColorFromSetting = setting.WordColor; 
            CPB.DataContext = this;
            CPB.Color = setting.WordColor;
            InitialTray();
            GetNewMusicFromList();
            //   mp.Open(new Uri(@"Music\Ina Wroldsen - I wanted you.mp3", UriKind.Relative));
            // mp.Play();
            //mp.MediaEnded += delegate { mp.Position = TimeSpan.FromSeconds(0); mp.Play(); };
            mp.MediaEnded += delegate { GetNewMusicFromList(); };
            Start(PetalBackground);
            String[] data;
            //String data = "我一直在等待你的出现^" +
            //"谢谢你的出现^" +
            //"此生不换^" +
            //"执子之手，与子偕老^" +
            //"携手到永远……";
            string path = AppDomain.CurrentDomain.BaseDirectory + "/Data/Word.txt";
            if (File.Exists(path))
            {
                data = File.ReadAllLines(path);
                StartTextAsync(data);
            }

        }
        Dictionary<int, FileInfo> keyValuePairsForMusicRandom = new Dictionary<int, FileInfo>();
        int musicCount = 0;
        void RefreshMusicDic()
        {
            int index = 0;
            if (System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Data/Music"))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "/Data/Music");
                FileInfo[] fileInfos = directoryInfo.GetFiles();
                if (fileInfos.Length == 0)
                {
                    return;

                }
                musicCount = fileInfos.Length;
                keyValuePairsForMusicRandom = new Dictionary<int, FileInfo>();
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    //Console.WriteLine(fileInfos[i].FullName);
                    keyValuePairsForMusicRandom.Add(index, fileInfos[i]);
                    index++;
                }
            }
        }
        List<int> HasPlayedSong = new List<int>();
        void GetNewMusicFromList()
        {
            RefreshMusicDic();
            Console.WriteLine($"musicCount={musicCount},HasPlayedSong.Count={HasPlayedSong.Count}");
            Random random = new Random();
            if (musicCount > 1)
            {
                int index = 0;
               
                if (HasPlayedSong.Count == musicCount)
                {
                    HasPlayedSong.Clear();
                    Console.WriteLine("所有已播放!");
                }
                do
                {
                    index = random.Next(0, musicCount);
                    Console.WriteLine($"筛选中本次选中为{index}");
                }
                while (HasPlayedSong.Contains(index));
                Console.WriteLine("选定歌曲"+index);
                HasPlayedSong.Add(index);
                mp.Open(new Uri(keyValuePairsForMusicRandom[index].FullName, UriKind.Absolute));
                notifyIcon.Text = keyValuePairsForMusicRandom[index].Name;
                mp.Position = TimeSpan.FromSeconds(0); mp.Play();
            }
            else
            {
                return;
            }


        }


        #region 系统状态栏
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private void InitialTray()
        {

            //设置托盘的各个属性
            notifyIcon = new System.Windows.Forms.NotifyIcon();

            // notifyIcon.BalloonTipText = "程序开始运行";
            notifyIcon.Text = "托盘图标";
            notifyIcon.Icon = new System.Drawing.Icon("Data/Touch Meee.ico");
            notifyIcon.Visible = true;
            //  notifyIcon.ShowBalloonTip(2000);
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);

            //设置菜单项
            #region 音乐控制
            System.Windows.Forms.MenuItem MI_MusicCtrl = new System.Windows.Forms.MenuItem("下一曲");
            MI_MusicCtrl.Click += Menu_Click;
            #endregion

            #region 背景控制
            System.Windows.Forms.MenuItem MI_BGCType1 = new System.Windows.Forms.MenuItem("飘落样式-爱");
            System.Windows.Forms.MenuItem MI_BGCType2 = new System.Windows.Forms.MenuItem("飘落样式-福");
            System.Windows.Forms.MenuItem MI_BGCType3 = new System.Windows.Forms.MenuItem("飘落样式-雪");
            System.Windows.Forms.MenuItem MI_BGCType4 = new System.Windows.Forms.MenuItem("飘落样式-星");
            System.Windows.Forms.MenuItem MI_BGCType = new System.Windows.Forms.MenuItem("飘落样式", new System.Windows.Forms.MenuItem[] { MI_BGCType1, MI_BGCType2, MI_BGCType3, MI_BGCType4 });
            MI_BGCType1.Click += MI_BGCType1_Click;
            MI_BGCType2.Click += MI_BGCType2_Click;
            MI_BGCType3.Click += MI_BGCType3_Click;
            MI_BGCType4.Click += MI_BGCType4_Click;
            System.Windows.Forms.MenuItem MI_BGCCtrl = new System.Windows.Forms.MenuItem("背景设置", new System.Windows.Forms.MenuItem[] { MI_BGCType });

            #endregion
            #region 文字控制
            //< ms:DropDownColorPicker HorizontalAlignment = "Left" Margin = "622,41,0,0" VerticalAlignment = "Top" Height = "103" Width = "108" />
           
            System.Windows.Forms.MenuItem MI_WordColor = new System.Windows.Forms.MenuItem("颜色设置");
            System.Windows.Forms.MenuItem MI_WordCtrl = new System.Windows.Forms.MenuItem("文字设置", new System.Windows.Forms.MenuItem[] { MI_WordColor });
            MI_WordColor.Click += MI_WordColor_Click;
            #endregion

            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("Exit");
            exit.Click += new EventHandler(exit_Click);


            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { MI_WordCtrl, MI_BGCCtrl, MI_MusicCtrl, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            //窗体状态改变时候触发
            this.StateChanged += new EventHandler(SysTray_StateChanged);
        }
      public   Color ColorFromSetting 
        {
            get { return setting.WordColor; }
            set
            {
               
               
                setting.WordColor = value;
            
                CPB.Visibility = Visibility.Collapsed;
                CPB.IsEnabled = false;
                //OnPropertyChanged("ColorFromSetting");
            }
        }
        private void MI_WordColor_Click(object sender, EventArgs e)
        {
            CPB.Visibility = Visibility.Visible;
            CPB.IsEnabled = true;
          
            // ColorPicker.change
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out Point pt);
        private void MI_BGCType3_Click(object sender, EventArgs e)
        {


           
            setting.SnowflakeType = 3;
            Start(PetalBackground);
           
            
        }
        private void MI_BGCType4_Click(object sender, EventArgs e)
        {



            setting.SnowflakeType = 4;
            Start(PetalBackground);


        }
        private void MI_BGCType2_Click(object sender, EventArgs e)
        {
          
            setting.SnowflakeType = 2;
            Start(PetalBackground);
           
        }

        private void MI_BGCType1_Click(object sender, EventArgs e)
        {
           
            setting.SnowflakeType = 1;
            Start(PetalBackground);
        }

        private void Menu_Click(object sender, EventArgs e)
        {
            GetNewMusicFromList();
        }

        ///
        /// 窗体状态改变时候触发
        ///
        ///

        ///

        private void SysTray_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// 退出选项
        ///
        ///

        ///

        private void exit_Click(object sender, EventArgs e)
        {
           
            notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
           
        }

       

        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.Activate();
                }
            }
        }
        #endregion
        #region 设置类序列化存储以及反序列化解析
        private void SerializSettingClass(Class_Setting setting)
        {
            FileStream stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Data/Setting.ini", FileMode.Create);
            XmlSerializer xmlserilize = new XmlSerializer(typeof(Class_Setting));
            xmlserilize.Serialize(stream, setting);
            stream.Close();
        }
        private Class_Setting DeserializSettingClass()
        {
            Class_Setting setting = new Class_Setting();
            FileStream stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Data/Setting.ini", FileMode.Open);
            XmlSerializer xmlserilize = new XmlSerializer(typeof(Class_Setting));
          
            setting = (Class_Setting)xmlserilize.Deserialize(stream);
            stream.Close();

            
           
            return setting;
        }
        #endregion
        private void MainWindow__Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SerializSettingClass(setting);
            notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        private void ColorPicker_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {

        }
    }

   
}
