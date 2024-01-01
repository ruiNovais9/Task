
using System.Runtime.Serialization;

namespace TaskManager.Contracts
{
    public class Project
    {
        public Project()
        {
            Name = string.Empty;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public long DeveloperId { get; set; } = 1;
        public DateTime? DeadLine { get; set; }
        public long TimeSpend { get; set; }
        public bool ProjectIsCompleted { get; set; }
    }
}
