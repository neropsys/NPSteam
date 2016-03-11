using System;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using TweetSharp;
namespace NPSteam
{
    /// <summary>
    /// Interaction logic for TwitterAuth.xaml
    /// </summary>
    public partial class TwitterAuth
    {
        public TwitterAuth()
        {
            InitializeComponent();
            
        }
        TextBox verifierField;
        private bool Authenticate()
        {
            var service = new TwitterService(Global.consumerKey, Global.consumerSecret);
            var requestTkn = service.GetRequestToken();
            Uri uri = service.GetAuthorizationUri(requestTkn);


            return false;
        }
        private void DisplayInputField()
        {
           // Main_Layout.Children.Remove(progress_ring);
            verifierField = new TextBox();
            verifierField.AcceptsReturn = false;
            
        }
    }
}
