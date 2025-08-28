namespace Rezk_Proj.Models
{
    
    public class Applications
    {
        public int JobId { get; set; }
        public Job Job { get; set; }
        public int ApplicantId { get; set; }
        public Applicant Applicant { get; set; }

        public int StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
