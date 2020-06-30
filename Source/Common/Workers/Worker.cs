using System;
using System.Threading;

namespace VoxCake.Networking
{
    public abstract class Worker
    {
        private Thread _workerThread;
        protected abstract Action Work { get; }

        protected void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }
        
        public void StartWork()
        {
            _workerThread = new Thread(() =>
            {
                Work?.Invoke();
            });
            _workerThread.Start();
        }

        public void StopWork()
        {
            _workerThread.Abort();
            _workerThread = null;
        }
    }
}