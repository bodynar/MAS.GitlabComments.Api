namespace MAS.GitlabComments.Logic.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Base;
    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Exceptions;
    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Select;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Models;

    /// <summary>
    /// Manager of retraction tokens, implementation of <see cref="IRetractionTokenManager"/>
    /// </summary>
    public class RetractionTokenManager : IRetractionTokenManager // TODO: add tests
    {
        /// <summary>
        /// Data provider of <see cref="Comment"/> entities
        /// </summary>
        private IDataProvider<Comment> CommentsDataProvider { get; }

        /// <summary>
        /// Data provider of <see cref="StoryRecord"/> entities
        /// </summary>
        private IDataProvider<StoryRecord> StoryRecordsDataProvider { get; }

        /// <summary>
        /// Data provider of <see cref="RetractionToken"/> entities
        /// </summary>
        private IDataProvider<RetractionToken> TokensDataProvider { get; }

        /// <inheritdoc cref="IBusinessLogicSettings"/>
        private IBusinessLogicSettings ApplicationSettings { get; }

        /// <inheritdoc cref="ILogger"/>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializing <see cref="RetractionTokenManager"/>
        /// </summary>
        /// <param name="commentsDataProvider">Comments data provider</param>
        /// <param name="storyRecordsDataProvider">Story records data provider</param>
        /// <param name="retractionTokensDataProvider">Retraction tokens data provider</param>
        /// <param name="applicationSettings">Application settings</param>
        /// <exception cref="ArgumentNullException">Some parameters are null</exception>
        public RetractionTokenManager(
            IDataProvider<Comment> commentsDataProvider,
            IDataProvider<StoryRecord> storyRecordsDataProvider,
            IDataProvider<RetractionToken> retractionTokensDataProvider,
            IBusinessLogicSettings applicationSettings,
            ILogger logger
        )
        {
            CommentsDataProvider = commentsDataProvider ?? throw new ArgumentNullException(nameof(commentsDataProvider));
            StoryRecordsDataProvider = storyRecordsDataProvider ?? throw new ArgumentNullException(nameof(storyRecordsDataProvider));
            TokensDataProvider = retractionTokensDataProvider ?? throw new ArgumentNullException(nameof(retractionTokensDataProvider));
            ApplicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Parameter "commentId" is <see cref="Guid.Empty"/></exception>
        public Guid Create(Guid commentId)
        {
            if (commentId == default)
            {
                throw new ArgumentException("CommentId parameter has default value");
            }

            var storyRecordId = StoryRecordsDataProvider.Add(
                new StoryRecord { CommentId = commentId }
            );

            var tokenId = TokensDataProvider.Add(new RetractionToken()
            {
                CommentId = commentId,
                StoryRecordId = storyRecordId,
                ValidUntil = DateTime.UtcNow.AddHours(ApplicationSettings.RetractionTokenLifeSpanHours),
            });

            return tokenId;
        }

        /// <inheritdoc />
        public IEnumerable<RetractionTokenReadModel> GetAvailable()
        {
            return
                TokensDataProvider
                    .Select<RetractionTokenReadModel>(new SelectConfiguration
                    {
                        Filter = new()
                        {
                            Name = "ExpiredRecordsFilter",
                            Items = new[]
                            {
                                new FilterItem()
                                {
                                    Name = "ExpiredDate",
                                    FieldName = nameof(RetractionToken.ValidUntil),
                                    LogicalComparisonType = ComparisonType.GreaterOrEqual,
                                    Value = DateTime.UtcNow
                                }
                            }
                        }
                    })
                    .ToList();
        }

        /// <inheritdoc />
        public void RemoveExpired()
        {
            var expiredRecords =
                TokensDataProvider
                    .Where(
                        new FilterGroup()
                        {
                            Name = "ExpiredRecordsFilter",
                            Items = new[]
                            {
                                new FilterItem()
                                {
                                    Name = "ExpiredDate",
                                    FieldName = nameof(RetractionToken.ValidUntil),
                                    LogicalComparisonType = ComparisonType.Less,
                                    Value = DateTime.UtcNow
                                }
                            }
                        }
                    )
                    .Select(x => x.Id)
                    .ToArray();

            TokensDataProvider.Delete(expiredRecords);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Parameter "tokenId" is <see cref="Guid.Empty"/></exception>
        /// <exception cref="EntityNotFoundException">Specified token is not found in database</exception>
        /// <exception cref="EntityNotFoundException">Related comment is not found in database</exception>
        /// <exception cref="InvalidOperationException">Specified token is outdated</exception>
        public void Retract(Guid tokenId)
        {
            if (tokenId == default)
            {
                throw new ArgumentNullException(nameof(tokenId));
            }

            var token = TokensDataProvider.Get(tokenId);

            if (token == default)
            {
                throw new EntityNotFoundException(nameof(RetractionToken), tokenId);
            }

            if (token.ValidUntil < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Specified token is not valid anymore");
            }

            var comment = CommentsDataProvider.Get(token.CommentId);

            if (comment == default)
            {
                throw new EntityNotFoundException(nameof(Comment), token.CommentId);
            }

            var storyRecord = StoryRecordsDataProvider.Get(token.StoryRecordId);

            var newValues = new Dictionary<string, object>
            {
                { nameof(Comment.AppearanceCount), comment.AppearanceCount - 1 },
            };

            CommentsDataProvider.Update(comment.Id, newValues);

            if (storyRecord != default)
            {
                StoryRecordsDataProvider.Update(storyRecord.Id, new Dictionary<string, object>
                {
                    { nameof(StoryRecord.IsRetracted), true },
                });
            }

            TokensDataProvider.Delete(tokenId);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Parameter "tokenIds" is <see langword="Guid.Empty"/></exception>
        public RetractResult Retract(IEnumerable<Guid> tokenIds)
        {
            if (tokenIds == default)
            {
                throw new ArgumentNullException(nameof(tokenIds));
            }

            var distinctTokens = tokenIds.Where(x => x != default).Distinct().ToList();

            if (!distinctTokens.Any())
            {
                return new RetractResult { };
            }

            var success = new List<RetractOperationResult>();
            var outdated = new List<RetractOperationResult>();
            var errors = new List<RetractOperationResult>();

            var commentIdToTokenId = new Dictionary<Guid, List<RetractionToken>>();

            foreach (var tokenId in distinctTokens)
            {
                try
                {
                    var token = TokensDataProvider.Get(tokenId);

                    if (token == default)
                    {
                        errors.Add(new RetractOperationResult { TokenId = tokenId, Error = "Specified token not found" });

                        continue;
                    }

                    if (token.ValidUntil < DateTime.UtcNow)
                    {
                        outdated.Add(new RetractOperationResult { TokenId = tokenId });

                        continue;
                    }

                    if (commentIdToTokenId.ContainsKey(token.CommentId))
                    {
                        commentIdToTokenId[token.CommentId].Add(token);
                    }
                    else
                    {
                        commentIdToTokenId.Add(token.CommentId, new List<RetractionToken> { token });
                    }
                }
                catch (Exception e)
                {
                    errors.Add(
                        new RetractOperationResult
                        {
                            TokenId = tokenId,
                            Error = e.Message
                        }
                    );
                }
            }

            var storyRecordIdsToRetract = new List<Guid>();

            foreach (var commentTokens in commentIdToTokenId)
            {
                try
                {
                    var comment = CommentsDataProvider.Get(commentTokens.Key);

                    if (comment == default)
                    {
                        errors.AddRange(
                            commentTokens.Value.Select(x => new RetractOperationResult { TokenId = x.Id, Error = "Related comment not found" })
                        );

                        continue;
                    }

                    storyRecordIdsToRetract.AddRange(
                        commentTokens.Value.Select(x => x.StoryRecordId)
                    );

                    var newValues = new Dictionary<string, object>
                    {
                        { nameof(Comment.AppearanceCount), comment.AppearanceCount - commentTokens.Value.Count },
                    };

                    CommentsDataProvider.Update(comment.Id, newValues);

                    TokensDataProvider.Delete(
                        commentTokens.Value.Select(x => x.Id).ToArray()
                    );

                    success.AddRange(
                        commentTokens.Value.Select(x => new RetractOperationResult { TokenId = x.Id })
                    );
                }
                catch (Exception e)
                {
                    errors.AddRange(
                        commentTokens.Value.Select(x => new RetractOperationResult { TokenId = x.Id, Error = e.Message })
                    );
                }
            }

            storyRecordIdsToRetract = storyRecordIdsToRetract.Distinct().ToList();

            foreach (var storyRecordId in storyRecordIdsToRetract)
            {
                StoryRecordsDataProvider.Update(storyRecordId, new Dictionary<string, object>
                {
                    { nameof(StoryRecord.IsRetracted), true },
                });
            }

            if (errors.Any())
            {
                Logger.Error(
                    $"Errors during batch retractions:\n{string.Join("\n- ", errors.Select(x => $"{x.TokenId} : {x.Error}"))}"
                );
            }

            return new RetractResult
            {
                Success = success,
                Outdated = outdated,
                Errors = errors,
            };
        }
    }
}