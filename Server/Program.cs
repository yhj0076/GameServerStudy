using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server;

class Packet
{
    public ushort size;
    public ushort packetId;
}

class GameSession : PacketSession
{
    public override void OnConnected(EndPoint endpoint)
    {
        Console.WriteLine($"OnConnected bytes: {endpoint}");

        /*Packet packet = new Packet() { size = 100, packetId = 10 };
        
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        byte[] buffer = BitConverter.GetBytes(packet.size);
        byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
        Array.Copy(buffer, 0, openSegment.Array,openSegment.Offset, buffer.Length);
        Array.Copy(buffer2, 0, openSegment.Array,openSegment.Offset + buffer.Length, buffer2.Length);
        ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
        
        // 100명
        // 1 -> 이동패킷이 100명
        // 100 -> 이동패킷이 100 * 100 = 1만
        Send(sendBuff);*/
        Thread.Sleep(5000);
        Disconnect();
    }

    public override void OnRecvPacket(ArraySegment<byte> arraySegment)
    {
        ushort size = BitConverter.ToUInt16(arraySegment.Array, arraySegment.Offset);
        ushort id = BitConverter.ToUInt16(arraySegment.Array, arraySegment.Offset + 2);
        Console.WriteLine($"OnRecvPacket id : {id}, size: {size}");
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
    static Listener _listener = new Listener();
    static void Main(string[] args)
    {
        // DNS(Domain Name System
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        // www.gomi.com -> 123.123.123.123

        _listener.init(endPoint, () => {
            return new GameSession();
        });
        Console.WriteLine("Listening...");
        while (true)
        {

        }
    }
}