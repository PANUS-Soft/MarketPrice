namespace MarketPrice.Data.Models
{
    public class Rating
    {
        public Guid RatingId { get; set; }
        public Guid RaterUserId { get; set; }
        public Guid RatedUserId { get; set; }
        public required byte Score { get; set; }
        public string? Comment { get; set; }
        public DateTime DateRecorded { get; set; } = DateTime.UtcNow;
        public DateTime? DateUpdated { get; set; }
    }
}
