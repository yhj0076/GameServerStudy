using System;
using System.Diagnostics;
using System.Threading;

namespace ServerCore
{
    class SessionManager
    {
        static object _lock = new object();
        
        public static void TestSession()
        {
            lock(_lock) { }
        }

        public static void Test()
        {
            UserManager.TestUser();
        }
    }

    class UserManager
    {
        static object _lock = new Object();

        public static void Test()
        {
            lock(_lock) 
            {
                SessionManager.TestSession();
            }
        }

        public static void TestUser()
        {
            lock(_lock) { }
        }
    }

    class Program
    {
        static int number = 0;
        static object _obj = new object();

        static void Thread_1()
        {
            // atomic = 원자성

            // Interlock
            // 원자성을 보장해 줄 수 있는 함수.
            // 성능에서 손해를 보게 된다.
            // 순서가 보장된다.
            // 정수형 밖에 다룰 수 없다.

            // Monitor
            // Mutual Exclusive(상호 배제)를 진행해주는 클래스.
            // 관리하기 어려워진다.
            // Enter는 Lock, Exit은 Unlock이다.
            // Lock에 관한 자세한 설명은 운영체제론 참조.

            for(int i = 0; i < 10000; i++)
            {
                lock(_obj) 
                {
                    SessionManager.Test();
                }
                // 위 코드는 하단 코드(try-finally문)을 깔끔하게 처리하는 방법이다.
                /*
                try
                {
                    Monitor.Enter(_obj);
                    number++;
                }
                finally
                {
                    Monitor.Exit(_obj);
                }*/
                // 위 코드는 하단 코드(if문)를 깔끔하게 처리하는 방법이다.
                /*
                Monitor.Enter(_obj);
                {
                    number++;

                    if(number == 10000)
                    {
                        Monitor.Exit(_obj);
                        return;
                    }
                }
                Monitor.Exit(_obj);
                */
            } 
        }

        // Deadlock(데드락)
        // 잠가놓고 안 열어서 더 이상 사용할 수 없는 자물쇠를 일컫는다.
        // 주로 교착 상태에서 일어난다.
        static void Thread_2()
        {
            for (int i = 0; i < 10000; i++)
            {
                lock (_obj)
                {
                    UserManager.Test();
                }
            }
        }

        static void Main(string[] args)
        {
             

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();

            Thread.Sleep(100);

            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}