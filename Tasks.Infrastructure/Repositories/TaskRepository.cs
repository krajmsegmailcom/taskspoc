using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tasks.Core.Entities.Interfaces;
using Tasks.Infrastructure.Data;
using TaskEntity = Tasks.Core.Entities.Task; // Alias for clarity
using Activity = Tasks.Core.Entities.Activity;
using Tasks.Core.Entities.Enum;
namespace Tasks.Infrastructure.Repositories
{
    public class TaskRepository : IRepository<TaskEntity>
    {
        private readonly TasksDbContext _context;

        public TaskRepository(TasksDbContext context)
        {
            _context = context;
        }



        public void Add(TaskEntity entity)
        {
            try
            {
                _context.Set<TaskEntity>().Add(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                Console.WriteLine($"An error occurred while adding the entity: {ex.Message}");
                throw; // Rethrow the exception to propagate it
            }

        }

        public void Update(TaskEntity entity)
        {
            _context.Set<TaskEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }


        public async Task<TaskEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TaskEntity>().FindAsync(id)
            ;
        }

        // Method to search tasks by name
        public IEnumerable<TaskEntity> SearchByName(string searchTerm)
        {
            return _context.Set<TaskEntity>()
     .Where(t => EF.Functions.Like(t.TaskName, $"%{searchTerm}%"))
     .ToList();

        }

        // Method to search tasks by tags
        public IEnumerable<TaskEntity> SearchByTags(IEnumerable<string> tags)
        {
            // Filter tasks where all specified tags are present in the task's tags
            return _context.Set<TaskEntity>()
                .Where(t => tags.All(tag => t.Tags.Contains(tag)))
                .ToList();
        }

        // Method to search tasks by due date range
        public IEnumerable<TaskEntity> SearchByDueDateRange(DateTime fromDate, DateTime toDate)
        {



            return _context.Set<TaskEntity>()
                .Where(t => t.DueDate >= fromDate && t.DueDate <= toDate)
                .ToList();
        }

        // Method to search tasks by status
        public IEnumerable<TaskEntity> SearchByStatus(IEnumerable<string> statuses)
        {
            // Convert statuses to TaskStatus
            var taskStatuses = statuses.Select(s => Enum.Parse<Core.Entities.Enum.Taskstatus>(s));

            return _context.Set<TaskEntity>()
                .Where(t => taskStatuses.Contains(t.Status))
                .ToList();
        }

        public TaskEntity GetById(int id)
        {
            return _context.Set<TaskEntity>().Find(id);
        }


        public IEnumerable<TaskEntity> GetAll()
        {
            return _context.Set<TaskEntity>()
            .Select(taskEntity => new TaskEntity
            {
                Id = taskEntity.Id,
                TaskName = taskEntity.TaskName,
                Tags = taskEntity.Tags,
                DueDate = taskEntity.DueDate,
                Color = taskEntity.Color,
                AssignedTo = taskEntity.AssignedTo,
                Status = taskEntity.Status,
                Activites = taskEntity.Activites
                // Map other properties as needed
            })
            .ToList();
        }

        bool IRepository<TaskEntity>.Delete(int id)
        {
            var entity = _context.Set<TaskEntity>().Find(id);
            if (entity != null)
            {
                _context.Set<TaskEntity>().Remove(entity);
                _context.SaveChanges();
                return true; // Return true to indicate successful deletion
            }
            return false; // Return false if entity was not found
        }


        // Existing repository methods..


        Activity IRepository<TaskEntity>.AddActivity(int taskId, DateTime activityDate, string doneBy, string description)
        {
            var task = _context.Set<TaskEntity>().Include(t => t.Activites).FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                if (task.Activites == null)
                {
                    task.Activites = new List<Activity>(); // Ensure Activites list is initialized
                }

                var activity = new Activity
                {
                    ActivityDate = activityDate,
                    DoneBy = doneBy,
                    Description = description
                };
                task.Activites.Add(activity);
                _context.SaveChanges();
                return activity;
            }
            else
            {
                // Handle the case where task is null
                // For example, you could throw an exception or return null
                throw new InvalidOperationException("Task not found.");
            }
        }


        Task<IEnumerable<Activity>> IRepository<TaskEntity>.GetTaskActivities(int taskId)
        {
            var task = _context.Set<TaskEntity>().Include(t => t.Activites).FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                return Task.FromResult<IEnumerable<Activity>>(task.Activites);
            }
            else
            {
                // Handle the case where task is null
                // For example, you could return an empty list or throw an exception
                return Task.FromResult<IEnumerable<Activity>>(Enumerable.Empty<Activity>());
            }


        }




        public Task<IEnumerable<TaskEntity>> SearchTasks(TaskSearchCriteria criteria)
        {
            IQueryable<TaskEntity> query = _context.Set<TaskEntity>();

            // Task name (contains search, case in-sensitive)
            if (!string.IsNullOrEmpty(criteria.Taskname))
            {
                var taskNameLower = criteria.Taskname.ToLower();
                query = query.Where(t => EF.Functions.Like(t.TaskName.ToLower(), $"%{taskNameLower}%"));
            }

            // One or more tags (exact tag name search)
            if (criteria.Tags != null && criteria.Tags.Any())
            {
                foreach (var tag in criteria.Tags)
                {
                    query = query.Where(t => t.Tags.Any(tg => tg == tag));
                }
            }

            // Due date (range search)
            if (criteria.StartDate.HasValue && criteria.EndDate.HasValue)
            {
                query = query.Where(t => t.DueDate >= criteria.StartDate.Value && t.DueDate <= criteria.EndDate.Value);
            }

            // Status search (one or more status)
            if (criteria.Statuses != null && criteria.Statuses.Any())
            {
                query = query.Where(t => criteria.Statuses.Contains(t.Status));
            }

            return Task.FromResult<IEnumerable<TaskEntity>>(query.ToList());
        }

        public bool validate(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return false;
            }

            // Directly compare the provided password with the stored password
            return user.Password == password;
        }
    }
}