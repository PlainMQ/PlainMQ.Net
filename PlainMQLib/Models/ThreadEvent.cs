using PlainMQLib.Models.Enums;

namespace PlainMQLib.Models
{
    public class ThreadEvent
    {
        public ThreadClass Class { get; set; }
        public int? InitiatorID { get; set; }
        public object? EventPayload { get; set; }
    }
}
