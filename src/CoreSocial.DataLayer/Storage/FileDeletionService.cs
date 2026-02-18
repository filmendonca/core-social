using DataLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataLayer.Storage
{
    //Class used to delete temporary files on a schedule
    public class FileDeletionService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FileDeletionService> _logger;

        public FileDeletionService(IServiceScopeFactory scopeFactory, ILogger<FileDeletionService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    //Since IFileStorage is a scoped service, it can't be injected in a hosted/singleton service
                    using var scope = _scopeFactory.CreateScope();
                    var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorage>();

                    //Delete temp files older than the specified time
                    fileStorage.DeleteTempFiles(TimeSpan.FromMinutes(30));

                    //Wait 10 minutes
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"An error ocurred in {nameof(ExecuteAsync)} method of {nameof(FileDeletionService)} class.");
                throw;
            }
            catch (Exception)
            {
                _logger.LogError($"An error ocurred when deleting a file in {nameof(ExecuteAsync)} method of {nameof(FileDeletionService)} class.");
            }
        }
    }
}
