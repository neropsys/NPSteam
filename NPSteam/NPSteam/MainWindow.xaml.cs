using System.Diagnostics;
using System.Management;
using System;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Drawing;
using MahApps.Metro.Controls;
//(?!-pid)(\d+)\w
//regex for parsing pid

namespace NPSteam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        const string playingGameLabel = "You are Playing";
        const string waitingGameLabel = "Finding Game";
        const string appName = "GameOverlayUI";
        const string pidRegex = @"(?!-pid)(\d+)\w";
        const string gameDirRegex = @"(?<=\\common\\)(.*)(?=\\)";
        const string overlaydPidQuery = "select CommandLine from Win32_Process where ProcessId = {0}";
        const string gameDirQueryString = "select ExecutablePath from Win32_Process where ProcessId = {0}";
        string exeDir = null;
        string currentPid = null;
        Icon gameIcon = null;
        

        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();

            //timer = new Timer(5000);        
            

        }

        void scanProcess(object sender, EventArgs e)
        {
            var processList = Process.GetProcessesByName(appName);
            Process overlayApp = getSteamProcess(processList);

            //if overlayapp is none, game is not being played
            if (overlayApp == null)
            {
                setLayout(null, null);
                return;
            }


            string wmiQuery = string.Format(overlaydPidQuery, overlayApp.Id);
            using (var searcher = new ManagementObjectSearcher(wmiQuery))
            using (var retCollection = searcher.Get())
            {
                foreach (var retObject in retCollection)
                {
                    //get game pid
                    currentPid = Regex.Match(retObject["CommandLine"].ToString(), pidRegex).Value;

                    //exit if current Pid is same as before
                    if (currentPid == Global.Pid)
                        return;
                    else
                        Global.Pid = currentPid;

                    var steamApp = Process.GetProcessById(Convert.ToInt16(Global.Pid));

                    string gameDirQuery = string.Format(gameDirQueryString, steamApp.Id);

                    //get game title from common directory in steam apps
                    using (var gameSearcher = new ManagementObjectSearcher(gameDirQuery))
                    using (var retValue = gameSearcher.Get())
                    {
                        foreach(var gameProcess in retValue)
                        {
                            Console.WriteLine("exe directory : " + gameProcess["ExecutablePath"].ToString());
                            exeDir = gameProcess["ExecutablePath"].ToString();
                            var gameFolderName = Regex.Match(exeDir, gameDirRegex).Value;


                            Console.WriteLine("directory : " + gameFolderName);
                            Global.CurrentGameName = gameFolderName;

                        }

                    }
                    gameIcon = System.Drawing.Icon.ExtractAssociatedIcon(exeDir);
                    ImageSource iconImage = Imaging.CreateBitmapSourceFromHIcon(gameIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    setLayout(Global.CurrentGameName, iconImage);
                        
                }
            }
            SendTweet();
            
        }

        void SendTweet()
        {
            string tweetFormat = "Now Playing - " + Global.CurrentGameName + " on Steam #NPSteam";
            var result = Global.Service.BeginSendTweet(new TweetSharp.SendTweetOptions { Status = tweetFormat });
        }
        Process getSteamProcess(Process[] processList)
        {
            foreach (Process process in processList)
            {
                Debug.WriteLine(process.ProcessName);
                if (process.ProcessName == appName)
                {
                    return process;
                }
            }
            return null;
        }
        void setLayout(string gameName, ImageSource iconImage)
        {
            if(gameName == null && iconImage == null)
            {
                progress_ring.IsActive = true;
                GameIcon.Visibility = Visibility.Hidden;
                StatusLabel.Content = waitingGameLabel;
                GameName.Text = "";
                return;
            }
            progress_ring.IsActive = false;
            GameIcon.Visibility = Visibility.Visible;
            StatusLabel.Content = playingGameLabel;
            GameIcon.Source = iconImage;
            GameName.Text = gameName;
            return;
        }
        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var screenArea = SystemParameters.WorkArea;
            Left = screenArea.Right - Width;
            Top = screenArea.Bottom - Height;

            //init layout
            setLayout(null, null);


            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += new EventHandler(scanProcess);
            timer.Start();
            
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
            // TODO location for saving ini file from Global
            Global.Instance.CloseTitle_ini();
        }
    }


}
