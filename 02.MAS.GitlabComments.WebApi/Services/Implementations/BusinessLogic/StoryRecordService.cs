namespace MAS.GitlabComments.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MAS.GitlabComments.Models;
    using MAS.GitlabComments.Models.Database;

    public class StoryRecordService : IStoryRecordService
    {
        private IDataProvider<StoryRecord> StoryRecordDataProvider { get; }

        public StoryRecordService(
            IDataProvider<StoryRecord> storyRecordDataProvider
        )
        {
            StoryRecordDataProvider = storyRecordDataProvider ?? throw new ArgumentNullException(nameof(storyRecordDataProvider));
        }

        public IEnumerable<StoryRecordModel> Get(DateTime? start, DateTime? endDate, Guid? commentId, int? count)
        {
            var dataItems = StoryRecordDataProvider.Get();

            return null;
        }
    }
}
