using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SecureDocumentStorageSystem.Controllers;
using SecureDocumentStorageSystem.Dto;
using SecureDocumentStorageSystem.Dtos;
using SecureDocumentStorageSystem.Services.Interfaces;
using Xunit;

namespace SecureDocumentStorageSystem.Tests.Controllers
{
	public class AuthControllerTests
	{
		private readonly Mock<IAuthService> _authServiceMock;
		private readonly AuthController _controller;

		public AuthControllerTests()
		{
			_authServiceMock = new Mock<IAuthService>();
			_controller = new AuthController(_authServiceMock.Object);
		}

		[Fact]
		public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
		{
			// Arrange
			var request = new RegisterRequest { Username = "testuser", Password = "Test123!" };
			_authServiceMock.Setup(s => s.RegisterAsync(request.Username, request.Password))
							.ReturnsAsync(true);

			// Act
			var result = await _controller.Register(request);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("User registered successfully", okResult.Value);
		}

		[Fact]
		public async Task Register_ReturnsBadRequest_WhenUsernameAlreadyExists()
		{
			// Arrange
			var request = new RegisterRequest { Username = "existinguser", Password = "Test123!" };
			_authServiceMock.Setup(s => s.RegisterAsync(request.Username, request.Password))
							.ReturnsAsync(false);

			// Act
			var result = await _controller.Register(request);

			// Assert
			var badRequest = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Username already exists", badRequest.Value);
		}

		[Fact]
		public async Task Login_ReturnsOkWithToken_WhenCredentialsAreValid()
		{
			// Arrange
			var request = new LoginRequest { Username = "validuser", Password = "Password123" };
			var fakeToken = "fake-jwt-token";
			_authServiceMock.Setup(s => s.LoginAsync(request.Username, request.Password))
							.ReturnsAsync(fakeToken);

			// Act
			var result = await _controller.Login(request);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var response = Assert.IsType<AuthResponse>(okResult.Value);
			Assert.Equal(fakeToken, response.Token);
		}

		[Fact]
		public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
		{
			// Arrange
			var request = new LoginRequest { Username = "invaliduser", Password = "wrongpass" };
			_authServiceMock.Setup(s => s.LoginAsync(request.Username, request.Password))
							.ReturnsAsync((string)null);

			// Act
			var result = await _controller.Login(request);

			// Assert
			var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
			Assert.Equal("Invalid credentials", unauthorized.Value);
		}
	}
}
