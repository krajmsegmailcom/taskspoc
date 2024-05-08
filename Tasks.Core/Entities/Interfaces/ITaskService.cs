using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Core.Entities.Common;
using Tasks.Core.Entities.Enum;

namespace Tasks.Core.Entities.Interfaces
{
    public interface ITaskService
    {
        Task CreateTask(string taskName, List<string> tags, DateTime dueDate, string color, string assignedTo);
        Task UpdateTask(int taskId, string TaskName, DateTime dueDate, string color, string assignedTo,string status);
        bool DeleteTask(int taskId);
        Task GetTaskById(int taskId);
        public IEnumerable<Task> GetAll();

        public Activity AddActivity(int taskId, DateTime activityDate, string doneBy, string description);
        public Task<IEnumerable<Activity>> GetTaskActivities(int taskId);
        public Task<IEnumerable<Task>> SearchTasks(TaskSearchCriteria criteria);
        bool Validuser(string username, string password);
    }
    public class TaskSearchCriteria
    {
        public string? Taskname { get; set; }
        public List<string>? Tags { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Taskstatus>? Statuses { get; set; }
    }


}

