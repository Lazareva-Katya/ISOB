using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lab3
{
    class Hack3
    {
        public static Task Hack = new Task(Run);
        public static int count = 10;
        private static EndPoint server = new IPEndPoint(IPAddress.Parse(Config.host), Config.ServerListenPort);
        private static void Run()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                socket.Bind(new IPEndPoint(IPAddress.Parse(Config.host), 6660));
                for (int i = 0; i < count; i++)
                {
                    TcpSegment tcp = new TcpSegment(6661 + i, Config.ServerListenPort, syn: true);
                    socket.SendTo(tcp.ToByteArray(), server);
                    byte[] data = new byte[1024];
                    Socket socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    try
                    {
                        socket1.Bind(new IPEndPoint(IPAddress.Parse(Config.host), 6661 + i));
                        var count = socket1.Receive(data);
                        var msg = TcpSegment.FromByteArray(TcpSegment.GetBytes(data, count));
                        var re = new TcpSegment(msg.destinationPort, msg.sourcePort, ack: true);
                        socket1.SendTo(re.ToByteArray(), server);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        socket1.Close();
                    }

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
