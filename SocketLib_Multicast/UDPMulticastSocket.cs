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
        CONTROL_PANORAMA,//파노라마정보
        RECORD,//녹화 정보
        FIRE_MONITORING//화재감시 정보
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
                case MULTICAST_DOMAIN.RECORD:
                    multicastaddress = IPAddress.Parse("224.0.0.10");
                    break;
                case MULTICAST_DOMAIN.FIRE_MONITORING:
                    multicastaddress = IPAddress.Parse("224.0.0.11");
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

        public UDPMulticastSocketWithDomain(MULTICAST_DOMAIN domain, MULTICAST_CHANNEL channel, ReceiveBufferCallback callBackFunc, string strLocalIPAddr = "")
        {
            m_multicastAddress = Converter.DomainToIPAddress(domain);
          
            m_nMulticastPort = (int)channel;
            try
            {
                // 소켓 생성 및 필요시 소켓 옵션 지정
                m_sock = new Socket(AddressFamily.InterNetwork,
                                    SocketType.Dgram,
                                    ProtocolType.Udp);
                m_sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                m_sock.ExclusiveAddressUse = false;

                // 소켓 바인드
                EndPoint localEP = null;
                MulticastOption mcastOption = null;
                IPAddress localIPAddr = null;
                if (strLocalIPAddr == "")
                {
                    localIPAddr = IPAddress.Any;
                    localEP = new IPEndPoint(IPAddress.Any, m_nMulticastPort);
                    mcastOption = new MulticastOption(m_multicastAddress, IPAddress.Any);                    
                }
                else
                {
                    //아이피 지정 바인드
                    localIPAddr = IPAddress.Parse(strLocalIPAddr);
                    localEP = new IPEndPoint(localIPAddr, m_nMulticastPort);                    
                    mcastOption = new MulticastOption(m_multicastAddress, localIPAddr);
                }
                m_sock.Bind(localEP);

                // 멀티캐스트 그룹에 가입                
#if false
                int loopback = (int)m_sock.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback);
                m_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastIP, IPAddress.Any));
                m_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback/*Loopback 설정*/, loopback);
                
#else
                
                m_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, mcastOption);
#endif
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            //수신
            Task.Run(() =>
            {
                byte[] buff = new byte[BUFFER.SIZE];
                EndPoint ep = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
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
                        m_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(m_multicastAddress, IPAddress.Any));
                        m_sock.Close();

                    }
                    Thread.Sleep(1);
                }

            });


        }

    }
}




