using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server;

public abstract class Packet
{
    public ushort size;
    public ushort packetId;

    public abstract ArraySegment<byte> Write();
    public abstract void Read(ArraySegment<byte> s);
}

class PlayerInfoReq : Packet
{
    public long playerId;

    public PlayerInfoReq()
    {
        this.packetId = (ushort)PacketID.PlayerInfoReq;
    }

    public override ArraySegment<byte> Write()
    {
        ArraySegment<byte> s = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;
            
            
        count += 2;
        success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
        count += 2;
        success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
        count += 8;
        success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

        if (!success)
            return null;
        
        return SendBufferHelper.Close(count);
    }

    public override void Read(ArraySegment<byte> s)
    {
        ushort count = 0;
        // ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
        count += 2;
        // ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        
        this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
        count += 8;
    }
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
                PlayerInfoReq p = new PlayerInfoReq();
                p.Read(buffer);
                Console.WriteLine($"PlayerInfoReq : {p.playerId}");
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