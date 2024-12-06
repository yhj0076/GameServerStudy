using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                // 받는다.
                byte[] buffer = new byte[1024];
                int recvBytes = clientSocket.Receive(buffer);
                string recvData = Encoding.UTF8.GetString(buffer, 0, recvBytes);
                Console.WriteLine($"[From Client] {recvData}");

                // 보낸다.
                byte[] buffer2 = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
                clientSocket.Send(buffer2);

                // 쫓아낸다.
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Main(string[] args)
        {
            // DNS(Domain Name System
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            // www.gomi.com -> 123.123.123.123

            // 문지기
            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listener.init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");
            while (true)
            {

            }
        }
    }
}