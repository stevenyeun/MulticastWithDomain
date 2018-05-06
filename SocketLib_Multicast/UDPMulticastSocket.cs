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
        CONTROL = 1,
        UISTATUS,
        ETC      
    }

    public class UDPMulticastSenderWithDomain
    {
        private UdpClient udpMulticastClient = new UdpClient();
        private IPEndPoint remoteEP;

        public UDPMulticastSenderWithDomain(MULTICAST_DOMAIN domain)
        {
            IPAddress multicastaddress;
            switch (domain)
            {
                case MULTICAST_DOMAIN.CONTROL:
                    multicastaddress = IPAddress.Parse("224.0.0.1");
                    break;
                case MULTICAST_DOMAIN.UISTATUS:
                    multicastaddress = IPAddress.Parse("224.0.0.2");
                    break;
                case MULTICAST_DOMAIN.ETC:
                    multicastaddress = IPAddress.Parse("224.0.0.3");
                    break;
                default:
                     multicastaddress = IPAddress.Parse("224.0.0.1");
                    break;
            }
 
            udpMulticastClient.JoinMulticastGroup(multicastaddress);
            remoteEP = new IPEndPoint(multicastaddress, 2222);       
        }

        public void SendStatus(string msg)
        {           
            byte[] buffer = null;
            buffer = Encoding.Unicode.GetBytes(msg);
            udpMulticastClient.Send(buffer, buffer.Length, remoteEP);
        }
    }
    public class UDPMulticastReceiverWithDomain
    {
        public delegate void ReceiveBufferCallback(byte[] receiveBuffer);

        public UDPMulticastReceiverWithDomain(MULTICAST_DOMAIN domain, ReceiveBufferCallback callBackFunc)
        {

            IPAddress multicastaddress;
            switch (domain)
            {
                case MULTICAST_DOMAIN.CONTROL:
                    multicastaddress = IPAddress.Parse("224.0.0.1");
                    break;
                case MULTICAST_DOMAIN.UISTATUS:
                    multicastaddress = IPAddress.Parse("224.0.0.2");
                    break;
                case MULTICAST_DOMAIN.ETC:
                    multicastaddress = IPAddress.Parse("224.0.0.3");
                    break;
                default:
                    multicastaddress = IPAddress.Parse("224.0.0.1");
                    break;
            }

            int port = 2222;
        
            // 소켓 생성 및 필요시 소켓 옵션 지정
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sock.ExclusiveAddressUse = false;
            // 소켓 바인드
            sock.Bind(new IPEndPoint(IPAddress.Any, port));

            // 멀티캐스트 그룹에 가입
            IPAddress multicastIP = multicastaddress;
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




