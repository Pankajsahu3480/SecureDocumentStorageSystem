using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureDocumentStorageSystem.Dto;
using SecureDocumentStorageSystem.Dtos;
using SecureDocumentStorageSystem.Services.Interfaces;

namespace SecureDocumentStorageSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			var result = await _authService.RegisterAsync(request.Username, request.Password);
			if (!result)
				return BadRequest("Username already exists");

			return Ok("User registered successfully");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var token = await _authService.LoginAsync(request.Username, request.Password);
			if (token == null)
				return Unauthorized("Invalid credentials");

			return Ok(new AuthResponse { Token = token });
		}
	}
}

