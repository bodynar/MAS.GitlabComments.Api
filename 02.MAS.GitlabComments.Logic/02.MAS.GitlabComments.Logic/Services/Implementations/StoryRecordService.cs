namespace MAS.GitlabComments.Logic.Services.Implementations
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Data.Models;
    using MAS.GitlabComments.Data.Services;
    using MAS.GitlabComments.Logic.Models;

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
