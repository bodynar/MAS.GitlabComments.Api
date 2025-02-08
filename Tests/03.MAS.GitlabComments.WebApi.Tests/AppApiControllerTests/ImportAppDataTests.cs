namespace MAS.GitlabComments.WebApi.Tests.AppApiControllerTests
{
    using System.IO;
    using System.Text;

    using MAS.GitlabComments.Logic.Models.Import;

    using Microsoft.AspNetCore.Http;

    using Moq;

    using Newtonsoft.Json;

    using Xunit;

    public sealed class ImportAppDataTests : BaseAppApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceError_WhenFileIsNull()
        {
            var expectedErrorMessage = "No file imported";

            var result = TestedController.ImportAppData(null).Result;

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedErrorMessage, result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceError_WhenFileIsEmpty()
        {
            var expectedErrorMessage = "No file imported";
            var mockFile = new Mock<IFormFile>();

            mockFile.Setup(x => x.Length).Returns(0);

            var result = TestedController.ImportAppData(mockFile.Object).Result;

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedErrorMessage, result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceError_WhenFileIsNotJson()
        {
            var expectedErrorMessage = "File is not a json";
            var mockFile = new Mock<IFormFile>();

            mockFile.Setup(x => x.Length).Returns(10);
            mockFile.Setup(x => x.ContentType).Returns("SomeValue");

            var result = TestedController.ImportAppData(mockFile.Object).Result;

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedErrorMessage, result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceError_WhenExceptionOccurs()
        {
            ShouldThrowAnException = true;
            var mockFile = new Mock<IFormFile>();

            mockFile.Setup(x => x.Length).Returns(10);
            mockFile.Setup(x => x.ContentType).Returns("application/json");
            mockFile.Setup(x => x.OpenReadStream())
                .Returns(() =>
                {
                    var json = JsonConvert.SerializeObject(new CommentExportModel[] { new CommentExportModel { AppearanceCount = 1 } });
                    var byteArray = Encoding.UTF8.GetBytes(json);
                    var stream = new MemoryStream(byteArray);

                    return stream;
                });

            var result = TestedController.ImportAppData(mockFile.Object).Result;

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(ExceptionTestMessage, result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceError_WhenJsonIsEmpty()
        {
            var expectedErrorMessage = "JSON is incorrect";
            var mockFile = new Mock<IFormFile>();

            mockFile.Setup(x => x.Length).Returns(10);
            mockFile.Setup(x => x.ContentType).Returns("application/json");
            mockFile.Setup(x => x.OpenReadStream())
                .Returns(() =>
                {
                    var json = JsonConvert.SerializeObject(null);
                    var byteArray = Encoding.UTF8.GetBytes(json);
                    var stream = new MemoryStream(byteArray);

                    return stream;
                });

            var result = TestedController.ImportAppData(mockFile.Object).Result;

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedErrorMessage, result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceSuccess()
        {
            var mockFile = new Mock<IFormFile>();

            mockFile.Setup(x => x.Length).Returns(10);
            mockFile.Setup(x => x.ContentType).Returns("application/json");
            mockFile.Setup(x => x.OpenReadStream())
                .Returns(() =>
                {
                    var json = JsonConvert.SerializeObject(new CommentExportModel[] { new CommentExportModel { AppearanceCount = 1 } });
                    var byteArray = Encoding.UTF8.GetBytes(json);
                    var stream = new MemoryStream(byteArray);

                    return stream;
                });

            var result = TestedController.ImportAppData(mockFile.Object).Result;

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }
    }
}
