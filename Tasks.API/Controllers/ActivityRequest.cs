using System.ComponentModel.DataAnnotations;

namespace Tasks.API.Controllers
{
    public class ActivityRequest
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