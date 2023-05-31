using PlainMQLib.Models;
using PlainMQLib.Models.Enums;
using PlainMQLib.ThreadManagement.ThreadPool;
using System.ComponentModel;

namespace PlainMQLib.ThreadManagement.ThreadTypes
{
    /// <summary>
    /// Base Class for ManagedThreads containing a ThreadAction which is executable on the
    /// specified event on the global event queue.
    /// 
    /// Execution of the ThreadAction is dependent on whether the class of the GlobalEventQueue entry 
    /// matches the class of the created ManagedThread
    /// </summary>
    public class ManagedThreadBase : IManagedThread
    {
        public ManagedThreadStatus Status { get; set; }

        public ParameterizedThreadStart? Action { get; set; }

        public ThreadClass InvokeClass { get; set; }

        public ThreadClass CancelClass { get; } = ThreadClass.TERMINATE;

        public int ID { get; set; }
        public string? Name { get; set; }

        internal Thread? _thread { get; set; }

        public virtual void ThreadAction()
        {
            ManagedThreadPool.GlobalEventQueue.QueueChange += (object? sender, CollectionChangeEventArgs args) =>
            {
                if (args.Element == null)
                    throw new InvalidDataException("Failed to subscribe to GlobalEventQueue");

                ThreadEvent ubEvent = (ThreadEvent)args.Element;

                if (ubEvent.Class == InvokeClass)
                {
                    _thread = new Thread(() => Action?.Invoke(ubEvent));
                    _thread.Start();
                }
            };

        }
    }
}
