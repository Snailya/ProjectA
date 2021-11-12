using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocAggregate.Specifications;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Desktop.Services
{
    public class ShepherdService : BackgroundService
    {
        private readonly IDocService _docService;
        private readonly ILogger _logger;
        private readonly IRepository<Document> _repository;

        private Timer _timer;

        public ShepherdService(ILogger<ShepherdService> logger, IRepository<Document> repository,
            IDocService docService)
        {
            _logger = logger;
            _repository = repository;
            _docService = docService;
        }

        public bool EnableSync { get; set; }

        public event EventHandler AfterExecute;

        public async Task ExecuteImmediately()
        {
            await DoWork(null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(nameof(ExecuteAsync));

            _timer = new Timer(o => DoWork(o), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        private async Task DoWork(object state)
        {
            _logger.LogInformation(nameof(DoWork));

            await BatchDownAsync();
            if (EnableSync) await BatchSynchronizeDocumentsAsync();
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

        public async Task BatchDownAsync()
        {
            _logger.LogInformation(nameof(BatchDownAsync));
            foreach (var document in await _repository.ListAsync()) await _docService.Down(document.EntityId);
        }

        public async Task BatchSynchronizeDocumentsAsync()
        {
            _logger.LogInformation(nameof(BatchSynchronizeDocumentsAsync));

            var spec = new DocumentsWithLinkedDocSpec();
            var sources = await _repository.ListAsync(spec);

            foreach (var document in sources)
            {
                if (document.LinkedDoc == null)
                    document.MakeLink(
                        await _docService.CopyDocumentAsync(document.EntityId, document.LinkedDocFolderId));

                await _docService.SynchronizeDocument(document.EntityId, document.LinkedDoc!.EntityId);
            }
        }
    }
}