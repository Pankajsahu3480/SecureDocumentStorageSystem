namespace SecureDocumentStorageSystem.Dto
{
    public class UploadDocumentRequest
    {
		public string FileName { get; set; } = string.Empty;
		public IFormFile File { get; set; } = default!;
	}
}
