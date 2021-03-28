namespace MAS.GitlabComments.Models
{
    using System;

    public class UpdateCommentModel
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public string Description { get; set; }
    }
}
