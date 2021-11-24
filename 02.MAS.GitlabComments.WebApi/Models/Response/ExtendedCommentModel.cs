namespace MAS.GitlabComments.Models
{
    using System;

    public class ExtendedCommentModel
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public string Description { get; set; }
    }
}
