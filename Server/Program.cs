﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server;


    
class Program
{
    static Listener _listener = new Listener();
    static void Main(string[] args)
    {
        PacketManager.Instance.Register();
        
        // DNS(Domain Name System
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        // www.gomi.com -> 123.123.123.123

        _listener.init(endPoint, () => {
            return new ClientSession();
        });
        Console.WriteLine("Listening...");
        while (true)
        {

        }
    }
}