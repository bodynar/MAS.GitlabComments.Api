namespace MAS.GitlabComments.WebApi.Tests.AppApiControllerTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using MAS.GitlabComments.Logic.Models.Import;

    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using Xunit;

    public sealed class ExportAppDataTests : BaseAppApiControllerTests
    {
        [Fact]
        public void ShouldReturnProblem_WhenExceptionOccurs()
        {
            var expectedErrorMessage = "Error during exporting app data";
            ShouldThrowAnException = true;
            var result = TestedController.ExportAppData();

            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
            Assert.Equal(expectedErrorMessage, (((ObjectResult)result).Value as ProblemDetails).Detail);
        }

        [Fact]
        public void ShouldReturnFileStreamResult()
        {
            var expectedFileName = $"MAS.GC_Export_{DateTime.UtcNow:dd.MM.yyyy_HH:mm}.json";
            var expectedContentType = "application/json";
            
            
            var result = TestedController.ExportAppData();


            Assert.NotNull(result);
            Assert.IsType<FileStreamResult>(result);
            Assert.Equal(expectedFileName, ((FileStreamResult)result).FileDownloadName);
            Assert.Equal(expectedContentType, ((FileStreamResult)result).ContentType);

            using var stream = new StreamReader(((FileStreamResult)result).FileStream);
            var jsonContent = stream.ReadToEnd();

            var data = JsonConvert.DeserializeObject<IEnumerable<CommentExportModel>>(jsonContent);
            Assert.NotNull(data);
            Assert.Empty(data);
        }
    }
}
