#if NET35
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Artemis.Utils
{
    static class _35Async
    {
        public static void ParallelForEach<T>(IEnumerable<T> items, Action<T> operation)
        {
            int tasks = 0; // keep track of number of active tasks
            object locker = new object(); // synchronization object

            foreach (var item in items)
            {
                lock (locker) tasks++;
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    operation((T)o);

                    lock (locker)
                    {
                        tasks--;
                        Monitor.Pulse(locker);
                    }
                }, item);
            }

            // wait for all tasks to finish
            lock (locker)
            {
                while (tasks > 0)
                    Monitor.Wait(locker);
            }
        }
    }
}
#endif