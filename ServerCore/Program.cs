using System;
using System.Diagnostics;
using System.Threading;

namespace ServerCore
{
    /*class Lock
    {

        // 커널을 이용하면 속도가 겁나게 느리다.
        // bool == 커널
        // AutoResetEvent _available = new AutoResetEvent(true);   // 톨게이트 개념. true면 열려있고 false면 닫혀있다.

        ManualResetEvent _available = new ManualResetEvent(true);   // 방문 개념.

        public void Acquire()
        {
            _available.WaitOne();   // 입장 시도
            _available.Reset(); // 문을 닫는다.
        }

        public void Release() 
        {
            _available.Set();   // 문을 열어준다.
        }
    }*/

    class Program
    {
        static int _num = 0;
        //static Lock _lock = new Lock();
        static Mutex _lock = new Mutex();

        static void Thread_1()
        {
            for(int i = 0; i < 1000000000; i++) 
            {
                _lock.WaitOne();
                _num++;
                _lock.ReleaseMutex();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000000; i++)
            {
                _lock.WaitOne();
                _num--;
                _lock.ReleaseMutex();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}