using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace laba_1
{
    class TaskQueue : IDisposable
    {
        public delegate void TaskDelegate();
        private ConcurrentQueue<TaskDelegate> tasks;
        private List<Thread> threads;
        private bool isWork = true;

        public TaskQueue(int count)
        {
            tasks = new ConcurrentQueue<TaskDelegate>();
            threads = new List<Thread>();

            Thread thread;
            for (int i = 0; i < count; i++)
            {
                thread = new Thread(new ThreadStart(Work));
                thread.Name = "Thread " + i.ToString();
                threads.Add(thread);
                threads[i].Start();
            }
        }

        public void EnqueueTask(TaskDelegate task)
        {
            tasks.Enqueue(task);
        }

        private void Work()
        {   
            while(isWork)
            {
                try
                {
                    TaskDelegate task;
                    while (!tasks.TryDequeue(out task))
                       Thread.Sleep(10);
                    task();
                }
                catch (Exception exc)
                {
                   
                }
            }         
        }

        public void Wait()
        {
            /*
            lock(Locker)
            {
                while (!tasks.IsEmpty)
                {
                    Thread.Sleep(100);
                }
                isWork = false;
            }
            */
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            
        }

        public void CloseThreads()
        {
            foreach (Thread thread in threads)
            {
                thread.Interrupt();
            }
        }

        public void Dispose()
        {
            CloseThreads();
            GC.SuppressFinalize(this);
        }

        ~TaskQueue()
        {
            CloseThreads();
        }

    }
}
