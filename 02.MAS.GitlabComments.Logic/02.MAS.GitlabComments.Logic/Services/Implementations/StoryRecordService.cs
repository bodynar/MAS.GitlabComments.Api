namespace MAS.GitlabComments.Logic.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data.Filter;
    using MAS.GitlabComments.Data.Models;
    using MAS.GitlabComments.Data.Services;
    using MAS.GitlabComments.Logic.Models;

    public class CommentStoryRecordService : ICommentStoryRecordService
    {
        private IDataProvider<StoryRecord> DataProvider { get; }

        public CommentStoryRecordService(
            IDataProvider<StoryRecord> dataProvider
        )
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public IEnumerable<StoryRecordModel> Get(DateTime? start, DateTime? endDate, Guid? commentId, int? count)
        {
            var filtersDefined = start.HasValue || endDate.HasValue || commentId.HasValue;

            var dataItems = Enumerable.Empty<StoryRecord>();

            if (filtersDefined)
            {
                FilterGroup filter = new()
                {

                };

                dataItems = DataProvider.Where(filter);
            }
            else
            {
                dataItems = DataProvider.Get();
            }

            /**
             * TODO:
             * 1. Request JOIN model from StoryModel & Comment
             *      Probably must update DataProvider to support joint columns declared via attribute
             *      and special generic method
             *      OR
             *      special generic method with additional columns
             *      
             *      And all must support select with\-out filtering
            */


            return null;
        }
    }
}
