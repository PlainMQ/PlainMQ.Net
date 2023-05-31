using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainMQLib.Models.Enums
{
    /// <summary>
    /// Lifecycle Management enum for the ManagedThreads - Currently the ERROR status is
    /// treated as an abort
    /// </summary>
    public enum ManagedThreadStatus
    {
        NEW,
        IDLE,
        RUNNING,
        ERROR
    }
}
