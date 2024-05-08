
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Core.Entities;
using Tasks.Core.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using Tasks.Core.Entities.Enum;


namespace Tasks.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<Core.Entities.Task> _taskRepository;
        private readonly IEmailService _emailService;

        public TaskService(IRepository<Core.Entities.Task> taskRepository, IEmailService emailService)
        {
            _taskRepository = taskRepository;
            _emailService = emailService;
        }


        public Core.Entities.Task UpdateTask(int taskId, string TaskName, DateTime dueDate, string color, string assignedTo, string status)
        {
            var existingTask = _taskRepository.GetById(taskId);
            if (existingTask != null)
            {
                existingTask.TaskName = TaskName;
                existingTask.DueDate = dueDate;
                existingTask.Color = color;
                existingTask.AssignedTo = assignedTo;
                existingTask.Status = (Taskstatus)Enum.Parse(typeof(Taskstatus), status);
                _taskRepository.Update(existingTask);
            }
            return existingTask; // Return the updated task
        }


        public Core.Entities.Task CreateTask(string taskName, List<string> tags, DateTime dueDate, string color, string assignedTo)
        {
            var newTask = new Core.Entities.Task
            {
                TaskName = taskName,
                Tags = tags.ConvertAll(tag => tag), // Convert List<Tag> to List<string>
                DueDate = DateTime.UtcNow,
                Color = color,
                AssignedTo = assignedTo,
                Status = 0
            };

            _taskRepository.Add(newTask);
            var message = "";
            try
            {
                // Send email notification
                _emailService.SendEmail(assignedTo, "New Task Assigned", $"You have been assigned a new task: {taskName}");
                message = "Email notification sent successfully";
            }
            catch (Exception ex)
            {
                message = "Failed to send email notification";
                // Optionally, you can log the error or perform any other error handling here
            }

            return newTask;
        }






        public Core.Entities.Task GetTaskById(int taskId)
        {
            var task = _taskRepository.GetById(taskId);

            // Check if the task is null
            if (task == null)
            {
                // Handle the case where the task with the given ID is not found
                // For now, let's return null
                return null;
            }

            // Return the retrieved task
            return task;
        }






        public bool DeleteTask(int taskId)
        {
            var deletedTask = _taskRepository.Delete(taskId);
            return deletedTask;

        }

        public Core.Entities.Activity AddActivity(int taskId, DateTime activityDate, string doneBy, string description)
        {
            return _taskRepository.AddActivity(taskId, activityDate, doneBy, description);
        }

        public Task<IEnumerable<Activity>> GetTaskActivities(int taskId)
        {
            return _taskRepository.GetTaskActivities(taskId);
        }



        public IEnumerable<Core.Entities.Task> GetAll()
        {
            return _taskRepository.GetAll();
        }

        public Task<IEnumerable<Core.Entities.Task>> SearchTasks(TaskSearchCriteria criteria)
        {
            return _taskRepository.SearchTasks(criteria);
        }

        bool ITaskService.Validuser(string username, string password)
        {
            return _taskRepository.validate(username, password);
        }
    }
}
