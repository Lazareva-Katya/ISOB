using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lab3
{
    class Hack1
    {
        public static Task Hack = new Task(Run);
        public static void Run()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {

                TcpSegment tcp = new TcpSegment(6661, Config.ServerListenPort, syn: true);
                socket.Bind(new IPEndPoint(IPAddress.Parse(Config.host), 6661));

                for (int i = Config.ServerListenPort - 10; i <= Config.ServerListenPort; i++)
                {
                    var point = new IPEndPoint(IPAddress.Parse(Config.host), Config.ServerListenPort);

                    tcp.destinationPort = i;
                    point.Port = i;
                    socket.SendTo(tcp.ToByteArray(), point);
                }
                socket.Close();

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(new IPEndPoint(IPAddress.Parse(Config.host), 6661));

                byte[] data = new byte[1024];
                int len = socket.Receive(data);
                var message = TcpSegment.FromByteArray(TcpSegment.GetBytes(data, len));

                Console.WriteLine($"syn = {message.syn}, ack = {message.ack} в {message.sourcePort}.");
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
