using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Core.Entities.Common;
using Tasks.Core.Entities.Enum;

namespace Tasks.Core.Entities
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string TaskName { get; set; }
        public List<string> Tags { get; set; }
        public DateTime DueDate { get; set; }
        public string Color { get; set; }
        public string AssignedTo { get; set; }
         public Taskstatus Status { get; set; }
        public List<Activity>? Activites { get; set; }

  
    }
}
