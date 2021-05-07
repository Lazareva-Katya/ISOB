using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lab3
{
    class Hack2
    {
        public static Task Hack = new Task(Run);
        public static int count = 10;
        private static void Run()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                socket.Bind(new IPEndPoint(IPAddress.Parse(Config.host), 6661));
                for (int i = 0; i < count; i++)
                {
                    TcpSegment tcp = new TcpSegment(6661 + i, Config.ServerListenPort, syn: true);
                    socket.SendTo(tcp.ToByteArray(), new IPEndPoint(IPAddress.Parse(Config.host), Config.ServerListenPort));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                socket.Close();
            }
        }
    }
}
