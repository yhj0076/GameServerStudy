using System;
using System.Diagnostics;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        static int number = 0;

        static void Thread_1()
        {
            // atomic = 원자성

            // Interlock : 원자성을 보장해 줄 수 있는 함수. 성능에서 손해를 보게 된다. 순서가 보장된다.
            for(int i = 0; i < 1000000; i++)
            {
                // All or Nothing
                Interlocked.Increment(ref number);  // ref -> number의 주소값을 참조한다.
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