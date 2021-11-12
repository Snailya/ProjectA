using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocAggregate.Specifications;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Core.Services
{
    public class DocService : IDocService
    {
        private readonly ILogger<DocService> _logger;
        private readonly IRepository<Document> _repository;
        private readonly IDMSService _dmsService;

        public DocService(ILogger<DocService> logger, IRepository<Document> repository,
            IDMSService dmsService)
        {
            _logger = logger;
            _repository = repository;
            _dmsService = dmsService;
        }

        /// <summary>
        ///     Get document information from file server and save info to local database.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Document> Down(int entityId)
        {
            _logger.LogInformation(nameof(Down));

            var document = await _repository.GetBySpecAsync(new DocumentByEntityIdSpec(entityId));
            if (document == null)
                throw new Exception(); // todo: define exception type

            var versions = _dmsService.GetVersions(entityId).Reverse().ToList();
            foreach (var version in versions.Where(version => version.VersionNumber.IsMajorVersion()).Where(version =>
                         document.Versions.All(x => x.VersionNumber != version.VersionNumber)))
                document.UpdateVersion(version);

            // update other info use latest version
            var documentRemote = _dmsService.GetDocument(document.EntityId);
            document.FileName = documentRemote.FileName;
            document.FilePath = documentRemote.FilePath;
            document.FileNamePath = documentRemote.FileNamePath;
            document.UpdatedAt = documentRemote.UpdatedAt;
            document.UpdatedBy = documentRemote.UpdatedBy;

            await _repository.SaveChangesAsync();

            return document;
        }

        public Task<Document> CopyDocumentAsync(int sourceId, int targetFolderId)
        {
            _logger.LogInformation(nameof(CopyDocumentAsync));

            var copy = _dmsService.CopySingleDocument(sourceId, targetFolderId);

            return Task.FromResult(new Document(copy));
        }

        public async Task SynchronizeDocument(int sourceId, int targetId)
        {
            _logger.LogInformation(nameof(SynchronizeDocument));

            var sourceVersions = _dmsService.GetVersions(sourceId).ToList();
            var targetVersions = _dmsService.GetVersions(targetId).ToList();

            foreach (var version in sourceVersions.Where(version =>
                         targetVersions.All(x => x.VersionNumber != version.VersionNumber)))
            {
                var fileStream = await _dmsService.DownloadVersionAsync(version.VersionId);
                await _dmsService.UpdateVersionAsync(targetId, fileStream);
                if (version.VersionNumber.IsMajorVersion())
                    _dmsService.PublishVersion(targetId);
            }
        }
    }
}