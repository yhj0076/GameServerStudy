using Server;
using ServerCore;

public class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
        {
            return;
        }
        
        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Leave(clientSession)
            );
    }
    
    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move move = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
        {
            return;
        }

        // Console.WriteLine(move.posX + " " + move.posY);
        
        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Move(clientSession, move)
        );
    }
}