using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SecureDocumentStorageSystem.Controllers;
using SecureDocumentStorageSystem.Dto;
using SecureDocumentStorageSystem.Models;
using SecureDocumentStorageSystem.Services.Interfaces;
using Xunit;

namespace SecureDocumentStorageSystem.Tests.Controllers
{
	public class FileControllerTests
	{
		private readonly Mock<IDocumentService> _docServiceMock;
		private readonly FileController _controller;
		private readonly Guid _testUserId = Guid.NewGuid();

		public FileControllerTests()
		{
			_docServiceMock = new Mock<IDocumentService>();
			_controller = new FileController(_docServiceMock.Object);

			// Mock User.Identity with NameIdentifier claim
			var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
			}, "mock"));

			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};
		}

		[Fact]
		public async Task Upload_ReturnsOk_WhenUploadIsSuccessful()
		{
			// Arrange
			var mockFile = new Mock<IFormFile>();
			var request = new UploadDocumentRequest
			{
				FileName = "test.txt",
				File = mockFile.Object
			};

			_docServiceMock.Setup(s => s.UploadAsync(_testUserId, request.FileName, request.File))
						   .Returns(Task.CompletedTask);

			// Act
			var result = await _controller.Upload(request);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("File uploaded successfully", okResult.Value);
		}

		[Fact]
		public async Task Get_ReturnsFile_WhenDocumentIsFound_Latest()
		{
			// Arrange
			var fileName = "doc.txt";
			var content = new byte[] { 1, 2, 3 };
			var document = new Document { Content = content };

			_docServiceMock.Setup(s => s.GetLatestAsync(_testUserId, fileName))
						   .ReturnsAsync(document);

			// Act
			var result = await _controller.Get(fileName, null);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal(content, fileResult.FileContents);
			Assert.Equal("application/octet-stream", fileResult.ContentType);
			Assert.Equal(fileName, fileResult.FileDownloadName);
		}

		[Fact]
		public async Task Get_ReturnsFile_WhenDocumentIsFound_ByRevision()
		{
			// Arrange
			var fileName = "doc.txt";
			var revision = 2;
			var content = new byte[] { 10, 20, 30 };
			var document = new Document { Content = content };

			_docServiceMock.Setup(s => s.GetByRevisionAsync(_testUserId, fileName, revision))
						   .ReturnsAsync(document);

			// Act
			var result = await _controller.Get(fileName, revision);

			// Assert
			var fileResult = Assert.IsType<FileContentResult>(result);
			Assert.Equal(content, fileResult.FileContents);
		}

		[Fact]
		public async Task Get_ReturnsNotFound_WhenDocumentDoesNotExist()
		{
			// Arrange
			var fileName = "missing.txt";

			_docServiceMock.Setup(s => s.GetLatestAsync(_testUserId, fileName))
						   .ReturnsAsync((Document)null);

			// Act
			var result = await _controller.Get(fileName, null);

			// Assert
			var notFound = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("File not found", notFound.Value);
		}

		[Fact]
		public async Task List_ReturnsOk_WithListOfFiles()
		{
			// Arrange
			var files = new List<string> { "file1.txt", "file2.txt" };

			_docServiceMock.Setup(s => s.ListUserFilesAsync(_testUserId))
						   .ReturnsAsync(files);

			// Act
			var result = await _controller.List();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedFiles = Assert.IsType<List<string>>(okResult.Value);
			Assert.Equal(2, returnedFiles.Count);
		}
	}
}
