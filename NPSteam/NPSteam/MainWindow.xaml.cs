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
        const string regexPattern = @"(?!-pid)(\d+)\w";
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

            string wmiQuery = string.Format("select CommandLine from Win32_Process where ProcessId = {0}", overlayApp.Id);
            var searcher = new ManagementObjectSearcher(wmiQuery);
            var retCollection = searcher.Get();
            foreach (var retObject in retCollection)
            {
                Global.Pid = Regex.Match(retObject["CommandLine"].ToString(), regexPattern).Value;
                Process steamApp = Process.GetProcessById(Convert.ToInt16(Global.Pid));

                Console.WriteLine("pid = {0}, game Name = {1}", Global.Pid, steamApp.MainWindowTitle);
                gameName = steamApp.MainWindowTitle;
                //Global.Service.sendt
            }
            retCollection.Dispose();
            searcher.Dispose();
            
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
            string tweetFormat = "Now Playing - " + gameName + " On Steam #NPSteam";
            var result = Global.Service.BeginSendTweet(new TweetSharp.SendTweetOptions { Status = tweetFormat });
        }
    }


}
