using Newtonsoft.Json;

namespace Task.Blazor.Components.Pages
{
    public class TasksView
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("taskName")]
        public string TaskName { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("assignedTo")]
        public string AssignedTo { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
        public List<ActivityModelview> Activities { get; set; }

        public bool AreActivitiesVisible { get; set; }
    }
    public class ActivityModelview
    {
        public int Id { get; set; } // Assuming you need the activity ID for updating
        public DateTime ActivityDate { get; set; }
        public string DoneBy { get; set; }
        public string Description { get; set; }
    }
}
