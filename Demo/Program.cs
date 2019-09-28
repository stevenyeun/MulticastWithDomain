using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketLib_Multicast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Demo
{
    class Program
    {
        private static void ReceiveBufferCallback(byte[] receiveBuffer)
        {
            string recvMessage = Encoding.UTF8.GetString(receiveBuffer);
            //recvMessage = recvMessage.Replace("\0", "");
            //recvMessage = recvMessage.Replace("\"", "\\r\\n");
            //System.Console.WriteLine("receiveMessage : " + recvMessage);
            //recvMessage = recvMessage.ToLower();

       
            string prettyJson = JToken.Parse(recvMessage).ToString(Newtonsoft.Json.Formatting.Indented);
            System.Console.WriteLine("receiveMessage : " + prettyJson);
        }

        
        public static UDPMulticastSocketWithDomain socket = new UDPMulticastSocketWithDomain(
            MULTICAST_DOMAIN.CAMERA_INFO, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback);

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
