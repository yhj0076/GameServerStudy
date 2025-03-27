using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server;

public enum PacketID
{
    PlayerInfoReq = 1,
	Test = 2,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

class PlayerInfoReq : IPacket
{
    public byte testByte;
	public long playerId;
	public string name;
	public class Skill
	{
	    public int id;
		public short level;
		public float duration;
		public class Attribute
		{
		    public int att;
		
		    public void Read(ReadOnlySpan<byte> span, ref ushort count)
		    {
		        this.att = BitConverter.ToInt32(span.Slice(count, span.Length - count));
				count += sizeof(int);
		    }
		
		    public bool Write(Span<byte> s, ref ushort count)
		    {
		        bool success = true;
		        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.att);
				        count += sizeof(int);
		        return success;
		    }
		}
		public List<Attribute> attributes = new List<Attribute>();
	
	    public void Read(ReadOnlySpan<byte> span, ref ushort count)
	    {
	        this.id = BitConverter.ToInt32(span.Slice(count, span.Length - count));
			count += sizeof(int);
			this.level = BitConverter.ToInt16(span.Slice(count, span.Length - count));
			count += sizeof(short);
			this.duration = BitConverter.ToSingle(span.Slice(count, span.Length - count));
			count += sizeof(float);
			this.attributes.Clear();
			ushort attributeLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
			count += sizeof(ushort);
			for (int i = 0; i < attributeLen; i++)
			{
			    Attribute attribute = new Attribute();
			    attribute.Read(span, ref count);
			    attributes.Add(attribute);
			}
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
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.attributes.Count);
			count += sizeof(ushort);
			foreach (Attribute attribute in this.attributes)
			{
			    // TODO
			    success &= attribute.Write(s, ref count);
			}
	        return success;
	    }
	}
	public List<Skill> skills = new List<Skill>();

    public ushort Protocol { get { return (ushort)PacketID.PlayerInfoReq; } }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq);
        count += sizeof(ushort);

        segment.Array[segment.Offset + count] = (byte)this.testByte;
		count += sizeof(byte);
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
        this.testByte = (byte)s.Array[s.Offset + count];
		count += sizeof(byte);
		this.playerId = BitConverter.ToInt64(span.Slice(count, span.Length - count));
		count += sizeof(long);
		ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
		count += nameLen;
		this.skills.Clear();
		ushort skillLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		for (int i = 0; i < skillLen; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(span, ref count);
		    skills.Add(skill);
		}
    }
}
class Test : IPacket
{
    public int testInt;

    public ushort Protocol { get { return (ushort)PacketID.Test; } }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.Test);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.testInt);
		        count += sizeof(int);
        
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
        this.testInt = BitConverter.ToInt32(span.Slice(count, span.Length - count));
		count += sizeof(int);
    }
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