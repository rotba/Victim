using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Victim
{
    public class Program
    {
        static void Main(string[] args)
        {
            string pass = "";
            Random rand = new Random();
            for (int i = 0; i <6; i++) {
                pass += (char)('a'+rand.Next(0,26));
            }
            UdpClient uclient = new UdpClient(0);
            int random_port = ((IPEndPoint)uclient.Client.LocalEndPoint).Port;
            uclient.Close();
            String this_ip="";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    this_ip = ip.ToString();
                }
            }
            VictimServer vs = new VictimServer(random_port, pass, this_ip);
            vs.serve();
        }
    }
}
