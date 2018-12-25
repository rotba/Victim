using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Victim
{
    class VictimServer
    {
        private int _listen_port;
        private string _password;
        private Queue<DateTime> _hacks;
        private readonly string please_enter_msg = "Please enter your password";
        private readonly string access_granted = "Access Granted";
        private readonly string newline_delimeter = "\r\n";
        private readonly int MAX_MSG_SIZE = 256;
        public VictimServer(int listen_port) {
            _listen_port = listen_port;
            _hacks = new Queue<DateTime>();
            _password = "159763";
        }
        public void serve() {
            Console.WriteLine($"Server listening on port {0}, password is {1}",_listen_port, 123456);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            socket.Listen(10);
            Socket client = socket.Accept();
            while (true)
            {
                Thread t = new Thread(() => handleClient(client));
            }
        }

        private void handleClient(Socket client)
        {
            Console.WriteLine("HEYY");
            while (client.Connected)
            {
                string response;
                send(client, "Please enter your password\r\n");
                response = receive(client);
                if (valid_password(response))
                {
                    send(client, "Access Granted");
                    response = receive(client);
                    handle_msg(response);
                }
                disconnect_client(client);
            }
        }

        private int send(Socket sock, string msg) {
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            sock.Send(buffer, 4, SocketFlags.None);
            return 1;
        }

        private string receive(Socket client)
        {
            byte[] rcv_buffer = new byte[MAX_MSG_SIZE];
            int readBytes = client.Receive(rcv_buffer, MAX_MSG_SIZE, SocketFlags.None);
            if (readBytes == 0)
            {
                //handleNoResponse(client);
            }
            return System.Text.Encoding.Default.GetString(rcv_buffer);
        }
        private bool valid_password(string password)
        {
            return password.Equals(_password);
        }
        private void handle_msg(string msg)
        {
            Regex rx = new Regex(@"(?<=Hecked in by\s*)\w+",
                RegexOptions.Compiled | RegexOptions.None);
            MatchCollection matches = rx.Matches(msg);
            if (matches.Count > 0) {
                addHack(matches[0].Groups[0].Value);
            }
            if (hacked()) {
                Console.WriteLine(msg);
                _hacks.Clear();
            }
        }

        private bool hacked()
        {
            if (_hacks.Count == 0) return false;
            int hacks_ps_count = 0;
            Queue<DateTime> tmp = new Queue<DateTime>();
            DateTime last = _hacks.Dequeue();
            while (!(_hacks.Count == 0)) {
                hacks_ps_count++;
                DateTime curr = _hacks.Dequeue();
                TimeSpan duration = last - curr;
                TimeSpan one_sec = new TimeSpan(0, 0, 1);
                if ((duration> one_sec) && (hacks_ps_count>=10)) {
                    _hacks.Clear();
                    return true;
                }
                tmp.Enqueue(curr);
            }
            while ((tmp.Count > 0)) {
                _hacks.Enqueue(tmp.Dequeue());
            }
            return false;
        }

        private void addHack(string value)
        {
            _hacks.Enqueue(DateTime.Now);
        }

        private void disconnect_client(Socket client)
        {
            client.Close();
        }

        private void sendPleaseEnterMsg(Socket client)
        {
            string msg = please_enter_msg + newline_delimeter;
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            client.Send(buffer, 4, SocketFlags.None);
        }
        
    }
}
