namespace MAS.GitlabComments.WebApi.HostedServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using MAS.GitlabComments.Base;
    using MAS.GitlabComments.Logic.Services;

    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Hosted service for clearing expired retraction tokens
    /// </summary>
    public class ClearExpiredTokensService : IHostedService, IDisposable
    {
        /// <summary>
        /// Application logger
        /// </summary>
        private ILogger Logger { get; }

        /// <inheritdoc cref="IRetractionTokenManager"/>
        private IRetractionTokenManager TokenManager { get; }

        /// <summary>
        /// Timer for delayed execution of operation
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Initializing <see cref="ClearExpiredTokensService"/>
        /// </summary>
        /// <param name="logger">Application logger</param>
        /// <param name="tokenManager">Implementation of <see cref="IRetractionTokenManager"/></param>
        public ClearExpiredTokensService(
            ILogger logger,
            IRetractionTokenManager tokenManager
        )
        {
            Logger = logger;
            TokenManager = tokenManager;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            timer.Dispose();
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.Debug($"Hosted service \"{nameof(ClearExpiredTokensService)}\": Starting.");

            var now = DateTime.Now;
            var nextWednesday = GetNextWednesday(now);
            var initialDelay = nextWednesday - now;

            while (initialDelay.TotalMilliseconds < 0)
            {
                nextWednesday = GetNextWednesday(now.AddDays(1));
                initialDelay = nextWednesday - now;
            }

            timer = new Timer(ClearExpiredTokens, null, initialDelay, TimeSpan.FromDays(7));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.Debug($"Hosted service \"{nameof(ClearExpiredTokensService)}\": Stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get date of closest Wednesday
        /// </summary>
        /// <param name="start">Date from which search should be performed</param>
        /// <returns>Date of closest wednesday</returns>
        private static DateTime GetNextWednesday(DateTime start)
        {
            var daysToAdd = ((int)DayOfWeek.Wednesday - (int)start.DayOfWeek + 7) % 7;

            return start.AddDays(daysToAdd).Date.AddHours(0);
        }

        /// <summary>
        /// Clear expired retraction tokens
        /// </summary>
        /// <param name="_">(unused operation state argument)</param>
        private void ClearExpiredTokens(object _ = null)
        {
            Logger.Debug($"Hosted service \"{nameof(ClearExpiredTokensService)}\": Start executing task at: {DateTimeOffset.Now:T}.");

            TokenManager.RemoveExpired();

            Logger.Debug($"Hosted service \"{nameof(ClearExpiredTokensService)}\": End executing task at: {DateTimeOffset.Now:T}.");
        }
    }
}
