namespace Task.Blazor.Components.Pages
{
    public class TaskSearchCriteria
    {
        public string? Taskname { get; set; }
        public List<string>? Tags { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Taskstatus>? Statuses { get; set; }
    }
    public enum Taskstatus
    {
        Pending = 0,
        Completed = 1,
        In_Progress = 2
        // Add more statuses as needed
    }
}
