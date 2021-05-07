using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Lab2_Server
{
    class Server
    {
        public void Listener()
        {
            Console.WriteLine($"Сервер (SS) подключен к порту 127.0.0.1:{Config.SS_port}");
            UdpClient reciever = new UdpClient(Config.SS_port);
            IPEndPoint IP = null;
            try
            {
                while (true)
                {
                    byte[] data = reciever.Receive(ref IP);
                    IP.Port = Config.C_port;

                    var message = Serilizer<Message>.Deserilaze(data);
                    Console.WriteLine(message.ToString() + $"из порта {IP.Address}:{IP.Port}");
                    if (message.Type == MessageType.C_To_SS)
                    {
                        var tgs_bytes = Helper.RecoverData(
                            new List<byte>(DES.Decrypt(message.Data[0].ToArray(), Config.TGS_To_SS_Key)));
                        var tgs = Serilizer<TicketGranting>.Deserilaze(tgs_bytes);

                        var c = Helper.RecoverData(
                            new List<byte>(DES.Decrypt(message.Data[1].ToArray(), Config.C_To_SS_Key)));
                        var c2 = Serilizer<TimeMark>.Deserilaze(c);

                        Message ReMessage = new Message();
                        if (Helper.CheckTime(tgs.IssuingTime, c2.T, tgs.Duration))
                        {
                            ReMessage.Type = MessageType.SS_To_C;
                            DateTime reTime = c2.T;
                            var time_bytes = Serilizer<long>.Serilize(c2.T.Ticks + 1);
                            var bytes = DES.Encrypt(Helper.ExtendData(time_bytes),
                                                    Config.C_To_SS_Key);
                            ReMessage.Data.Add(new List<byte>(bytes));
                        }
                        else ReMessage.Type = MessageType.TicketNotValid;

                        ReMessage.Send(IP);
                        Console.WriteLine($"Сообщение отправлено из сервера (SS) в порт {IP.Address}:{IP.Port}");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
