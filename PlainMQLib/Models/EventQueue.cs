using System.Collections.Concurrent;
using System.ComponentModel;

namespace PlainMQLib.Models
{
    /// <summary>
    /// Simple event bound queue to be used in the context of the GlobalEventQueue
    /// </summary>
    /// <typeparam name="T">Type of object found in the EventQueue</typeparam>
    public class EventQueue<T>
    {
        public ConcurrentQueue<T> Queue { get; set; }

        public event CollectionChangeEventHandler QueueChange;

        public EventQueue()
        {
            Queue = new ConcurrentQueue<T>();
            QueueChange += EventQueue_QueueChange;
        }

        private void EventQueue_QueueChange(object? sender, CollectionChangeEventArgs e)
        {
            //initialization purposes only
        }

        public void Enqueue(T obj)
        {
            Queue.Enqueue(obj);
            var cArgs = new CollectionChangeEventArgs(CollectionChangeAction.Add, obj);
            QueueChange.Invoke(this, cArgs);
        }
    }
}
