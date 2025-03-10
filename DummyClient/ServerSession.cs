using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient;

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

class ServerSession : Session
{

    /*static unsafe void ToBytes(byte[] array, int offset, ulong value)
    {
        fixed(byte* ptr = &array[offset])
            *(ulong*)ptr = value;
    }*/
    
    public override void OnConnected(EndPoint endpoint)
    {
        Console.WriteLine($"OnConnected : {endpoint}");
        
        PlayerInfoReq packet = new PlayerInfoReq(){packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001};
        
        // 보낸다
        // for (int i = 0; i < 5; i++)
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;
            
            
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId);
            count += 8;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);
            
            
            /*byte[] size = BitConverter.GetBytes(packet.size);   // 2
            byte[] packetId = BitConverter.GetBytes(packet.packetId);   // 2
            byte[] playerId = BitConverter.GetBytes(packet.playerId);   // 8
            
            Array.Copy(size, 0, s.Array,s.Offset + count, 2);
            count += 2;
            Array.Copy(packetId, 0, s.Array,s.Offset + count, 2);
            count += 2;
            Array.Copy(playerId, 0, s.Array,s.Offset + count, 8);
            count += 8;*/
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);
            
            if(success)
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
