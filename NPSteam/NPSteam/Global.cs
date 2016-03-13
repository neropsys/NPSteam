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
        private static Global m_instance;
        public static Global Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new Global();
                }
                return m_instance;
            }
        }

        //NPSteam_Titles.ini format
        //(User defined game name)=(game name from directory)


        // TODO Load NPSteam_Titles.ini
        static Global()
        {

        }
        // TODO method for saving NPSteam_Title.ini
    }
}
