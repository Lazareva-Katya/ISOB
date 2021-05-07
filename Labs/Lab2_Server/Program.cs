using System;
using System.Threading.Tasks;

namespace Lab2_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthServer AS = new AuthServer();
            TicketGrantingServer TGS = new TicketGrantingServer();
            Server SS = new Server();
            try
            {
                Task.Run(() => AS.Listener());
                Task.Run(() => TGS.Listener());
                Task.Run(() => SS.Listener());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.ReadLine();
        }
    }
}
