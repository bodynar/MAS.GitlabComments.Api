namespace MAS.GitlabComments.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Models;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("[controller]")]
    public class CommentsApiController: ControllerBase
    {
        private ILogger<CommentsApiController> Logger { get; }

        public CommentsApiController(ILogger<CommentsApiController> logger)
        {
            Logger = logger;
        }

        [HttpGet]
        public IEnumerable<CommentModel> Get()
        {
            return default;
        }

        [HttpGet]
        public string GetDescription(Guid commentId)
        {
            return default;
        }

        [HttpPut]
        public void Add([FromBody]AddCommentModel addCommentModel)
        {

        }

        [HttpDelete]
        public void Delete(Guid commentId)
        {

        }

        [HttpPost]
        public void Update([FromBody]UpdateCommentModel updateCommentModel)
        {

        }

        [HttpPost]
        public void Increment([FromBody]Guid commentId)
        {

        }
    }
}
