namespace MAS.GitlabComments.Logic.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Select;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Models;

    /// <summary>
    /// Implementation of <see cref="ICommentStoryRecordService"/>
    /// </summary>
    public class CommentStoryRecordService : ICommentStoryRecordService
    {
        /// <summary> Data provider of StoryRecords </summary>
        private IDataProvider<StoryRecord> DataProvider { get; }

        /// <summary>
        /// Initializing <see cref="CommentStoryRecordService"/>
        /// </summary>
        /// <param name="dataProvider">Instance of data provider of <see cref="StoryRecord"/></param>
        /// <exception cref="ArgumentNullException">Param dataProvider is null</exception>
        public CommentStoryRecordService(
            IDataProvider<StoryRecord> dataProvider
        )
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        /// <inheritdoc cref="ICommentStoryRecordService.Get(DateTime?, DateTime?, Guid?, int?)"/>
        public IEnumerable<StoryRecordViewModel> Get(DateTime? startDate, DateTime? endDate, Guid? commentId, int? count)
        {
            var filtersDefined = startDate.HasValue || endDate.HasValue || commentId.HasValue;

            var filter = filtersDefined ? BuildFilter(startDate, endDate, commentId) : null;

            var dataItems = DataProvider
                .Select<StoryRecordReadModel>(new SelectConfiguration { Filter = filter })
                .ToList()
                .GroupBy(x => x.CommentId, x => x.CommentText)
                .Select(x => new StoryRecordViewModel()
                {
                    CommentId = x.Key,
                    Count = x.Count(),
                    CommentText = x.First()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return dataItems;
        }

        private FilterGroup BuildFilter(DateTime? start, DateTime? endDate, Guid? commentId)
        {
            var filterItems = new List<FilterItem>();

            if (start.HasValue)
            {
                filterItems.Add(
                    new FilterItem()
                    {
                        Name = "CreatedOnStart",
                        FieldName = nameof(StoryRecord.CreatedOn),
                        LogicalComparisonType = ComparisonType.GreaterOrEqual,
                        Value = start.Value.AddDays(-1) // TODO: temporary, till implement converting to DATE
                    }
                );
            }

            if (endDate.HasValue)
            {
                filterItems.Add(
                    new FilterItem()
                    {
                        Name = "CreatedOnEnd",
                        FieldName = nameof(StoryRecord.CreatedOn),
                        LogicalComparisonType = ComparisonType.LessOrEqual,
                        Value = endDate.Value.AddDays(1) // TODO: temporary, till implement converting to DATE
                    }
                );
            }

            if (commentId.HasValue)
            {
                filterItems.Add(
                    new FilterItem()
                    {
                        Name = "CommentIdEquality",
                        FieldName = nameof(StoryRecord.CommentId),
                        LogicalComparisonType = ComparisonType.Equal,
                        Value = commentId.Value
                    }
                );
            }

            FilterGroup filter = new()
            {
                LogicalJoinType = FilterJoinType.And,
                Name = "StoryRecordFilter",
                Items = filterItems,
                TableAlias = $"{nameof(StoryRecord)}s"
            };

            return filter;
        }
    }
}
