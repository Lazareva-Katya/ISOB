using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Lab2_Client
{
    class Client
    {
        public string Login { get; set; }
        private byte[] TicketGrantingTicket { get; set; }
        private byte[] TicketGrantingService { get; set; }
        private byte[] C_To_TGS_Key { get; set; }
        private byte[] C_To_SS_Key { get; set; }
        DateTime T4 { get; set; }

        readonly TextBox _textBox;
        public Client(TextBox textBox)
        {
            _textBox = textBox;
        }

        private readonly IPEndPoint ASEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Config.AS_port);
        private readonly IPEndPoint SSEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Config.SS_port);
        private readonly IPEndPoint TGSEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Config.TGS_port);
        public void Register(string login)
        {
            Login = login;
            Message message = new Message(MessageType.C_To_AS);
            message.Data.Add(new List<byte>(Helper.ToByteArray(login)));
            message.Send(ASEndPoint);
        }
        
        public void Listener()
        {
            UdpClient reciever = new UdpClient(Config.C_port);
            IPEndPoint IP = null;
            try
            {
                while (true)
                {
                    byte[] data = reciever.Receive(ref IP);
                    Message message = Serilazer<Message>.Deserilaze(data);
                    switch (message.Type)
                    {
                        case MessageType.AS_To_C:
                            TicketGrantingTicket = DES.Decrypt(message.Data[0].ToArray(), Config.C_Key);
                            C_To_TGS_Key = Helper.RecoverData(new List<byte>(DES.Decrypt(message.Data[1].ToArray(), Config.C_Key)));
                            var a = Helper.ToString(C_To_TGS_Key);
                            Print("AS -> C. Клиент (C) имеется в базе сервера аутентификации (AS).");
                            break;
                        case MessageType.TGS_To_C:
                            TicketGrantingService = DES.Decrypt(message.Data[0].ToArray(), C_To_TGS_Key);
                            C_To_SS_Key = Helper.RecoverData(new List<byte>(DES.Decrypt(message.Data[1].ToArray(), C_To_TGS_Key)));

                            Message msg = new Message(MessageType.C_To_SS);
                            msg.Data.Add(new List<byte>(TicketGrantingService));

                            var mark = new TimeMark() { C = Login, T = DateTime.Now };
                            var Aut2 = Helper.ExtendData(Serilazer<TimeMark>.Serilize(mark));
                            T4 = mark.T;
                            msg.Data.Add(new List<byte>(DES.Encrypt(Aut2, C_To_SS_Key)));

                            msg.Send(SSEndPoint);

                            Print("TGS -> C. Сервер выдачи разрешений (TGS) посылает клиенту (C) ключ шифрования и билет, для доступа к серверу (SS).");
                            break;
                        case MessageType.SS_To_C:
                            var t = DES.Decrypt(message.Data[0].ToArray(), C_To_SS_Key);
                            var checkT_bytes = Helper.RecoverData(new List<byte>(t));
                            var asd = Helper.ToString(checkT_bytes);
                            var checkT = Serilazer<long>.Deserilaze(checkT_bytes);
                            if (T4.Ticks + 1 == checkT)
                            {
                                Print("Выполнено!");
                            }
                            break;
                        case MessageType.TicketNotValid:
                            Print("Билет недействителен.");
                            break;
                        case MessageType.AccessDenied:
                            Print("Отклонено в доступе.");
                            break;
                        default:
                            MessageBox.Show("Неверный тип сообщения.");
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void GetRes()
        {
            if (C_To_TGS_Key != null &&
                TicketGrantingTicket != null)
            {
                Message message = new Message(MessageType.С_To_TGS);
                message.Data.Add(new List<byte>(TicketGrantingTicket));

                TimeMark mark = new TimeMark() { C = Login, T = DateTime.Now };
                var Aut1 = Helper.ExtendData(Serilazer<TimeMark>.Serilize(mark));
                message.Data.Add(new List<byte>(DES.Encrypt(Aut1, C_To_TGS_Key)));
                message.Data.Add(new List<byte>(Helper.ToByteArray(Config.id_SS)));

                message.Send(TGSEndPoint);
            }
        }

        public void Print(string message)
        {
            _textBox.Invoke(new Action(() =>
            {
                _textBox.AppendText(message + "\r\n");
            }));
        }
    }
}
