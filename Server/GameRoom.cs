using ServerCore;

namespace Server;

public class GameRoom : IJobQueue
{
    List<ClientSession> _sessions = new List<ClientSession>();
    JobQueue _jobQueue = new JobQueue();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
    
    public void Push(Action job)
    {
        _jobQueue.Push(job);
    }

    public void Flush()
    {
        foreach (ClientSession s in _sessions)
        {
            s.Send(_pendingList);
        }

        // Console.WriteLine($"Flushed {_pendingList.Count} items");
        _pendingList.Clear();
    }
    
    public void Broadcast(ArraySegment<byte> segment)
    {
        _pendingList.Add(segment);
    }
    
    public void Enter(ClientSession session)
    {
        // 플레이어 추가하고
        _sessions.Add(session);
        session.Room = this;
        
        // 신입생한테 모든 플레이어 목록 전송
        S_PlayerList playerList = new S_PlayerList();
        foreach (var s in _sessions)
        {
            playerList.players.Add(new S_PlayerList.Player()
            {
                isSelf = (s == session),
                playerId = s.SessionId,
                posX = s.PosX,
                posY = s.PosY,
            });
        }
        session.Send(playerList.Write());
        

        // 신입생 입장을 모두에게 알린다
        S_BroadcastEnterGame enter =  new S_BroadcastEnterGame();
        enter.playerId = session.SessionId;
        enter.posX = 0;
        enter.posY = 0;
        Broadcast(enter.Write());
    }

    public void Leave(ClientSession session)
    {
        // 플레이어 제거하고
        _sessions.Remove(session);
        
        // 모두에게 알린다
        S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
        leave.playerId = session.SessionId;
        Broadcast(leave.Write());
    }

    public void Move(ClientSession session, C_Move? packet)
    {
        // 좌표 바꿔주고
        session.PosX = packet.posX;
        session.PosY = packet.posY;
        
        // 모두에게 알린다
        S_BroadcastMove move = new S_BroadcastMove();
        move.playerId = session.SessionId;
        move.posX = packet.posX;
        move.posY = packet.posY;
        Broadcast(move.Write());
    }
}