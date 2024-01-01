
namespace TaskManager.Contracts.Responses
{
    public class ProjectResponse
    {
        public ProjectResponse()
        {
            Projects = new List<Project>();
            IsSucess = false;
            Error = null;
        }
        public List<Project> Projects { get; set; }
        public bool IsSucess {  get; set; }
        public Error? Error { get; set; }
    }

    public class Error
    {
        public Error()
        {
            Message = string.Empty;
        }
        public string Message { get; set; }
    }
}
