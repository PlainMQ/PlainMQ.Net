using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainMQLib.Models
{
    public class ServerConfig
    {
        public string? ServerPort { get; set; }
        public string? ServerAddress { get; set; }
        public string? Timeout { get; set; }
    }
}
