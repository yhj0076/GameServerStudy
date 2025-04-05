namespace DummyClient;

public class SessionManager
{
    static SessionManager _session = new SessionManager();
    public static SessionManager Instance { get { return _session; } }
    
    List<ServerSession> _sessions = new List<ServerSession>();
    object _lock = new object();
    Random _random = new Random();
    public void SendForEach()
    {
        lock (_lock)
        {
            foreach (ServerSession session in _sessions)
            {
                C_Move movePacket = new C_Move();
                movePacket.posX = _random.Next(-20, 20);
                movePacket.posY = _random.Next(-20, 20);
                session.Send(movePacket.Write());
            }
        }
    }
    
    public ServerSession Generate()
    {
        lock (_lock)
        {
            ServerSession session = new ServerSession();
            _sessions.Add(session);
            return session;
        }
    }
}