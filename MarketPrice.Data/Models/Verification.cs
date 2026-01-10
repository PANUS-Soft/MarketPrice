namespace MarketPrice.Data.Models
{
    public class Verification
    {
        public Guid VerificationId { get; set; }
        public required Guid UserId { get; set; }
        public required int VerificationTypeId { get; set; }
        public required int CurrentVerificationStatusId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset DateStarted { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? DateCompleted { get; set; }


    }
}
