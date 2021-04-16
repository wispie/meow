using Grimoire.Networking;
using Grimoire.UI;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Grimoire
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(defaultValue: false);
            RootHolder Instance = new RootHolder();
            Application.Run(Instance);
        }
        
        public static bool FindAvailablePort(out int port)
        {
            Random random = new Random();
            IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] activeTcpConnections;
            IPEndPoint[] activeTcpListeners;
            try
            {
                activeTcpConnections = iPGlobalProperties.GetActiveTcpConnections();
                activeTcpListeners = iPGlobalProperties.GetActiveTcpListeners();
            }
            catch (NetworkInformationException)
            {
                port = 0;
                return false;
            }
            int randPort;
            TcpConnectionInformation tcpConnectionInformation;
            IPEndPoint iPEndPoint;
            do
            {
                randPort = random.Next(1001, 65535);
                tcpConnectionInformation = activeTcpConnections.FirstOrDefault((TcpConnectionInformation c) => c.LocalEndPoint.Port == randPort);
                iPEndPoint = activeTcpListeners.FirstOrDefault((IPEndPoint l) => l.Port == randPort);
            }
            while (tcpConnectionInformation != null || iPEndPoint != null);
            port = randPort;
            return true;
        }
    }
}