using PlainMQLib.Models.Enums;

namespace PlainMQLib.Models
{
    internal class ThreadEvent
    {
        internal ThreadClass Class { get; set; }
        internal int? InitiatorID { get; set; }
        internal object? EventPayload { get; set; }
    }
}
