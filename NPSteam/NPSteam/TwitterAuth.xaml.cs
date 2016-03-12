using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Markup;
using TweetSharp;
using System.IO;

namespace NPSteam
{
    /// <summary>
    /// Interaction logic for TwitterAuth.xaml
    /// </summary>
    public partial class TwitterAuth : MahApps.Metro.Controls.MetroWindow
    {
        TwitterAuth Instance;
        public TwitterAuth()
        {
            Instance = this;
            InitializeComponent();
            
        }
        TextBox verifierField;
        private Label label;
        private Button submitButton;
        private Button cancelButton;
        private async void Authenticate()
        {
            var service = new TwitterService(Global.consumerKey, Global.consumerSecret);
            OAuthRequestToken requestTkn = await Task.Run(new Func<OAuthRequestToken>(service.GetRequestToken));
            Uri uri = service.GetAuthorizationUri(requestTkn);
            Process.Start(uri.ToString());
            DisplayInputField();
            return;
        }
        private void DisplayInputField()
        {
            Main_Layout.Children.Remove(progress_ring);
            UIElement root;
            FileStream s = new FileStream("VerifierLayout.xaml", FileMode.Open);
            root = (UIElement)XamlReader.Load(s);
            s.Close();
            Main_Layout.Children.Add(root);
          
        }

        private void AuthWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayInputField();
        }
    }
}
