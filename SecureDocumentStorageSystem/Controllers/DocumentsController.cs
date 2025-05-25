using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureDocumentStorageSystem.Repositories.Interfaces;
using System.Security.Claims;

namespace SecureDocumentStorageSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentsController : ControllerBase
	{
		private readonly IDocumentRepository _repo;

		public DocumentsController(IDocumentRepository repo, IHttpContextAccessor httpContextAccessor)
		{
			_repo = repo;
		
		}

		
		//[HttpPost]
		//public async Task<IActionResult> Upload([FromBody] IFormFile file)
		//{
		//	if (file == null || file.Length == 0)
		//		return BadRequest("No file provided.");

		//	var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		//	if (userId == null)
		//		return Unauthorized();

		//	using var ms = new MemoryStream();
		//	await file.CopyToAsync(ms);
		//	var content = ms.ToArray();

		//	await _repo.SaveDocumentAsync(Guid.Parse(userId), file.FileName, content);
		//	return Ok("File uploaded successfully.");
		//}

	}
}
