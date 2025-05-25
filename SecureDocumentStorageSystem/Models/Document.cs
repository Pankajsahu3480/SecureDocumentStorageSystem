namespace SecureDocumentStorageSystem.Models
{
    public class Document
    {
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string FileName { get; set; } = string.Empty;
		public byte[] Content { get; set; } = Array.Empty<byte>();
		public int Version { get; set; }
		public DateTime UploadedAt { get; set; }
	}
}
