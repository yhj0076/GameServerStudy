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

class PlayerInfoReq : Packet
{
    public long playerId;
}

class PlaerInfoOk : Packet
{
    public int hp;
    public int attack;
}

public enum PacketID
{
    PlayerInfoReq = 1,
    PlayerInfoOk = 2,
}

class ClientSession : PacketSession
{
    public override void OnConnected(EndPoint endpoint)
    {
        Console.WriteLine($"OnConnected bytes: {endpoint}");

        // Packet packet = new Packet() { size = 100, packetId = 10 };
        //
        // ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        // byte[] buffer = BitConverter.GetBytes(packet.size);
        // byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
        // Array.Copy(buffer, 0, openSegment.Array,openSegment.Offset, buffer.Length);
        // Array.Copy(buffer2, 0, openSegment.Array,openSegment.Offset + buffer.Length, buffer2.Length);
        // ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
        //
        // // 100명
        // // 1 -> 이동패킷이 100명
        // // 100 -> 이동패킷이 100 * 100 = 1만
        // Send(sendBuff);
        Thread.Sleep(5000);
        Disconnect();
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        switch ((PacketID)id)
        {
            case PacketID.PlayerInfoReq:
            {
                long plaerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                count += 8;
                Console.WriteLine($"PlayerInfoReq : {plaerId}");
                break;
            }
        }
        
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