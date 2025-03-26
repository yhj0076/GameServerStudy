using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient;

class PlayerInfoReq
{
    public long playerId;
    public string name;

    public struct SkillInfo
    {
        public int id;
        public short level;
        public float duration;

        public bool Write(Span<byte> s, ref ushort count)
        {
            bool success = true;
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
            count += sizeof(int);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
            count += sizeof(short);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
            count += sizeof(float);
            return success;
        }

        public void Read(ReadOnlySpan<byte> s, ref ushort count)
        {
            id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
            count += sizeof(int);
            level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
            count += sizeof(short);
            duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
            count += sizeof(float);
        }
    }
    
    public List<SkillInfo> skills = new List<SkillInfo>();

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
        count += sizeof(long);
        
        // string
        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
        count += sizeof(ushort);
        count += nameLen;
        
        // skill list
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
        count += sizeof(ushort);
        foreach (SkillInfo skill in skills)
        {
            // TODO
            success &= skill.Write(s, ref count);
        }
        
        
        success &= BitConverter.TryWriteBytes(s, count);
        
        if (!success)
            return null;
        
        return SendBufferHelper.Close(count);
    }

    public void Read(ArraySegment<byte> s)
    {
        ushort count = 0;
        ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(s.Array, s.Offset, s.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt64(span.Slice(count, span.Length - count));
        count += sizeof(long);
        
        // string
        ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
        count += sizeof(ushort);
        this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
        count += nameLen;
        
        // skill list
        skills.Clear();
        ushort skillLen = BitConverter.ToUInt16(s.Slice(count, span.Length - count));
        count += sizeof(ushort);
        for (int i = 0; i < skillLen; i++)
        {
            SkillInfo skill = new SkillInfo();
            skill.Read(s, ref count);
            skills.Add(skill);
        }
    }
}

// class PlaerInfoOk : Packet
// {
//     public int hp;
//     public int attack;
// }

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
        
        PlayerInfoReq packet = new PlayerInfoReq(){playerId = 1001, name = "ABCD"};
        packet.skills.Add(new PlayerInfoReq.SkillInfo() {id = 101,  level = 1,duration = 3.0f});
        packet.skills.Add(new PlayerInfoReq.SkillInfo() {id = 201,  level = 2,duration = 4.0f});
        packet.skills.Add(new PlayerInfoReq.SkillInfo() {id = 301,  level = 3,duration = 5.0f});
        packet.skills.Add(new PlayerInfoReq.SkillInfo() {id = 401,  level = 4,duration = 6.0f});
        // 보낸다
        // for (int i = 0; i < 5; i++)
        {
            ArraySegment<byte> s = packet.Write();
            if(s != null)
                Send(s);
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
