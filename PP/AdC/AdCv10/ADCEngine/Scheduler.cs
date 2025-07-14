//#define USE_THREADS
#if USE_THREADS
#warning USE_THREADS Flag Activated
#endif

using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace ADCEngine
{
    public class Scheduler
    {
        private static int _nbTasksPerPool = 8;
        public static int GetNbTasksPerPool() { return _nbTasksPerPool; }

        private static int _totalNbTasks = 0;

        static Scheduler()
        {
            _nbTasksPerPool = Int32.Parse(ConfigurationManager.AppSettings["AdcEngine.NbTasksPerPool"]);
        }

#if USE_THREADS
        //=================================================================
        //
        //=================================================================
        public static Task StartTaskPool(string name, int nb_threads, System.Action action)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Thread[] threads = new Thread[nb_threads];

                for (int i = 0; i < nb_threads; i++)
                {
                    Thread t = new Thread(() => action.Invoke());
                    t.Priority = ThreadPriority.BelowNormal;
                    t.Name = name + "-" + i;
                    t.Start();
                    threads[i] = t;
                    Interlocked.Increment(ref _totalNbTasks);
                }

                foreach (Thread t in threads)
                    t.Join();
            });

            return task;
        }

        //=================================================================
        //
        //=================================================================
        public static Task StartTaskPool(string name, System.Action action)
        {
            Task task = StartTaskPool(name, _nbTasksPerPool, action);
            return task;
        }

        //=================================================================
        //
        //=================================================================
        public static Task StartSingleTask(System.Action action)
        {
            Task task = StartTaskPool("noname", 1, action);
            return task;
        }

#else
        //=================================================================
        //
        //=================================================================
        public static Task StartTaskPool(string name, System.Action action)
        {
            Task[] tasks = new Task[_nbTasksPerPool];
            for (int i = 0; i < _nbTasksPerPool; i++)
                tasks[i] = StartSingleTask(name + "-" + i, action);

            Task task = StartSingleTask(name + "-w", () => Task.WaitAll(tasks));
            return task;
        }

        //=================================================================
        //
        //=================================================================
        public static Task StartSingleTask(string name, System.Action action)
        {
            int x = Interlocked.Increment(ref _totalNbTasks);
            //Console.WriteLine("Scheduler creating task #" + x);
            return Task.Factory.StartNew(
                    () => { Thread.CurrentThread.Name = name; action.Invoke(); },
                    TaskCreationOptions.LongRunning
                );
        }
#endif

    }
}
