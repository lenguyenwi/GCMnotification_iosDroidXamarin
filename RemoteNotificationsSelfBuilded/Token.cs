using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RemoteNotificationsSelfBuilded
{
    public class Token
    {
        private static string testString = "hi this is test String in Token";

        public static string TestString
        {
            get
            {
                return testString;
            }

            set
            {
                testString = value;
            }
        }
        public static void addAnTestStringUsingMethode()
        {
            TestString = "hi this is StringAdded using methode"; 
        }

        //public static List<string> stringtokenList;
        //static string tokenString = "";
        ////public <string> TokenString { get { return this.stringtokenList; } set { this.stringtokenList = value; } }
        ////ctrl R E to generate getter setter
        //public Token()
        //{
        //}
        //public void  addstringToList(string token)
        //{
        //    StringtokenList.Add(token);
        //}
        //public static string getTokenStringformTokenObject()
        //{
        //    return tokenString;
        //}
        //public static string TokenString
        //{
        //    get
        //    {
        //        return tokenString;
        //    }

        //    set
        //    {
        //        tokenString = value;
        //    }
        //}

        //public List<string> StringtokenList
        //{
        //    get
        //    {
        //        return stringtokenList;
        //    }

        //    set
        //    {
        //        stringtokenList = value;
        //    }
        //}
    }
}