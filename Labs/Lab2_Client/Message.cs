using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Lab2_Client
{
    [Serializable]
    class Message
    {
        public Message(MessageType messageType = 0)
        {
            Type = messageType;
            Data = new List<List<byte>>();
        }
        public Message() { }
        public MessageType Type { get; set; }
        public List<List<byte>> Data { get; set; }
        public void Send(IPEndPoint IP)
        {
            UdpClient sender = new UdpClient();

            try
            {
                byte[] dgram = Serilazer<Message>.Serilize(this);
                sender.Send(dgram, dgram.Length, IP);
            }
            finally
            {
                sender.Close();
            }
        }
    }
}
