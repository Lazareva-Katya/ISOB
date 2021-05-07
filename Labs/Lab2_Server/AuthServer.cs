using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Lab2_Server
{
    class AuthServer
    {
        private readonly List<string> Users = new List<string>();
        public void Listener()
        {
            UdpClient reciever = new UdpClient(Config.AS_port);
            Console.WriteLine($"Сервер аутентификации (AS) подключен к порту 127.0.0.1:{Config.AS_port}");
            IPEndPoint IP = null;
            try
            {
                while (true)
                {
                    byte[] data = reciever.Receive(ref IP);
                    IP.Port = Config.C_port;
                    Message ReMessage = new Message();

                    var message = Serilizer<Message>.Deserilaze(data);

                    if (message.Type == MessageType.C_To_AS)
                    {
                        var id = Helper.ToString(message.Data[0].ToArray());
                        Console.WriteLine($"Сообщение из порта {IP.Address}:{IP.Port} идентификатор c = {id} проверка нахождения клиента в базе сервера аутентификации (AS)!");
                        if (Users.Contains(id))
                        {
                            ReMessage.Type = MessageType.AS_To_C;

                            TicketGranting ticket = new TicketGranting()
                            {
                                ClientIdentity = id,
                                Duration = Config.AS_Duration.Ticks,
                                IssuingTime = DateTime.Now,
                                ServiceIdentity = Config.TGS,
                                Key = Helper.ToString(Config.C_To_TGS_Key)
                            };
                            var ticket_bytes = Helper.ExtendData(Serilizer<TicketGranting>.Serilize(ticket));
                            var c_to_tgs_key_bytes = Helper.ExtendData(Config.C_To_TGS_Key);

                            var tb_enc = DES.Encrypt(ticket_bytes, Config.AS_To_TGS_Key);
                            tb_enc = DES.Encrypt(tb_enc, Config.C_Key);

                            var c_to_tgs_key_enc = DES.Encrypt(c_to_tgs_key_bytes, Config.C_Key);

                            ReMessage.Data.Add(new List<byte>(tb_enc));
                            ReMessage.Data.Add(new List<byte>(c_to_tgs_key_enc));

                        }
                        else
                        {
                            ReMessage.Type = MessageType.AccessDenied;
                            Console.WriteLine("Доступ к серверу аутентификации (AS) запрещен.");
                        }
                        ReMessage.Send(IP);
                        Console.WriteLine($"Сообщение отправлено из сервера аутентификации (AS) в порт {IP.Address}:{IP.Port}!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
        public AuthServer()
        {
            Users.Add("ekaterina");
            Users.Add("client");
        }
    }
}
