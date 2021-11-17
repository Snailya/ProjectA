using System.Threading.Tasks;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocAggregate.Specifications;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Core.Services
{
    public class AppService : IAppService
    {
        private readonly IDocumentHelper _documentHelper;
        private readonly IRepository<Document> _repository;

        public AppService(IRepository<Document> repository, IDocumentHelper documentHelper)
        {
            _repository = repository;
            _documentHelper = documentHelper;
        }

        public async Task ListenAndUpdate()
        {
            var documents = await _repository.ListAsync();
            foreach (var document in documents)
            {
                await _documentHelper.UpdateInfo(document);
                await _repository.UpdateAsync(document);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task<Document> AddDocument(int entityId)
        {
            var document = new Document(entityId);
            await _repository.AddAsync(document);
            await _repository.SaveChangesAsync();

            return document;
        }

        public async Task<Document> AddDocumentBindingToFolder(int sourceId, int targetFolderId)
        {
            var source = await _repository.GetBySpecAsync(new DocumentSpec(sourceId));
            if (source == null)
            {
                source = new Document(sourceId);
                await _repository.AddAsync(source);
            }

            var updatedSource = await _documentHelper.CreateBinding(source, targetFolderId);
            await _repository.UpdateAsync(updatedSource);
            await _repository.SaveChangesAsync();

            return source;
        }

        public async Task<Document> AddDocumentBindingToExistFile(int sourceId, int targetId)
        {
            var source = await _repository.GetBySpecAsync(new DocumentSpec(sourceId)) ??
                         new Document(sourceId);
            var binding = await _repository.GetBySpecAsync(new DocumentSpec(targetId)) ??
                          new Document(targetId);

            source.AddBinding(binding);
            await _repository.UpdateAsync(source);
            await _repository.SaveChangesAsync();

            return source;
        }
    }
}