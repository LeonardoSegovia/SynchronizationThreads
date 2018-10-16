using System;
using System.Threading;

namespace SynchronizationThreads
{
    class Program
    {
        // shared field for work result
        public static int Result = 0;

        // lock handle for shared result
        private static readonly object _lockHandle = new object();

        // event wait handles
        public static EventWaitHandle ReadyForResult = new AutoResetEvent(false);
        public static EventWaitHandle SetResult = new AutoResetEvent(false);
        

        public static void DoWork()
        {
            while (true)
            {
                var i = Result;

                // simulate long calculation
                Thread.Sleep(1);


                // wait until main loop is ready to receive result
                ReadyForResult.WaitOne();
                

                // return result
                lock (_lockHandle)
                {
                    Result = i + 1;
                }

                // tell main loop that we set the result
                SetResult.Set();

            }
        }

        static void Main(string[] args)
        {
            // start the thread
            var t = new Thread(DoWork);
            t.Start();

            // collect result every 10 milliseconds
            for (var i = 0; i < 100; i++)
            {
                #region ...
                // tell thread that we're ready to receive the result
                ReadyForResult.Set();
                #endregion

                #region ...
                // wait until thread has set the result
                SetResult.WaitOne();
                #endregion

                lock (_lockHandle)
                {
                    Console.WriteLine(Result);
                }

                // simulate other work
                Thread.Sleep(10);
            }

            // messy abort
            t.Abort();
        }
    }
}
