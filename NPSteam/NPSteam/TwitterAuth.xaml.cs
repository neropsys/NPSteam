using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using System.Windows.Markup;
using System.Windows.Input;
using TweetSharp;
using System.IO;
using MahApps.Metro.Controls.Dialogs;
namespace NPSteam
{
    /// <summary>
    /// Interaction logic for TwitterAuth.xaml
    /// </summary>
    public partial class TwitterAuth
    {
        public TwitterAuth()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            
        }
        Button submitBtn, cancelBtn;
        TextBox verifierBox;

        OAuthRequestToken requestTkn;
        UIElement verifierLayout;
        MainWindow mainWindow;

        private async void Authenticate()
        {
            Global.Service = new TwitterService(Global.ConsumerKey, Global.ConsumerSecret);
            requestTkn = await Task.Run(new Func<OAuthRequestToken>(Global.Service.GetRequestToken));
            Uri uri = Global.Service.GetAuthorizationUri(requestTkn);
            Process.Start(uri.ToString());
            DisplayInputField();
            return;
        }
        private void DisplayInputField()
        {
            Main_Layout.Children.Remove(progress_ring);
            FileStream s = new FileStream("VerifierLayout.xaml", FileMode.Open);
            verifierLayout = (UIElement)XamlReader.Load(s);
            s.Close();
            Main_Layout.Children.Add(verifierLayout);
            submitBtn = LogicalTreeHelper.FindLogicalNode(Window.GetWindow(this), "SubmitBtn") as Button;
            cancelBtn = LogicalTreeHelper.FindLogicalNode(Window.GetWindow(this), "CancelBtn") as Button;
            verifierBox = LogicalTreeHelper.FindLogicalNode(Window.GetWindow(this), "VerifierBox") as TextBox;
            submitBtn.Click += SubmitBtn_MouseLeftButtonDown;
            cancelBtn.Click += CancelBtn_MouseLeftButtonDown;
            verifierBox.KeyDown += (object sender, KeyEventArgs e)=>{
                if(e.Key == Key.Enter)
                {
                    CheckKey();
                }
            };


        }

    

        private async void CheckKey()
        {
            string pinCode = verifierBox.Text;
            bool authSuccess = false;
            Main_Layout.Children.Remove(verifierLayout);
            Main_Layout.Children.Add(progress_ring);
           
            
            await Task.Run(() =>
            {
                OAuthAccessToken accessTkn = Global.Service.GetAccessToken(requestTkn, pinCode);
                Global.Service.AuthenticateWith(accessTkn.Token, accessTkn.TokenSecret);
                if (Global.Service.Response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    authSuccess = false;
                }
                else
                {
                    authSuccess = true;
                    Global.Instance.ReadTitle_ini();
                }
            });
            if(authSuccess == true)
            {

                mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
                //show mainform
            }
            else
            {
                var dialogue = this.ShowMessageAsync("Failed!", ":(");
                await dialogue;
                if(dialogue.Result == MessageDialogResult.Affirmative)
                {
                    Application.Current.Shutdown();
                }
            }
        }
       
        private void CancelBtn_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SubmitBtn_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            CheckKey();
        }

        private void AuthWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            Authenticate();
        }
    }
}
