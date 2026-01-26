namespace MarketPrice.Domain.Image.DTOs
{
    public class ImageResponseDto
    {
        public required byte[] ImageData { get; set; }
        public required string ContentType { get; set; }
    }
}
