using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace laba_2
{
    class Program
    {
        private static int copiedFilesCount = 0;
        private static string destinationDirectory;
        private static string sourceDirectory;
        private static TaskQueue _TaskQueue = null;
        private static Action<object> actCopy;
        private static Action<object> actCopyDirectory;
        private static Action<object> actCopyFile;

        static void Main(string[] args)
        {
            sourceDirectory = @"D:\1"; //args[0];
            destinationDirectory = @"D:\2"; //args[1];
           
            CopyInfo copyInfo = new CopyInfo()
            {
                DirectoryName = destinationDirectory,
                Filename = string.Empty,
                SourceFilename = string.Empty,
                SourceDirectoryName = sourceDirectory
            };

            actCopy = (object obj) =>
            {
                Copy(obj);
            };
            actCopyDirectory = (object obj) =>
            {
                CopyDirectory(obj);
            };
            actCopyFile = (object obj) =>
            {
                CopyFile(obj);
            };


            try
            {
                _TaskQueue = new TaskQueue(3);

                if (Directory.Exists(copyInfo.Source))
                {
                    CreateDestinationDirectory(copyInfo.Destination);
                    Task task = new Task(actCopy, copyInfo);
                    _TaskQueue.EnqueueTask(task);
                }

                Thread.Sleep(1000);
                //Task[] taskArr = _TaskQueue.tasks.ToArray<Task>();
                //Task.WaitAll(taskArr);
                Console.WriteLine("Copied files: " + copiedFilesCount);
                _TaskQueue.Wait();

            }
            finally
            {
                if (_TaskQueue != null)
                {
                    _TaskQueue.Dispose();
                }
            }
        }


        private static void CreateDestinationDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }


        private static void Copy(object state)
        {
            CopyInfo copyInfo = state as CopyInfo;
            if (Directory.Exists(copyInfo.Destination))
            {
                Task task = new Task(actCopyDirectory, copyInfo); 
                _TaskQueue.EnqueueTask(task);
            }
            else
            {
                Task task = new Task(actCopyFile, copyInfo);
                _TaskQueue.EnqueueTask(task);
            }
        }

        private static void CopyFile(object state)
        {
            CopyInfo copyInfo = state as CopyInfo;
            File.Copy(copyInfo.Source, copyInfo.Destination);
            Interlocked.Increment(ref copiedFilesCount);
        }

        private static void CopyDirectory(object state)
        {
            CopyInfo copyInfo = state as CopyInfo;
            CreateDestinationDirectory(copyInfo.Destination);
            foreach (string dir in Directory.GetDirectories(copyInfo.Source))
            {
                string d = dir.Replace(copyInfo.SourceDirectoryName, string.Empty);
                d = d.Substring(1);
                CopyInfo newCopyInfo = new CopyInfo()
                {
                    DirectoryName = copyInfo.Destination,
                    Filename = d,
                    SourceFilename = string.Empty,
                    SourceDirectoryName = dir
                };
                Task task = new Task(actCopyDirectory, newCopyInfo);
                _TaskQueue.EnqueueTask(task);
            }
            foreach (string file in Directory.GetFiles(copyInfo.Source))
            {
                string f = file.Replace(copyInfo.SourceDirectoryName, string.Empty);
                f = f.Substring(1);
                CopyInfo newCopyInfo = new CopyInfo()
                {
                    DirectoryName = copyInfo.Destination,
                    Filename = f,
                    SourceFilename = f,
                    SourceDirectoryName = copyInfo.SourceDirectoryName
                };
                newCopyInfo.SourceFilename = newCopyInfo.Filename;
                Task task = new Task(actCopy, newCopyInfo);
                _TaskQueue.EnqueueTask(task);
            }
        }

    }
}
    

