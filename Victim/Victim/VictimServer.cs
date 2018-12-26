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
    public class VictimServer
    {
        private int _listen_port;
        private string _ip;
        private string _password;
        private Queue<DateTime> _hacks;
        private readonly string please_enter_msg = "Please enter your password";
        private readonly string access_granted = "Access Granted";
        private readonly string newline_delimeter = "\r\n";
        private readonly int MAX_MSG_SIZE = 256;
        private readonly object syncLock = new object();

        public VictimServer(int listen_port, string password, string ip) {
            _listen_port = listen_port;
            _hacks = new Queue<DateTime>();
            _password = password;
            _ip = ip;
        }
        public void serve() {
            Console.WriteLine(String.Format("Server listening on port {0}, password is {1}",_listen_port, _password));
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _listen_port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            socket.Listen(10);
            while (true)
            {
                Socket client = socket.Accept();
                Thread t = new Thread(() => handleClient(client));
                t.Start();
            }
        }

        private void handleClient(Socket client)
        {;
            while (client.Connected)
            {
                string response;
                send(client, "Please enter your password\r\n");
                response = receive(client);
                if (valid_password(response))
                {
                    send(client, "Access Granted\r\n");
                    response = receive(client);
                    handle_msg(response);
                }
                disconnect_client(client);
            }
        }

        private int send(Socket sock, string msg) {
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            sock.Send(buffer, msg.Length, SocketFlags.None);
            return 1;
        }

        private string receive(Socket client)
        {
            string ans = "";
            byte[] rcv_buffer = new byte[MAX_MSG_SIZE];
            int offset = 0;
            int readBytes = client.Receive(rcv_buffer, MAX_MSG_SIZE, SocketFlags.None);
            while (!reached_message_end(rcv_buffer, offset, readBytes)) {
                byte[] tmp = { rcv_buffer[offset] };
                ans += Encoding.ASCII.GetString(tmp);
                offset++;
            }
            if (offset == 0)
            {
                //handleNoResponse(client);
            }
            return ans;
        }

        public bool valid_password(string password)
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
            lock (syncLock) {
                if (hacked())
                {
                    Console.WriteLine(msg);
                    _hacks.Clear();
                }
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
        private bool reached_message_end(byte[] rcv_buffer, int offset, int readBytes)
        {
            if (offset < readBytes - 1 && offset < rcv_buffer.Length-1)
            {
                byte[] maybe_delimeter = { rcv_buffer[offset], rcv_buffer[offset+1] };
                return Encoding.ASCII.GetString(maybe_delimeter).Equals(newline_delimeter);
            }
            else {
                return false;
            }
        }
        private void addHack(string value)
        {
            _hacks.Enqueue(DateTime.Now);
        }

        private void disconnect_client(Socket client)
        {
            client.Close();
        }
        
    }
}
