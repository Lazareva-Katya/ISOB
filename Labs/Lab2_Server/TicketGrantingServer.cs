using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Lab2_Server
{
    class TicketGrantingServer
    {
        public void Listener()
        {
            UdpClient reciever = new UdpClient(Config.TGS_port);
            Console.WriteLine($"Сервер выдачи разрешений (TGS) подключен к порту 127.0.0.1:{Config.TGS_port}");
            IPEndPoint IP = null;
            try
            {
                while (true)
                {
                    byte[] data = reciever.Receive(ref IP);
                    IP.Port = Config.C_port;

                    var message = Serilizer<Message>.Deserilaze(data);

                    if (message.Type == MessageType.С_To_TGS)
                    {
                        var tgt_json = Helper.RecoverData(
                            new List<byte>(DES.Decrypt(message.Data[0].ToArray(), Config.AS_To_TGS_Key)));
                        var tgt = Serilizer<TicketGranting>.Deserilaze(tgt_json);

                        var c_json = Helper.RecoverData(
                            new List<byte>(DES.Decrypt(message.Data[1].ToArray(), Config.C_To_TGS_Key)));
                        var a = Helper.ToString(c_json);

                        var c = Serilizer<TimeMark>.Deserilaze(c_json);

                        var id = Helper.ToString(message.Data[2].ToArray());

                        Message ReMessage = new Message();

                        if (tgt.ClientIdentity == c.C)
                        {
                            if (Helper.CheckTime(tgt.IssuingTime, c.T, tgt.Duration))
                            {
                                ReMessage.Type = MessageType.TGS_To_C;
                                var TGS = new TicketGranting()
                                {
                                    ClientIdentity = c.C,
                                    ServiceIdentity = id,
                                    Duration = Config.TGS_Duration.Ticks,
                                    IssuingTime = DateTime.Now,
                                    Key = Helper.ToString(Config.C_To_SS_Key)
                                };
                                var ticket_bytes = Helper.ExtendData(Serilizer<TicketGranting>.Serilize(TGS));
                                var c_to_ss_key_bytes = Helper.ExtendData(Config.C_To_SS_Key);

                                var tb_enc = DES.Encrypt(ticket_bytes, Config.TGS_To_SS_Key);
                                tb_enc = DES.Encrypt(tb_enc, Config.C_To_TGS_Key);

                                var c_to_ss_key_enc = DES.Encrypt(c_to_ss_key_bytes, Config.C_To_TGS_Key);

                                ReMessage.Data.Add(new List<byte>(tb_enc));
                                ReMessage.Data.Add(new List<byte>(c_to_ss_key_enc));
                            }
                            else
                            {
                                ReMessage.Type = MessageType.TicketNotValid;
                                Console.WriteLine("Билет в сервере выдачи разрешений(TGS) недействителен.");
                            }
                        }
                        else
                        {
                            ReMessage.Type = MessageType.AccessDenied;
                            Console.WriteLine("Доступ к серверу выдачи разрешений (TGS) запрещен.");
                        }
                        ReMessage.Send(IP);
                        Console.WriteLine($"Сообщение отправлено из сервера выдачи разрешений (TGS) в порт {IP.Address}:{IP.Port}");
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
