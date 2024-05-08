using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Task.Blazor.Components.Pages
{
    public class TaskModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("taskName")]
        public string TaskName { get; set; }

        [JsonProperty("tags")]
        public List<TagItem> Tags { get; set; } = new List<TagItem>();

        [JsonProperty("dueDate")]
        public DateTime DueDate { get; set; } = DateTime.Today;

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("assignedTo")]
        public string AssignedTo { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
        public List<ActivityModel> Activities { get; set; }
    }
    public class ActivityModel
    {
        public int Id { get; set; } // Assuming you need the activity ID for updating
        public DateTime ActivityDate { get; set; } = DateTime.Today;
        public string DoneBy { get; set; }
        public string Description { get; set; }
    }
    public class TagItem
    {
        public string Value { get; set; }
    }
}
