using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureDocumentStorageSystem.Dto;
using SecureDocumentStorageSystem.Services;
using SecureDocumentStorageSystem.Services.Interfaces;
using System.Security.Claims;

namespace SecureDocumentStorageSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class FileController : ControllerBase
	{
		private readonly IDocumentService _docService;

		public FileController(IDocumentService docService)
		{
			_docService = docService;
		}

		private Guid GetUserId() =>
			Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

		[HttpPost("upload")]
		public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest request)
		{
			var userId = GetUserId();
			await _docService.UploadAsync(userId, request.FileName, request.File);
			return Ok("File uploaded successfully");
		}

		[HttpGet("{fileName}")]
		public async Task<IActionResult> Get(string fileName, [FromQuery] int? revision)
		{
			var userId = GetUserId();
			var doc = revision.HasValue
				? await _docService.GetByRevisionAsync(userId, fileName, revision.Value)
				: await _docService.GetLatestAsync(userId, fileName);

			if (doc == null) return NotFound("File not found");

			return File(doc.Content, "application/octet-stream", fileName);
		}

		[HttpGet("list")]
		public async Task<IActionResult> List()
		{
			var userId = GetUserId();
			var files = await _docService.ListUserFilesAsync(userId);
			return Ok(files);
		}


		[HttpDelete("Delete")]
		public async Task<IActionResult> SoftDeleteDocument([FromQuery] Guid id, [FromQuery] Guid userId)
		{
			if (id == Guid.Empty || userId == Guid.Empty)
				return BadRequest("Both 'id' and 'userId' must be valid GUIDs.");

			try
			{
				await _docService.SoftDeleteDocumentAsync(id, userId);
				return Ok("Document deleted successfully."); // Return 200 with a message
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while soft deleting the document: {ex.Message}");
			}
		}

	}
}
