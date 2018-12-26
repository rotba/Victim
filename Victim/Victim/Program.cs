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
            VictimServer vs = new VictimServer(2019,"passssap", "192.168.0.26");
            vs.serve();
        }
    }
}
