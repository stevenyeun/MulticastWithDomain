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
        private static void Display(byte[] receiveBuffer)
        {
            string recvMessage = Encoding.UTF8.GetString(receiveBuffer);
            recvMessage = recvMessage.Replace("\0", "");
            System.Console.WriteLine("receiveMessage : " + recvMessage);
        }
        private static void ReceiveBufferCallback1(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback2(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback3(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }      
        private static void ReceiveBufferCallback4(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback5(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback6(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback7(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback8(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback9(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback10(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback11(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback12(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        private static void ReceiveBufferCallback13(byte[] receiveBuffer)
        {
            Display(receiveBuffer);
        }
        public static UDPMulticastSocketWithDomain socket1 = new UDPMulticastSocketWithDomain(
            MULTICAST_DOMAIN.CONTROL, MULTICAST_CHANNEL.CH1, ReceiveBufferCallback1);

        //  public static UDPMulticastSocketWithDomain socket2 = new UDPMulticastSocketWithDomain(
        //MULTICAST_DOMAIN.CONTROL_OBJ, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback2);

        //  public static UDPMulticastSocketWithDomain socket3 = new UDPMulticastSocketWithDomain(
        //MULTICAST_DOMAIN.UI_STATE_INFO, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback3);

        //  public static UDPMulticastSocketWithDomain socket4 = new UDPMulticastSocketWithDomain(
        //MULTICAST_DOMAIN.REF_POINTS_INFO, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback4);

  //      public static UDPMulticastSocketWithDomain socket5 = new UDPMulticastSocketWithDomain(
  //MULTICAST_DOMAIN.TRACK_INFO, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback5);

//        public static UDPMulticastSocketWithDomain socket6 = new UDPMulticastSocketWithDomain(
//      MULTICAST_DOMAIN.INTEREST_TRACK_INFO, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback6);

//        public static UDPMulticastSocketWithDomain socket7 = new UDPMulticastSocketWithDomain(
//      MULTICAST_DOMAIN.CAMERA_INFO, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback7);

//        public static UDPMulticastSocketWithDomain socket8 = new UDPMulticastSocketWithDomain(
//   MULTICAST_DOMAIN.COMMON, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback8);

//        public static UDPMulticastSocketWithDomain socket9 = new UDPMulticastSocketWithDomain(
//MULTICAST_DOMAIN.CONTROL_PANORAMA, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback9);


//        public static UDPMulticastSocketWithDomain socket10 = new UDPMulticastSocketWithDomain(
//MULTICAST_DOMAIN.RECORD, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback10);

//        public static UDPMulticastSocketWithDomain socket11 = new UDPMulticastSocketWithDomain(
//MULTICAST_DOMAIN.FIRE_MONITORING, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback11);

//        public static UDPMulticastSocketWithDomain socket12 = new UDPMulticastSocketWithDomain(
//MULTICAST_DOMAIN.VIDEO, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback12);

//        public static UDPMulticastSocketWithDomain socket13 = new UDPMulticastSocketWithDomain(
//MULTICAST_DOMAIN.REPORT_STATUS, MULTICAST_CHANNEL.COMMON, ReceiveBufferCallback13);

        
        static void Main(string[] args)
        {


            //StartTimerSendPacket();
        
            Console.WriteLine("Press <Enter> to exit... ");
            while (true)
            {
                ConsoleKey temp = Console.ReadKey().Key;
                if(temp == ConsoleKey.Enter)
                {
                    return;
                }
                if (temp == ConsoleKey.S)
                {
                    Program.socket1.SendPacket(Program.socket1.MulticastAddress.ToString() + " " + Program.socket1.MulticastPort.ToString());
                    //Program.socket2.SendPacket(Program.socket2.MulticastAddress.ToString() + " " + Program.socket2.MulticastPort.ToString());
                    //Program.socket3.SendPacket(Program.socket3.MulticastAddress.ToString() + " " + Program.socket3.MulticastPort.ToString());
                    //Program.socket4.SendPacket(Program.socket4.MulticastAddress.ToString() + " " + Program.socket4.MulticastPort.ToString());
                    //Program.socket5.SendPacket(Program.socket5.MulticastAddress.ToString() + " " + Program.socket5.MulticastPort.ToString());
                    //Program.socket6.SendPacket(Program.socket6.MulticastAddress.ToString() + " " + Program.socket6.MulticastPort.ToString());
                    //Program.socket7.SendPacket(Program.socket7.MulticastAddress.ToString() + " " + Program.socket7.MulticastPort.ToString());
                    //Program.socket8.SendPacket(Program.socket8.MulticastAddress.ToString() + " " + Program.socket8.MulticastPort.ToString());
                    //Program.socket9.SendPacket(Program.socket9.MulticastAddress.ToString() + " " + Program.socket9.MulticastPort.ToString());
                    //Program.socket10.SendPacket(Program.socket10.MulticastAddress.ToString() + " " + Program.socket10.MulticastPort.ToString());
                    //Program.socket11.SendPacket(Program.socket11.MulticastAddress.ToString() + " " + Program.socket11.MulticastPort.ToString());
                    //Program.socket12.SendPacket(Program.socket12.MulticastAddress.ToString() + " " + Program.socket12.MulticastPort.ToString());
                    //Program.socket13.SendPacket(Program.socket13.MulticastAddress.ToString() + " " + Program.socket13.MulticastPort.ToString());
                }
                if (temp == ConsoleKey.C)
                {
                    Console.Clear();
                }
            }      
        }
  

    }
}
