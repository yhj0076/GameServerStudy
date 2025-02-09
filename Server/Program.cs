using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server;

class GameSession : Session
{
    class Knight
    {
        public int hp;
        public int attack;
    }
    
    public override void OnConnected(EndPoint endpoint)
    {
        Console.WriteLine($"OnConnected bytes: {endpoint}");

        Knight knight = new Knight() { hp = 100, attack = 10 };
        
        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        byte[] buffer = BitConverter.GetBytes(knight.hp);
        byte[] buffer2 = BitConverter.GetBytes(knight.attack);
        Array.Copy(buffer, 0, openSegment.Array,openSegment.Offset, buffer.Length);
        Array.Copy(buffer2, 0, openSegment.Array,openSegment.Offset + buffer.Length, buffer2.Length);
        ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
        
        // 100명
        // 1 -> 이동패킷이 100명
        // 100 -> 이동패킷이 100 * 100 = 1만
        Send(sendBuff);
        Thread.Sleep(1000);
        Disconnect();
    }

    public override int OnRecv(ArraySegment<byte> buffer)
    {
        string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Console.WriteLine($"[From Client] {recvData}");
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