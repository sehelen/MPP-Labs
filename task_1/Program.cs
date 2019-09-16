using System;
using System.Threading;

namespace laba_1
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskQueue _TaskQueue = null;
            try
            {
                _TaskQueue = new TaskQueue(3);

                for (int i = 0; i < 50; i++)
                {
                    _TaskQueue.EnqueueTask(SomeTask);
                }
                _TaskQueue.Wait();
            }
            finally
            {
                if(_TaskQueue != null)
                {         
                   _TaskQueue.Dispose();
                }
            }
                                    
        }

        static public void SomeTask()
        {
            Console.WriteLine(Thread.CurrentThread.Name);
        }
        
    }
}
