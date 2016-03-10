using System.Diagnostics;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
//(?!-pid)(\d+)\w
//regex for parsing pid

namespace NPSteam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string appName = "GameOverlayUI";
        const string regexPattern = @"(?!-pid)(\d+)\w";
        public MainWindow()
        {
            InitializeComponent();
            var processList = Process.GetProcessesByName(appName);

            string pid = null;
            var time = Stopwatch.StartNew();
            
            Process overlayApp = getSteamProcess(processList);
            string wmiQuery = string.Format("select CommandLine from Win32_Process where ProcessId = {0}", overlayApp.Id);
            var searcher = new ManagementObjectSearcher(wmiQuery);
            var retCollection = searcher.Get();
            foreach(var retObject in retCollection)
            {
                pid = Regex.Match(retObject["CommandLine"].ToString(), regexPattern).Value;
                Console.WriteLine("pid = {0}", pid);
            }
            Process steamApp = Process.GetProcessById(Convert.ToInt16(pid));
            time.Stop();
            Debug.WriteLine(time.ElapsedMilliseconds);

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
    }


}
