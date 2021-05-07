using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Lab2_Server
{
    class Message
    {
        public Message(MessageType messageType = 0)
        {
            Type = messageType;
            Data = new List<List<byte>>();
        }
        public Message() { Data = new List<List<byte>>(); }
        public MessageType Type { get; set; }
        public List<List<byte>> Data { get; set; }
        public void Send(IPEndPoint remoteIP)
        {
            UdpClient sender = new UdpClient();

            try
            {
                byte[] dgram = Serilizer<Message>.Serilize(this);
                sender.Send(dgram, dgram.Length, remoteIP);
            }
            finally
            {
                sender.Close();
            }
        }
        public override string ToString()
        {
            return $"Получено сообщение типа {Type} ";
        }
    }
}
