using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Victim
{
    class VictimServer
    {
        private int _listenPort;
        public VictimServer(int listenPort) {
            _listenPort = listenPort;
        }
        public void serve() {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            socket.Listen(10);
            Socket client = socket.Accept();
            while (true)
            {
                handleClinet()
            }
        }
    }
}
