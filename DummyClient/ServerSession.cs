using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient;

public class ServerSession : PacketSession
{

    /*static unsafe void ToBytes(byte[] array, int offset, ulong value)
    {
        fixed(byte* ptr = &array[offset])
            *(ulong*)ptr = value;
    }*/
    
    public override void OnConnected(EndPoint endpoint)
    {
        Console.WriteLine($"OnConnected : {endpoint}");
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
        Console.WriteLine($"OnDisconnected bytes: {endpoint}");
    }
}
