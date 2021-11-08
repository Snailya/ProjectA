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
        private readonly ILogger _logger;
        private readonly ISynchronizeService _synchronizeService;

        private Timer _timer;

        public ShepherdService(ILogger<ShepherdService> logger, ISynchronizeService synchronizeService)
        {
            _logger = logger;
            _synchronizeService = synchronizeService;
        }

        public bool EnableSync { get; set; }

        public event EventHandler AfterExecute;

        public void ExecuteImmediately()
        {
            DoWork(null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(nameof(ExecuteAsync));

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation(nameof(DoWork));

            _synchronizeService.Down();
            if (EnableSync) _synchronizeService.Up();
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