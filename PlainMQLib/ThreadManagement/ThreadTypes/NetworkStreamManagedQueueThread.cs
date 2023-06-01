using PlainMQLib.Models;
using System.Net.Sockets;

namespace PlainMQLib.ThreadManagement.ThreadTypes
{
    /// <summary>
    /// Specialization of a ManagedQueueThread which has a NetworkStream as a property.
    /// 
    /// Desired functionality is to write to the resident NetworkStream
    /// </summary>
    internal class NetworkStreamManagedQueueThread : ManagedQueueThread, IDisposable
    {
        internal NetworkStream NStream { get; set; }

        internal NetworkStreamManagedQueueThread(NetworkStream nStream, bool isWriter)
        {
            NStream = nStream;
            InvokeClass = isWriter ? Models.Enums.ThreadClass.BROADCAST : Models.Enums.ThreadClass.READER;
            Action = (o) =>
            {
                ThreadMethod();
            };

            _thread = new Thread(() => Action.Invoke(nStream));
            _thread.Start();
        }

        private void ThreadMethod()
        {
            while (Status != Models.Enums.ManagedThreadStatus.ERROR)
            {
                if (LocalQueue.Any())
                {
                    var obj = LocalQueue.Dequeue();

                    if (obj.EventPayload is PlainMessage msg)
                    {
                        QueueAction?.Invoke(obj);
                    }
                }
                else
                {
                    Status = Models.Enums.ManagedThreadStatus.IDLE;
                    Thread.Sleep(10);
                }
            }
        }

        public void Dispose()
        {
            NStream?.Dispose();
        }
    }
}
