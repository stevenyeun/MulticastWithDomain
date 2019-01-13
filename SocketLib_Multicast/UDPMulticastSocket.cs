using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketLib_Multicast
{

    public enum MULTICAST_DOMAIN
    {
        /// <summary>
        /// 명령, 요청
        /// </summary>
        CONTROL = 1,
        CONTROL_OBJ,


        /// <summary>
        /// 정보수신
        /// </summary>
        UI_STATE_INFO,//UI변경상태
        REF_POINTS_INFO,//참조점들 정보
        TRACK_INFO,//추적표적정보
        INTEREST_TRACK_INFO,//관심표적정보
        CAMERA_INFO,//카메라정보
        COMMON,//공통정보      
        CONTROL_PANORAMA//공통정보      
        /////////////////////////
    }
    public enum MULTICAST_CHANNEL
    {
        COMMON = 10000,
        CH1 = 10001,
        CH2,
        CH3,
        CH4,
        CH5,
        CH6,
        CH7,
        CH8,
        CH9,
        CH10,
        CH11,
        CH12
    }

    public class BUFFER
    {
        public const int SIZE = 65536/2;
    }

    public class Converter
    {      
        static public IPAddress DomainToIPAddress(MULTICAST_DOMAIN domain)
        {
            IPAddress multicastaddress;
            switch (domain)
            {
                case MULTICAST_DOMAIN.CONTROL:
                    multicastaddress = IPAddress.Parse("224.0.0.1");
                    break;
                case MULTICAST_DOMAIN.CONTROL_OBJ:
                    multicastaddress = IPAddress.Parse("224.0.0.2");
                    break;
                case MULTICAST_DOMAIN.UI_STATE_INFO:
                    multicastaddress = IPAddress.Parse("224.0.0.3");
                    break;
                case MULTICAST_DOMAIN.REF_POINTS_INFO:
                    multicastaddress = IPAddress.Parse("224.0.0.4");
                    break;
                case MULTICAST_DOMAIN.TRACK_INFO:
                    multicastaddress = IPAddress.Parse("224.0.0.5");
                    break;
                case MULTICAST_DOMAIN.INTEREST_TRACK_INFO:
                    multicastaddress = IPAddress.Parse("224.0.0.6");
                    break;
                case MULTICAST_DOMAIN.CAMERA_INFO:
                    multicastaddress = IPAddress.Parse("224.0.0.7");
                    break;
                case MULTICAST_DOMAIN.COMMON:
                    multicastaddress = IPAddress.Parse("224.0.0.8");
                    break;
                case MULTICAST_DOMAIN.CONTROL_PANORAMA:
                    multicastaddress = IPAddress.Parse("224.0.0.9");
                    break;
                default:
                    multicastaddress = IPAddress.Parse("224.0.0.1");
                    break;
            }

            return multicastaddress;
        }
    }


    /*
    public class UDPMulticastSenderWithDomain
    {
        private UdpClient udpMulticastClient = new UdpClient();
        private IPEndPoint remoteEP;

        public UDPMulticastSenderWithDomain(MULTICAST_DOMAIN domain, MULTICAST_CHANNEL channel)
        {
            IPAddress multicastaddress = Converter.DomainToIPAddress(domain);
         
 
            udpMulticastClient.JoinMulticastGroup(multicastaddress);
            remoteEP = new IPEndPoint(multicastaddress, (int)channel);

        }

        public void SendPacket(string msg)
        {           
            byte[] buffer = new byte[BUFFER.SIZE];
            byte[] temp = Encoding.Unicode.GetBytes(msg);

            Array.Copy(temp, buffer, temp.Length);
            udpMulticastClient.Send(buffer, buffer.Length, remoteEP);
        }
    }
    */
    public class UDPMulticastSocketWithDomain
    {
        private Socket m_sock;
        IPAddress m_multicastAddress;
        private int m_nMulticastPort;
        public void SendPacket(string msg)
        {
            byte[] buffer = new byte[BUFFER.SIZE];
#if false
            byte[] temp = Encoding.Unicode.GetBytes(msg);
#else
            byte[] temp = Encoding.UTF8.GetBytes(msg);
#endif
            Array.Copy(temp, buffer, temp.Length);

            EndPoint ep = new IPEndPoint(m_multicastAddress, m_nMulticastPort);
            int n = m_sock.SendTo(buffer, 0, buffer.Length, SocketFlags.None, ep);
        }
        public delegate void ReceiveBufferCallback(byte[] receiveBuffer);

        public UDPMulticastSocketWithDomain(MULTICAST_DOMAIN domain, MULTICAST_CHANNEL channel, ReceiveBufferCallback callBackFunc)
        {

            m_multicastAddress = Converter.DomainToIPAddress(domain);

            m_nMulticastPort = (int)channel;
        
            // 소켓 생성 및 필요시 소켓 옵션 지정
            m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);          
            m_sock.ExclusiveAddressUse = false;

            // 소켓 바인드
            m_sock.Bind(new IPEndPoint(IPAddress.Any, m_nMulticastPort));

            // 멀티캐스트 그룹에 가입
            IPAddress multicastIP = m_multicastAddress;

            try
            {
#if true
                int loopback = (int)m_sock.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback);
                m_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastIP, IPAddress.Any));
                m_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback/*Loopback 설정*/, loopback);
                
#else
                m_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastIP, IPAddress.Any));
#endif
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            //수신
            Task.Run(() =>
            {
                byte[] buff = new byte[BUFFER.SIZE];
                EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    try
                    {
                        // 데이타 수신
                        //Console.WriteLine("recv blocking in multicastReceiverSocket Port={0}", nMulticastReceiverPort);
                        //var recvBuffer;
                        int n = m_sock.ReceiveFrom(buff, 0, buff.Length, SocketFlags.None, ref ep);
                        //var buffer;
                        //sock.ReceiveFrom(buffer, ref ep);
                        //string recvMessage = Encoding.ASCII.GetString(buff, 0, buff.Length);
                        string recvMessage = Encoding.UTF8.GetString(buff, 0, n);
                        //Console.WriteLine("Port={0} {1}", port, recvMessage);

                        byte[] sendBuffer = new byte[n];

                        Array.Copy(buff, 0, sendBuffer, 0, n);
                        callBackFunc(sendBuffer);
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                        Thread.Sleep(5000);
                        // 멀티캐스트 그룹에서 탈퇴
                        m_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(multicastIP, IPAddress.Any));
                        m_sock.Close();

                    }
                    Thread.Sleep(1);
                }
             
            });

           
        }
        
    }

















    public class UDPMulticastSender
    {
        private UdpClient udpMulticastClient = new UdpClient();
        private IPEndPoint remoteEP;

        public UDPMulticastSender(string ipAddr, int port)
        {
            IPAddress multicastaddress = IPAddress.Parse(ipAddr);
            udpMulticastClient.JoinMulticastGroup(multicastaddress);
            remoteEP = new IPEndPoint(multicastaddress, port);
        }

        public void SendPacket(byte[] buffer)
        {
            //System.Console.WriteLine(msg);
            //byte[] buffer = null;
            //buffer = Encoding.Unicode.GetBytes(msg);
            udpMulticastClient.Send(buffer, buffer.Length, remoteEP);
        }
    }
    public class UDPMulticastReceiver
    {
        public delegate void ReceiveBufferCallback(byte[] receiveBuffer);

        public UDPMulticastReceiver(string ipAddr, int port, ReceiveBufferCallback callBackFunc)
        {
            //int port = 2222;
            //string ipAddr = "239.0.0.200";

            // 소켓 생성 및 필요시 소켓 옵션 지정
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sock.ExclusiveAddressUse = false;
            // 소켓 바인드
            sock.Bind(new IPEndPoint(IPAddress.Any, port));

            // 멀티캐스트 그룹에 가입
            IPAddress multicastIP = IPAddress.Parse(ipAddr);
            sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastIP, IPAddress.Any));

            byte[] buff = new byte[1024];
            EndPoint ep = new IPEndPoint(IPAddress.Any, 0);

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        // 데이타 수신
                        //Console.WriteLine("recv blocking in multicastReceiverSocket Port={0}", nMulticastReceiverPort);
                        //var recvBuffer;
                        int n = sock.ReceiveFrom(buff, 0, buff.Length, SocketFlags.None, ref ep);
                        //var buffer;
                        //sock.ReceiveFrom(buffer, ref ep);
                        //string recvMessage = Encoding.ASCII.GetString(buff, 0, buff.Length);
                        string recvMessage = Encoding.UTF8.GetString(buff, 0, n);
                        //Console.WriteLine("Port={0} {1}", port, recvMessage);

                        byte[] sendBuffer = new byte[n];

                        Array.Copy(buff, 0, sendBuffer, 0, n);
                        callBackFunc(sendBuffer);
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                        Thread.Sleep(5000);

                        // 멀티캐스트 그룹에서 탈퇴
                        sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(multicastIP, IPAddress.Any));
                        sock.Close();
                    }
                    Thread.Sleep(1);
                }
            });
        }

    }


}




