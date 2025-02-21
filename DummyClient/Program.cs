using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }
    
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endpoint)
        {
            Packet packet = new Packet() { size = 4, packetId = 7 };
            
            // 보낸다
            for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                byte[] buffer = BitConverter.GetBytes(packet.size);
                byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
                Array.Copy(buffer, 0, openSegment.Array,openSegment.Offset, buffer.Length);
                Array.Copy(buffer2, 0, openSegment.Array,openSegment.Offset + buffer.Length, buffer2.Length);
                ArraySegment<byte> sendBuff = SendBufferHelper.Close(packet.size);
                
                Send(sendBuff);
            }
        }
    
        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }
    
        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    
        public override void OnDisconnected(EndPoint endpoint)
        {
            Console.WriteLine($"OnDisconnected bytes: {endpoint}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // DNS(Domain Name System
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => {return new GameSession();});

            while (true)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(100);
            }
        }
    }
}