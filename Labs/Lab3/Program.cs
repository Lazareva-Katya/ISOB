using System;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Listen.Start();
            Hack1.Hack.Start();
            Console.ReadLine();
        }
    }
}
