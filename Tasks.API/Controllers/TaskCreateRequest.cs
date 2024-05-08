using Tasks.Core.Entities;

namespace Tasks.API.Controllers
{
    public class TaskCreateRequest
    {
        public string TaskName { get; set; }
       public List<string> Tags { get; set; } // Assuming Tag is a string
        public DateTime DueDate { get; set; }
        public string Color { get; set; }
        public string AssignedTo { get; set; }
    }
}