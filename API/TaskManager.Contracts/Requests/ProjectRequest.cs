
namespace TaskManager.Contracts.Requests
{
    public class ProjectRequest
    {
        public ProjectRequest()
        {
            Name = string.Empty;
        }
        public string Name { get; set; }
        public long DeveloperId { get; set; } = 1;
        public DateTime? DeadLine { get; set; }
    }
}
