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
    class Program
    {
        static void Main(string[] args)
        {
            //VictimServer vs = new VictimServer(8000);
            //IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("132.73.209.153"), 8000);
            //Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            //socket.Bind(localEndPoint);
            //socket.Listen(10);
            //Socket client = socket.Accept();
            //while (client.Connected)
            //{
            //    //Read the command's Type.
            //    byte[] buffer = new byte[4];
            //    int readBytes = client.Receive(buffer, 4, SocketFlags.None);
            //    if (readBytes == 0)
            //        break;
            //    Console.WriteLine(System.Text.Encoding.Default.GetString(buffer));
            //}
            Thread t = new Thread(() => foo(3));
            t.Start();
        }
        static void foo(int a) {
            Console.WriteLine(a);
        }
    }
}
