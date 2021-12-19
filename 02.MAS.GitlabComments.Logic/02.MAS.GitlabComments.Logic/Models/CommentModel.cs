namespace MAS.GitlabComments.Logic.Models
{
    using System;

    public class CommentModel
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public long AppearanceCount { get; set; }
    }
}
