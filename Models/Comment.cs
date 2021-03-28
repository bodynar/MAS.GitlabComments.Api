namespace MAS.GitlabComments.Models
{
    using System;

    public class Comment
    {
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string Message { get; set; }

        public string Description { get; set; }

        public long AppearanceCount { get; set; }
    }
}
