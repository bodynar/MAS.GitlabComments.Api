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

        /// <inheritdoc cref="ICommentStoryRecordService.Get(DateTime?, DateTime?, int?)"/>
        public IEnumerable<StoryRecordViewModel> Get(DateTime? startDate, DateTime? endDate, int? count)
        {
            var filter = BuildFilter(startDate, endDate);

            var dataItems = DataProvider
                .Select<StoryRecordReadModel>(new SelectConfiguration { Filter = filter })
                .ToList()
                .GroupBy(x => x.CommentId)
                .Select(x => new StoryRecordViewModel()
                {
                    CommentId = x.Key,
                    Count = x.Count(),
                    CommentText = x.First().CommentText,
                    Number = x.First().Number,
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return dataItems;
        }

        /// <summary>
        /// Get user custom filter
        /// </summary>
        /// <param name="start">Specified start date</param>
        /// <param name="endDate">Specified end date</param>
        /// <returns>Built filter, instance of <see cref="FilterGroup"/></returns>
        private static FilterGroup BuildFilter(DateTime? start, DateTime? endDate)
        {
            var filterItems = new List<FilterItem>()
            {
                new FilterItem()
                {
                    Name = "Active",
                    FieldName = nameof(StoryRecord.IsRetracted),
                    LogicalComparisonType = ComparisonType.NotEqual,
                    Value = true,
                },
            };

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
