using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lab3
{
    static class Server
    {
        static IPEndPoint listenPoint = new IPEndPoint(IPAddress.Parse(Config.host), Config.ServerListenPort);
        public static Task Listen = new Task(Listener);
        static Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        static List<int> SynRecived = new List<int>();
        static List<int> Established = new List<int>();

        static int MaxSynRecived = 8;
        static int MaxEstablished = 8;

        public static void Listener()
        {
            Console.WriteLine($"Сервер начал работу на {listenPoint.Address}:{listenPoint.Port}.");

            try
            {
                listenSocket.Bind(listenPoint);
                while (true)
                {
                    byte[] data = new byte[1024];
                    int len = listenSocket.Receive(data);
                    TcpSegment message = TcpSegment.FromByteArray(TcpSegment.GetBytes(data, len));
                    if (message.destinationPort != Config.ServerListenPort)
                        continue;
                    else if (Established.Contains(message.sourcePort))
                        continue;
                    else if (SynRecived.Contains(message.sourcePort) && message.ack)
                        AddToConnected(message);
                    else if (!SynRecived.Contains(message.sourcePort) && message.syn)
                        AddToSynReceived(message);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            finally
            {
                listenSocket.Close();
            }
        }
        public static void AddToSynReceived(TcpSegment request)
        {
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sender.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111));
            try
            {
                var response = new TcpSegment(Config.ServerListenPort, request.sourcePort, syn: true, ack: true);
                sender.SendTo(response.ToByteArray(),
                    new IPEndPoint(IPAddress.Parse(Config.host), request.sourcePort));
                SynRecived.Add(request.sourcePort);
                Console.WriteLine($"{request.sourcePort} added to SynReceived");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();
            }
            if (SynRecived.Count > MaxSynRecived)
                throw new Exception("SynReceived is overloaded");
        }
        public static void AddToConnected(TcpSegment request)
        {
            Established.Add(request.sourcePort);
            SynRecived.Remove(request.sourcePort);
            Console.WriteLine($"{request.sourcePort} added to Connected");
            if (Established.Count > MaxEstablished)
                throw new Exception("Connected is overloaded");
        }
    }
}
