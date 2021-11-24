namespace MAS.GitlabComments.Models
{
    using System;

    public class CommentModel
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public long AppearanceCount { get; set; }
    }
}
