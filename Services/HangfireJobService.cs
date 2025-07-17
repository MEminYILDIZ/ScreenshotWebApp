using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenshotWebApp.Services
{
    public class HangfireJobService : BackgroundService
    {
        private readonly ScreenshotService _screenshotService;
        private readonly ILogger<HangfireJobService> _logger;

        public HangfireJobService(ScreenshotService screenshotService, ILogger<HangfireJobService> logger)
        {
            _screenshotService = screenshotService;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Hangfire job servis başlatıldı: {time}", DateTime.Now);
                //Cron.Daily saati UTC'ye göre alıyor
                RecurringJob.AddOrUpdate(
                    "screenshot-job",
                    () => _screenshotService.TakeScreenshotAsync(),
                    Cron.Daily(13)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hangfire job servisi başlatılırken hata oluştu.");
            }

            return Task.CompletedTask;
        }
    }
}
