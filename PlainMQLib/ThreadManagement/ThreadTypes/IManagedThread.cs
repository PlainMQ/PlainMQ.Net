using PlainMQLib.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainMQLib.ThreadManagement.ThreadTypes
{
    internal interface IManagedThread
    {
        internal void ThreadAction();

        internal ManagedThreadStatus Status { get; set; }

        internal ParameterizedThreadStart? Action { get; set; }

        internal ThreadClass InvokeClass { get; set; }

        public int ID { get; set; }
        public string? Name { get; set; }
    }
}
