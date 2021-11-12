using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocSetAggregate;
using ProjectA.Core.Models.DocSetAggregate.Specifications;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Core.Services
{
    public class DocSetService : IDocSetService
    {
        private readonly ILogger<DocSetService> _logger;
        private readonly IRepository<DocumentSet> _repository;

        public DocSetService(ILogger<DocSetService> logger, IRepository<DocumentSet> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task AddUpdateLog(Document document)
        {
            _logger.LogInformation(nameof(AddUpdateLog));

            var spec = new DocumentSetContainsSpecificDocumentSpec(document);
            foreach (var docSet in await _repository.ListAsync(spec))
                docSet.Update(document);
        }
    }
}