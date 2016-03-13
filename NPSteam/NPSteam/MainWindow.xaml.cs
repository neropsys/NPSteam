using System.Diagnostics;
using System.Management;
using System;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
//(?!-pid)(\d+)\w
//regex for parsing pid

namespace NPSteam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        const string appName = "GameOverlayUI";
        const string pidRegex = @"(?!-pid)(\d+)\w";
        const string overlaydPidQuery = "select CommandLine from Win32_Process where ProcessId = {0}";
        const string gameDirQueryString = "select ExecutablePath from Win32_Process where ProcessId = {0}";
        string exeDir = null;
        public MainWindow()
        {
            InitializeComponent();


            //timer = new Timer(5000);        
            

        }

        void scanProcess()
        {
            var processList = Process.GetProcessesByName(appName);
            Process overlayApp = getSteamProcess(processList);

            //if overlayapp is none, game is not being played
            if (overlayApp == null)
            {
                return;
            }


            string wmiQuery = string.Format(overlaydPidQuery, overlayApp.Id);
            using (var searcher = new ManagementObjectSearcher(wmiQuery))
            using (var retCollection = searcher.Get())
            {
                foreach (var retObject in retCollection)
                {
                    //get game pid
                    Global.Pid = Regex.Match(retObject["CommandLine"].ToString(), pidRegex).Value;
                    var steamApp = Process.GetProcessById(Convert.ToInt16(Global.Pid));

                    string gameDirQuery = string.Format(gameDirQueryString, steamApp.Id);

                    //get game title from common directory in steamapps
                    using (var gameSearcher = new ManagementObjectSearcher(gameDirQuery))
                    using (var retValue = gameSearcher.Get())
                    {
                        foreach(var gameProcess in retValue)
                        {
                            Console.WriteLine("exe directory : " + gameProcess["ExecutablePath"].ToString());
                            exeDir = gameProcess["ExecutablePath"].ToString();
                            var directorArray = gameProcess["ExecutablePath"].ToString().Split('\\');


                            //get name of the game from directory instead of window title of the game
                            Console.WriteLine("directory : " + directorArray[directorArray.Length-2]);
                            Global.CurrentGameName = directorArray[directorArray.Length - 2];
                        }

                    }
                    var icon = System.Drawing.Icon.ExtractAssociatedIcon(exeDir);
                    ImageSource iconImage = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    GameIcon.Source = iconImage;
                    GameName.Text = Global.CurrentGameName;
                        
                }
            }
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

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var screenArea = SystemParameters.WorkArea;
            Left = screenArea.Right - Width;
            Top = screenArea.Bottom - Height;
            scanProcess();
            string tweetFormat = "Now Playing - " + CurrentGameName + " on Steam #NPSteam";
            var result = Global.Service.BeginSendTweet(new TweetSharp.SendTweetOptions { Status = tweetFormat });
        }
    }


}
