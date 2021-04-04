using System;

namespace Project.Modules.Logs.Entities
{
    public class LogAccess
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string IpAddress { get; set; }
        public object Header { get; set; }
        public object Query { get; set; }
        public object Body { get; set; }
        public string Path { get; set; }
        public object Response { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
