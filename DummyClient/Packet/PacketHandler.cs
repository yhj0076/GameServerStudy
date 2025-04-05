using DummyClient;
using ServerCore;

public class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame chatPacket = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;
    }
    
    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame chatPacket = (S_BroadcastLeaveGame)packet;
        ServerSession serverSession = session as ServerSession;
    }
    
    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList chatPacket = (S_PlayerList)packet;
        ServerSession serverSession = session as ServerSession;
    }
    
    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove chatPacket = (S_BroadcastMove)packet;
        ServerSession serverSession = session as ServerSession;
    }
}