using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Core.Services
{
    public class DocumentHelper : IDocumentHelper
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger<DocumentHelper> _logger;

        public DocumentHelper(ILogger<DocumentHelper> logger, IFileSystemService fileSystemService)
        {
            _logger = logger;
            _fileSystemService = fileSystemService;
        }

        private Task UpdateBaseInfo(Document document)
        {
            var infoToUpdate = _fileSystemService.GetDocument(document.EntityId);

            document.FileName = infoToUpdate.FileName;
            document.FilePath = infoToUpdate.FilePath;
            document.FileNamePath = infoToUpdate.FileNamePath;
            document.UpdatedAt = infoToUpdate.UpdatedAt;
            document.UpdatedBy = infoToUpdate.UpdatedBy;

            return Task.CompletedTask;
        }

        private Task UpdateVersion(Document document)
        {
            var versions = _fileSystemService.GetVersions(document).Reverse().ToList();
            foreach (var version in versions.Where(version => version.VersionNumber.IsMajorVersion()).Where(version =>
                         document.Versions.All(x => x.VersionNumber != version.VersionNumber)))
                document.UpdateVersion(version);

            return Task.CompletedTask;
        }

        public async Task UpdateInfo(Document document)
        {
            await UpdateBaseInfo(document);
            await UpdateVersion(document);
        }

        public Task<Document> CreateBinding(Document document, int folderId)
        {
            var binding = _fileSystemService.CopySingleDocument(document, folderId);
            document.AddBinding(binding);

            return Task.FromResult(document);
        }

        public async Task UpdateBindings(Document document, Document specificBinding = null)
        {
            var bindingsToUpdate = specificBinding != null ? new[] {specificBinding} : document.Bindings;

            foreach (var binding in bindingsToUpdate)
            {
                var sourceVersions = _fileSystemService.GetVersions(document).ToList();
                var targetVersions = _fileSystemService.GetVersions(binding).ToList();

                foreach (var version in sourceVersions.Where(version =>
                             targetVersions.All(x => x.VersionNumber != version.VersionNumber)))
                {
                    var fileStream = await _fileSystemService.DownloadVersionAsync(version.VersionId);
                    await _fileSystemService.UpdateVersionAsync(binding, fileStream);

                    if (!version.VersionNumber.IsMajorVersion()) continue;
                    var newVersion = await _fileSystemService.PublishVersion(binding);
                    binding.UpdateVersion(newVersion);
                }
            }
        }
    }
}