using System;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    S_BroadcastEnterGame = 1,
	C_LeaveGame = 2,
	S_BroadcastLeaveGame = 3,
	S_PlayerList = 4,
	C_Move = 5,
	S_BroadcastMove = 6,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

public class S_BroadcastEnterGame : IPacket
{
    public int playerId;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEnterGame; } }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(8192);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_BroadcastEnterGame);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		        count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
		        count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
		        count += sizeof(float);
        
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
        this.playerId = BitConverter.ToInt32(span.Slice(count, span.Length - count));
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(span.Slice(count, span.Length - count));
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(span.Slice(count, span.Length - count));
		count += sizeof(float);
    }
}
public class C_LeaveGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame; } }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(8192);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_LeaveGame);
        count += sizeof(ushort);

        
        
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
        
    }
}
public class S_BroadcastLeaveGame : IPacket
{
    public int playerId;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastLeaveGame; } }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(8192);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_BroadcastLeaveGame);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
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
        this.playerId = BitConverter.ToInt32(span.Slice(count, span.Length - count));
		count += sizeof(int);
    }
}
public class S_PlayerList : IPacket
{
    public class Player
	{
	    public bool isSelf;
		public int playerId;
		public float posX;
		public float posY;
	
	    public void Read(ReadOnlySpan<byte> span, ref ushort count)
	    {
	        this.isSelf = BitConverter.ToBoolean(span.Slice(count, span.Length - count));
			count += sizeof(bool);
			this.playerId = BitConverter.ToInt32(span.Slice(count, span.Length - count));
			count += sizeof(int);
			this.posX = BitConverter.ToSingle(span.Slice(count, span.Length - count));
			count += sizeof(float);
			this.posY = BitConverter.ToSingle(span.Slice(count, span.Length - count));
			count += sizeof(float);
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.isSelf);
			        count += sizeof(bool);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
			        count += sizeof(int);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
			        count += sizeof(float);
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
			        count += sizeof(float);
	        return success;
	    }
	}
	public List<Player> players = new List<Player>();

    public ushort Protocol { get { return (ushort)PacketID.S_PlayerList; } }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(8192);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_PlayerList);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.players.Count);
		count += sizeof(ushort);
		foreach (Player player in this.players)
		{
		    // TODO
		    success &= player.Write(s, ref count);
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
        this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		for (int i = 0; i < playerLen; i++)
		{
		    Player player = new Player();
		    player.Read(span, ref count);
		    players.Add(player);
		}
    }
}
public class C_Move : IPacket
{
    public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.C_Move; } }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(8192);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C_Move);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
		        count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
		        count += sizeof(float);
        
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
        this.posX = BitConverter.ToSingle(span.Slice(count, span.Length - count));
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(span.Slice(count, span.Length - count));
		count += sizeof(float);
    }
}
public class S_BroadcastMove : IPacket
{
    public int playerId;
	public float posX;
	public float posY;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastMove; } }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(8192);

        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S_BroadcastMove);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		        count += sizeof(int);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
		        count += sizeof(float);
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
		        count += sizeof(float);
        
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
        this.playerId = BitConverter.ToInt32(span.Slice(count, span.Length - count));
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(span.Slice(count, span.Length - count));
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(span.Slice(count, span.Length - count));
		count += sizeof(float);
    }
}
