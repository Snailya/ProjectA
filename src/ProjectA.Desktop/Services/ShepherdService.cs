using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectA.Core.Interfaces;

namespace ProjectA.Desktop.Services
{
    public class ShepherdService : BackgroundService
    {
        private readonly IAppService _appService;
        private readonly ILogger _logger;

        private Timer _timer;

        public ShepherdService(ILogger<ShepherdService> logger,
            IAppService appService)
        {
            _logger = logger;
            _appService = appService;
        }
        
        public event EventHandler AfterExecute;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(nameof(ExecuteAsync));

            _timer = new Timer(o => DoWork(), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        public async void DoWork()
        {
            _logger.LogInformation(nameof(DoWork));

            await _appService.ListenAndUpdate();
            AfterExecute?.Invoke(this, EventArgs.Empty);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(StopAsync));

            _timer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation(nameof(Dispose));

            _timer?.Dispose();
            base.Dispose();
        }
    }
}