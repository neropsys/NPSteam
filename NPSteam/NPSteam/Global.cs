using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace NPSteam
{
    class Global
    {
        public static string Pid;
        public static string CurrentGameName;
        public const string ConsumerKey = Token.consumerKey;
        public const string ConsumerSecret = Token.consumerSecret;
        public static TwitterService Service;
    }
}
