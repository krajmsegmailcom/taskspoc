using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Core.Entities.Common;

namespace Tasks.Core.Entities
{
    public class Activity 
    {
        [Key]
        public int Id { get; set; }
        public DateTime ActivityDate { get; set; }
        public string DoneBy { get; set; }
        public string Description { get; set; }
        // Foreign key property
        public int TaskId { get; set; }
    }
}
