using System;
using System.Diagnostics;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        static volatile int number = 0;

        static void Thread_1()
        {
            // atomic = 원자성

            // Interlock : 원자성을 보장해 줄 수 있지만 성능에서 손해를 보게 된다
            for(int i = 0; i < 1000000; i++)
            {
                Interlocked.Increment(ref number);
            } 
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                Interlocked.Decrement(ref number);
            }
        }

        static void Main(string[] args)
        {
             

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}