using System.Diagnostics;
using System.Management;
using System;
using System.Text.RegularExpressions;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
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
        string gameName = null;
        public MainWindow()
        {
            InitializeComponent();


            //timer = new Timer(5000);        
            

        }

        void scanProcess()
        {
            var processList = Process.GetProcessesByName(appName);
            Process overlayApp = getSteamProcess(processList);


            if (overlayApp == null)
                return;


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
                            var directorArray = gameProcess["ExecutablePath"].ToString().Split('\\');


                            //get name of the game from directory instead of window title of the game
                            Console.WriteLine("directory : " + directorArray[directorArray.Length-2]);
                            gameName = directorArray[directorArray.Length - 2];
                        }

                    }


                    Console.WriteLine("pid = {0}, game Name = {1}", Global.Pid, steamApp.MainWindowTitle);
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
            string tweetFormat = "Now Playing - " + gameName + " on Steam #NPSteam";
            var result = Global.Service.BeginSendTweet(new TweetSharp.SendTweetOptions { Status = tweetFormat });
        }
    }


}
