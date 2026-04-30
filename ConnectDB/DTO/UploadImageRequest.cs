namespace ConnectDB.DTO
{
    public class UploadImageRequest
    {
        public long ProductId { get; set; }
        public IFormFile File { get; set; }
    }
}
