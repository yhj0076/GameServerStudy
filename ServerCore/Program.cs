using System;
using System.Diagnostics;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => { return $"My Name Is {Thread.CurrentThread.ManagedThreadId}"; });
        static void WhoAmI()
        {
            bool repeat = ThreadName.IsValueCreated;
            if (repeat) 
            {
                Console.WriteLine(ThreadName.Value + "{repeat}");
            } else
            {
                Console.WriteLine(ThreadName.Value);
            }
            Console.WriteLine(ThreadName.Value);
        }
        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);

            ThreadName.Dispose();
        }
    }
}