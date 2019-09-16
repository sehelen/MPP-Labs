using System.Threading;

namespace laba_3
{
    class Mutex
    {
        private int locked = 1;

        public void Lock()
        {
            while (Interlocked.CompareExchange(ref locked, 0, 1) != 1)
            {
                Thread.Sleep(10);
            }
        }

        public void Unlock()
        {
            Interlocked.Exchange(ref locked, 1);
        }
    }
}
