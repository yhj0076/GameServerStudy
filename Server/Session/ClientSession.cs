using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server;

public class ClientSession : PacketSession
{
    public int SessionId { get; set; }
    public GameRoom Room { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    
    public override void OnConnected(EndPoint endpoint)
    {
        Console.WriteLine($"OnConnected bytes: {endpoint}");
        
        Program.Room.Push(() => Program.Room.Enter(this));
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
	    PacketManager.Instance.OnRecvPacket(this, buffer);
    }
    
    public override void OnSend(int numOfBytes)
    {
        // Console.WriteLine($"Transferred bytes: {numOfBytes}");
    }

    public override void OnDisconnected(EndPoint endpoint)
    {
        SessionManager.Instance.Remove(this);
        if (Room != null)
        {
            GameRoom room = Room;
            room.Push(() => room.Leave(this));
            Room = null;
        }
        
        Console.WriteLine($"OnDisconnected bytes: {endpoint}"); 
    }
}