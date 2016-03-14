using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;
using System.IO;
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
        private static FileStream m_fstream;
        private static string titleFilePath;
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
        private Global()
        {
            if (File.Exists(titleFilePath)== false)
                m_fstream = new FileStream("NPSteam_Titles.ini", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            titleFilePath = Path.Combine("NPSteam_Titles.ini", System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        public void ReadTitle_ini()
        {
            // TODO read NPSteam_Title
        }
        // TODO method for saving NPSteam_Title.ini
        public void CloseTitle_ini(){
            m_fstream.Close();
        }
    }
}
