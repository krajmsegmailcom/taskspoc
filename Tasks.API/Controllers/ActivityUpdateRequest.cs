namespace Tasks.API.Controllers
{
    public class ActivityUpdateRequest
    {
        public int ActivityId { get; set; } // Assuming you need the activity ID for updating
        public DateTime ActivityDate { get; set; }
        public string DoneBy { get; set; }
        public string Description { get; set; }
    }

}