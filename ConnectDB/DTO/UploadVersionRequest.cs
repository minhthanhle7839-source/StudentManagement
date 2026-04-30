namespace ConnectDB.DTO
{
    public class UploadVersionRequest
    {
        public long ProductId { get; set; }
        public string Version { get; set; }
        public string? Changelog { get; set; }
        public IFormFile File { get; set; }
    }
}
