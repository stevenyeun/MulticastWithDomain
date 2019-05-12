using SocketLib_Multicast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        private static void ReceiveBufferCallback(byte[] receiveBuffer)
        {
            string recvMessage = Encoding.UTF8.GetString(receiveBuffer);
            System.Console.WriteLine("receiveMessage : " + recvMessage);
            recvMessage = recvMessage.ToLower();
        }

        
        public static UDPMulticastSocketWithDomain socket = new UDPMulticastSocketWithDomain(
            MULTICAST_DOMAIN.TRACK_INFO, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback);

        static void Main(string[] args)
        {
          

            //StartTimerSendPacket();

            Console.WriteLine("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {

            }      
        }
        public static void StartTimerSendPacket()
        {
            var timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.Start();
            timer.Elapsed += (sender, args) =>
            {
                timer.Stop();

             
                Program.socket.SendPacket("aaaaaaaaaa");

                timer.Start();
            };
        }


    }
}
