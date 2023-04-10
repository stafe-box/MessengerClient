using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MessengerClient
{
    public static class CurrentUser
    {
        public static string? Name;
        public static Color UserColor = Colors.Black;
        public static string ServerIP = "255.255.255.255";
        public static bool IsLoggedIn = false;
        public static List<string> Users = new List<string>();
    }
}
