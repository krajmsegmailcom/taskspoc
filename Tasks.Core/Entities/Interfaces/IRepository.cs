using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Core.Entities.Interfaces
{
    public interface IRepository<T>
    {
        T GetById(int id);

        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        bool Delete(int id);
        Task<IEnumerable<Task>> SearchTasks(TaskSearchCriteria criteria);
        bool validate(string username, string password);
       
        Activity AddActivity(int taskId, DateTime activityDate, string doneBy, string description);
        Task<IEnumerable<Activity>> GetTaskActivities(int taskId);
     

    }
}
