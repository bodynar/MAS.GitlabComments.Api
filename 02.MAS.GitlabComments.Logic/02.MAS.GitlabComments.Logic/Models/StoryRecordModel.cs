namespace MAS.GitlabComments.Logic.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class StoryRecordModel
    {
        public Guid CommentId { get; set; }

        public string CommentText { get; set; }

        public int IncrementCount { get; set; }
    }
}
