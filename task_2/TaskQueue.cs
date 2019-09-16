using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace laba_2
{
    class TaskQueue : IDisposable
    {
        public ConcurrentQueue<Task> tasks;
        private List<Thread> threads;

        public TaskQueue(int count)
        {
            tasks = new ConcurrentQueue<Task>();
            threads = new List<Thread>();

            Thread thread;
            for (int i = 0; i < count; i++)
            {
                thread = new Thread(new ThreadStart(Work));
                threads.Add(thread);
                threads[i].Start();
            }
        }

        public void EnqueueTask(Task task)
        {
            tasks.Enqueue(task);
        }

        private void Work()
        {   
            while(true)
            {
                try
                {
                    Task task;
                    while (!tasks.TryDequeue(out task))
                      Thread.Sleep(100);
                    task.Start();
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }         
        }

        public void Wait()
        {
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
            this.CloseThreads();
        }

    }
}
