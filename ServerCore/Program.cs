using System;
using System.Diagnostics;
using System.Threading;

namespace ServerCore
{
    /* 락을 표현하는 방법
     * 
     * 1. 근성 : 계속 문을 열기를 시도하는 것
     * 2. 양보 : 조금 있다가 다시 오겠다.
     * 3. 갑질 : 나 대신 너가 이거 문 열리는 지 보고 있어라.
     * 
     * 스핀락 : 근성과 양보를 서로서로 사용한다.
     * 뮤텍스 : 커널을 이용해서 스핀락보다는 느리다.
     * 
     * RWLock(ReaderWriterLock)
     */


    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}