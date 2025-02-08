namespace MAS.GitlabComments.Logic.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Exceptions;
    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Select;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Models;

    /// <summary>
    /// Service for managing <see cref="Comment"/>
    /// </summary>
    public class CommentService : ICommentService
    {
        /// <summary>
        /// Data provider of <see cref="Comment"/> entities
        /// </summary>
        private IDataProvider<Comment> CommentsDataProvider { get; }

        /// <summary>
        /// Data provider of <see cref="StoryRecord"/> entities
        /// </summary>
        private IDataProvider<StoryRecord> StoryRecordsDataProvider { get; }

        /// <inheritdoc cref="IBusinessLogicSettings"/>
        private IBusinessLogicSettings ApplicationSettings { get; }

        /// <inheritdoc cref="ISystemVariableProvider"/>
        private ISystemVariableProvider SystemVariableProvider { get; }

        /// <inheritdoc cref="ITempDatabaseModifier"/>
        private ITempDatabaseModifier TempDatabaseModifier { get; }

        /// <inheritdoc cref="IRetractionTokenManager"/>
        private IRetractionTokenManager RetractionTokenManager { get; }

        /// <summary>
        /// Initializing <see cref="CommentService"/>
        /// </summary>
        /// <param name="commentsDataProvider">Comments data provider</param>
        /// <param name="storyRecordsDataProvider">Story records data provider</param>
        /// <param name="applicationSettings">Application settings</param>
        /// <param name="systemVariableProvider">System variable data provider</param>
        /// <exception cref="ArgumentNullException">Some parameters are null</exception>
        public CommentService(
            IDataProvider<Comment> commentsDataProvider,
            IDataProvider<StoryRecord> storyRecordsDataProvider,
            IBusinessLogicSettings applicationSettings,
            ISystemVariableProvider systemVariableProvider,
            IRetractionTokenManager retractionTokenManager,
            ITempDatabaseModifier tempDatabaseModifier
        )
        {
            CommentsDataProvider = commentsDataProvider ?? throw new ArgumentNullException(nameof(commentsDataProvider));
            StoryRecordsDataProvider = storyRecordsDataProvider ?? throw new ArgumentNullException(nameof(storyRecordsDataProvider));
            ApplicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
            SystemVariableProvider = systemVariableProvider ?? throw new ArgumentNullException(nameof(systemVariableProvider));
            TempDatabaseModifier = tempDatabaseModifier ?? throw new ArgumentNullException(nameof(tempDatabaseModifier));
            RetractionTokenManager = retractionTokenManager ?? throw new ArgumentNullException(nameof(retractionTokenManager));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Some parameters are null</exception>
        /// <returns>Instance of <see cref="NewComment"/> if comment is created</returns>
        public NewComment Add(AddCommentModel addCommentModel)
        {
            if (addCommentModel == null)
            {
                throw new ArgumentNullException(nameof(addCommentModel));
            }

            if (!addCommentModel.IsImportAction)
            {
                if (string.IsNullOrEmpty(addCommentModel.Message))
                {
                    throw new ArgumentNullException(nameof(AddCommentModel.Message));
                }

                if (string.IsNullOrEmpty(addCommentModel.CommentWithLinkToRule))
                {
                    throw new ArgumentNullException(nameof(AddCommentModel.CommentWithLinkToRule));
                }
            }

            var newNumber = SystemVariableProvider.GetValue<int>("LastCommentNumber") + 1;
            var number = string.Format(ApplicationSettings.CommentNumberTemplate, newNumber);

            var count = addCommentModel.AppearanceCount <= 0 ? 1 : addCommentModel.AppearanceCount;

            var newId = CommentsDataProvider.Add(
                new Comment
                {
                    AppearanceCount = count,
                    Message = addCommentModel.Message,
                    CommentWithLinkToRule = addCommentModel.CommentWithLinkToRule,
                    Description = addCommentModel.Description,
                    Number = number,
                }
            );

            if (newId != default)
            {
                SystemVariableProvider.Set("LastCommentNumber", newNumber);
            }

            return new NewComment
            {
                Id = newId,
                Number = number,
            };
        }

        /// <inheritdoc />
        public IEnumerable<CommentModel> Get()
        {
            return
                CommentsDataProvider
                    .Select<CommentModel>()
                    .OrderByDescending(x => x.AppearanceCount)
                    .ToList();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Some parameters are null</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        public void Update(UpdateCommentModel updateCommentModel)
        {
            if (updateCommentModel == null)
            {
                throw new ArgumentNullException(nameof(updateCommentModel));
            }
            if (string.IsNullOrEmpty(updateCommentModel.Message))
            {
                throw new ArgumentNullException(nameof(UpdateCommentModel.Message));
            }
            if (string.IsNullOrEmpty(updateCommentModel.CommentWithLinkToRule))
            {
                throw new ArgumentNullException(nameof(UpdateCommentModel.CommentWithLinkToRule));
            }

            GetCommentWithWithChecking(updateCommentModel.Id);

            var newValues = new Dictionary<string, object>
            {
                { nameof(Comment.Message), updateCommentModel.Message }
            };

            if (!string.IsNullOrEmpty(updateCommentModel.Description))
            {
                newValues.Add(nameof(Comment.Description), updateCommentModel.Description);
            }
            if (!string.IsNullOrEmpty(updateCommentModel.CommentWithLinkToRule))
            {
                newValues.Add(nameof(Comment.CommentWithLinkToRule), updateCommentModel.CommentWithLinkToRule);
            }

            CommentsDataProvider.Update(updateCommentModel.Id, newValues);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        public Guid Increment(Guid commentId)
        {
            var entity = GetCommentWithWithChecking(commentId);

            var newValues = new Dictionary<string, object>
            {
                { nameof(Comment.AppearanceCount), entity.AppearanceCount + 1 },
            };

            CommentsDataProvider.Update(commentId, newValues);

            var tokenId = RetractionTokenManager.Create(commentId);

            return tokenId;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Parameter commentIds is null</exception>
        public void Delete(params Guid[] commentIds)
        {
            if (commentIds == null)
            {
                throw new ArgumentNullException(nameof(commentIds));
            }

            commentIds = commentIds.Where(x => x != default).ToArray();

            if (commentIds.Any())
            {
                CommentsDataProvider.Delete(commentIds);
            }
        }

        /// <inheritdoc cref="ICommentService.GetIncomplete"/>
        public IEnumerable<IncompleteCommentData> GetIncomplete()
        {
            return
                CommentsDataProvider
                    .Select<IncompleteCommentData>(
                        new SelectConfiguration()
                        {
                            Filter = new FilterGroup()
                            {
                                Items = new[]
                                {
                                    new FilterItem()
                                    {
                                        Name = "EmptyNumberFilter",
                                        FieldName = nameof(Comment.Number),
                                        LogicalComparisonType = ComparisonType.Equal,
                                        Value = string.Empty
                                    }
                                }
                            }
                        }
                    )
                    .ToList();
        }

        /// <inheritdoc cref="ICommentService.UpdateIncomplete"/>
        public void UpdateIncomplete()
        {
            var incompleteData = GetIncomplete();

            if (!incompleteData.Any())
            {
                return;
            }

            var lastNumber = SystemVariableProvider.GetValue<int>("LastCommentNumber");
            var updateDataTasks = incompleteData.Select(
                x => new Tuple<Guid, string>(
                    x.Id,
                    string.Format(ApplicationSettings.CommentNumberTemplate, ++lastNumber)
                )
            );

            foreach (var updateDataItem in updateDataTasks)
            {
                CommentsDataProvider.Update(
                    updateDataItem.Item1,
                    new Dictionary<string, object> { { nameof(Comment.Number), updateDataItem.Item2 } }
                );
            }

            SystemVariableProvider.Set("LastCommentNumber", lastNumber);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">One of parameters [<paramref name="sourceCommentId"/>, <paramref name="targetCommentId"/>] have default value</exception>
        /// <exception cref="EntityNotFoundException">Specified comments weren't found</exception>
        public void Merge(Guid sourceCommentId, Guid targetCommentId, MergeCommentUpdateModel newTargetValues)
        {
            if (sourceCommentId == default)
            {
                throw new ArgumentNullException(nameof(sourceCommentId));
            }

            if (targetCommentId == default)
            {
                throw new ArgumentNullException(nameof(targetCommentId));
            }

            var source = GetCommentWithWithChecking(sourceCommentId);
            var target = GetCommentWithWithChecking(targetCommentId);

            var newValues = new Dictionary<string, object>
            {
                { nameof(Comment.AppearanceCount), source.AppearanceCount + target.AppearanceCount }
            };

            if (newTargetValues != default)
            {
                if (!string.IsNullOrEmpty(newTargetValues.Message))
                {
                    newValues.Add(nameof(Comment.Message), newTargetValues.Message);
                }

                if (!string.IsNullOrEmpty(newTargetValues.Description))
                {
                    newValues.Add(nameof(Comment.Description), newTargetValues.Description);
                }

                if (!string.IsNullOrEmpty(newTargetValues.CommentWithLinkToRule))
                {
                    newValues.Add(nameof(Comment.CommentWithLinkToRule), newTargetValues.CommentWithLinkToRule);
                }
            }

            CommentsDataProvider.Update(target.Id, newValues);
            CommentsDataProvider.Delete(sourceCommentId);
        }

        /// <inheritdoc />
        public void RecalculateLastNumber()
        {
            var lastNumberFromVariable = SystemVariableProvider.GetValue<int>("LastCommentNumber");
            var maxNumberFromEntities =
                CommentsDataProvider
                    .Select<NumberOfComment>()
                    .Select(x =>
                        int.Parse(
                            string.Concat(
                                x.Number.ToCharArray().Where(x => char.IsDigit(x)).ToArray()    
                            )
                        )
                    )
                    .Max();

            if (maxNumberFromEntities <= lastNumberFromVariable)
            {
                return;
            }

            SystemVariableProvider.Set("LastCommentNumber", maxNumberFromEntities);
        }

        #region Not public API

        /// <summary>
        /// Get comment with validation by specified identifier
        /// </summary>
        /// <param name="commentId">Comment identifier value</param>
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        /// <returns>Found comment</returns>
        private Comment GetCommentWithWithChecking(Guid commentId)
        {
            if (commentId == default)
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            var entity = CommentsDataProvider.Get(commentId);

            if (entity == null)
            {
                throw new EntityNotFoundException(nameof(Comment), commentId);
            }

            return entity;
        }

        #endregion
    }
}
