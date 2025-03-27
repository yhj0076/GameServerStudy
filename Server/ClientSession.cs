using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server;

class ClientSession : PacketSession
{
    public override void OnConnected(EndPoint endpoint)
    {
        Console.WriteLine($"OnConnected bytes: {endpoint}");
        
        Thread.Sleep(5000);
        Disconnect();
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
	    PacketManager.Instance.OnRecvPacket(this, buffer);
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