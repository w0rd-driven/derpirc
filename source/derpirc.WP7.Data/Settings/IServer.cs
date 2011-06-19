using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace derpirc.Data.Settings
{
    public class IServer
    {
        public int NetworkId { get; set; }
        public string DisplayName { get; set; }
        public string HostName { get; set; }
        public string Ports { get; set; }
        public int Port { get; set; }
        public string Group { get; set; }
        public string Password { get; set; }
    }
}
