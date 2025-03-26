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

class PlayerInfoReq
{
    
	public long playerId;
	
	public string name;
	
	public struct Skill
	{
	    
		public int id;
		
		public short level;
		
		public float duration;
	
	    public void Read(ReadOnlySpan<byte> span, ref ushort count)
	    {
	        
			this.id = BitConverter.ToInt32(span.Slice(count, span.Length - count));
			            count += sizeof(int);
			
			this.level = BitConverter.ToInt16(span.Slice(count, span.Length - count));
			            count += sizeof(short);
			
			this.duration = BitConverter.ToSingle(span.Slice(count, span.Length - count));
			            count += sizeof(float);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
			        count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
			        count += sizeof(short);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
			        count += sizeof(float);
	        return success;
	    }
	}
	public List<Skill> skills = new List<Skill>();

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
		
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
		count += sizeof(ushort);
		count += nameLen;
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.skills.Count);
		count += sizeof(ushort);
		foreach (Skill skill in this.skills)
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
		
		ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += nameLen;
		
		this.skills.Clear();
		ushort skillLen = BitConverter.ToUInt16(s.Slice(count, span.Length - count));
		count += sizeof(ushort);
		for (int i = 0; i < skillLen; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(s, ref count);
		    skills.Add(skill);
		}
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
                Console.WriteLine($"PlayerInfoReq : {p.playerId} {p.name}");

                foreach (var skill in p.skills)
                {
                    Console.WriteLine($"Skill({skill.id})({skill.level})({skill.duration})");
                }
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