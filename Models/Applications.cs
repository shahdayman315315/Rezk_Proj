namespace Rezk_Proj.Models
{
    public enum Status
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3
    }
    public class Applications
    {
        public int JobId { get; set; }
        public Job Job { get; set; }
        public int ApplicantId { get; set; }
        public Applicant Applicant { get; set; }
        public Status Status { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
