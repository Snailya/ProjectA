using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EDoc2.IAppService;
using EDoc2.IAppService.Model;
using EDoc2.Sdk;
using EDoc2.Sdk.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectA.Core.Data;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models;
using ProjectA.Core.Services.Exceptions;

namespace ProjectA.Core.Services
{
    public class SynchronizeService : ISynchronizeService
    {
        private readonly HttpClient _client;
        private readonly IDbContextFactory<DocumentContext> _contextFactory;
        private readonly IFileAppService _fileAppService;
        private readonly ILogger<SynchronizeService> _logger;
        private readonly IOrgAppService _orgAppService;
        private string _token = string.Empty;

        public SynchronizeService(ILogger<SynchronizeService> logger, IDbContextFactory<DocumentContext> contextFactory,
            IOrgAppService orgAppService, IFileAppService fileAppService,
            HttpClient client)
        {
            if (string.IsNullOrWhiteSpace(SdkBaseInfo.BaseUrl))
                throw new ArgumentNullException(nameof(SdkBaseInfo.BaseUrl), "SdkBaseInfo.BaseUrl必须先设置才能正常使用接口！");

            _logger = logger;
            _contextFactory = contextFactory;
            _orgAppService = orgAppService;
            _fileAppService = fileAppService;
            _client = client;
        }

        private void ValidateToken()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                var checkTokenResult = _orgAppService.CheckUserTokenValidity(_token);
                if (checkTokenResult.Result == 0 && checkTokenResult.Data) return;
            }

            var userLoginDto = new UserLoginIntegrationByUserLoginNameDto
            {
                IntegrationKey = "46aa92ec-66af-4818-b7c1-8495a9bd7f17",
                IPAddress = "192.222.222.100",
                LoginName = "6470"
            };
            var loginResult = _orgAppService.UserLoginIntegrationByUserLoginName(userLoginDto);
            if (loginResult.Result == 0) _token = loginResult.Data;
        }

        private static DateTime ToDateTime(string dateTimeString)
        {
            return DateTime.Parse(dateTimeString);
        }

        private async Task<string> GetDownloadRegionHashAsync(int versionId)
        {
            var checkUrl =
                $" {SdkBaseInfo.BaseUrl.TrimEnd('/')}/downLoad/DownLoadCheck?token={_token}&ver_id={versionId}&r={new Random().Next()}";
            var httpResponseMessage = await _client.GetAsync(checkUrl);
            httpResponseMessage.EnsureSuccessStatusCode();

            Debug.WriteLine($"CHECK DOWNLOAD URL: {checkUrl}");

            var message = JsonConvert.DeserializeObject<dynamic>(await httpResponseMessage.Content.ReadAsStringAsync());
            if (message!.nResult != 0)
                throw new EDocApiException(
                    $"Unable to download file with error code {message.nResult}"); // throw if on error
            return message.RegionHash;
        }

        private async Task<Stream> DownloadSingleFileVersionStreamAsync(int versionId)
        {
            var regionHash = await GetDownloadRegionHashAsync(versionId);

            // build download url
            var downloadUrl =
                $"{SdkBaseInfo.BaseUrl.TrimEnd('/')}/downLoad/index?token={_token}&regionHash={regionHash}&async=true";
            var byteArray = await _client.GetByteArrayAsync(downloadUrl);

            Debug.WriteLine($"DOWNLOAD URL: {downloadUrl}");

            return new MemoryStream(byteArray);
        }

        #region Public Methods

        public async Task<int> Down()
        {
            _logger.LogInformation(nameof(Down));

            ValidateToken();

            await using var context = _contextFactory.CreateDbContext();

            foreach (var document in context.Documents.Include(x => x.Snapshot))
            {
                // update filename
                var fileInfoResult = _fileAppService.GetFileInfoById(_token, document.EntityId);
                if (fileInfoResult.Result == 0)
                {
                    document.FileName = fileInfoResult.Data.FileName;
                    document.FilePath = fileInfoResult.Data.FileNamePath;
                    document.UpdatedBy = fileInfoResult.Data.EditorName;
                    document.UpdatedAt = fileInfoResult.Data.FileModifyTime;
                }

                // get version info from EDoc Server
                var verListResult = _fileAppService.GetFileVerListByFileId(_token, document.EntityId);
                if (verListResult.Result != 0 || verListResult.Data == null) continue; // skip if failed to get version

                foreach (var eDocFileVerInfoResult in verListResult.Data.OrderBy(x => ToDateTime(x.FileCreateTime)))
                {
                    var versionNumber = VersionNumber.FromFileCurVerNumStr(eDocFileVerInfoResult.FileCurVerNumStr);
                    if (!versionNumber.IsMajorVersion()) continue; // skip minor version

                    // update database record
                    if (document.Versions.Any(x => x.VersionNumber == versionNumber)) continue;
                    document.UpdateVersion(new DocVersion
                        {VersionId = eDocFileVerInfoResult.FileCurVerId, VersionNumber = versionNumber});
                }
            }

            return await context.SaveChangesAsync();
        }

        public async Task<int> Up()
        {
            _logger.LogInformation(nameof(Up));

            ValidateToken();

            await using var context = _contextFactory.CreateDbContext();

            var documents =
                context.Documents.Include(x => x.Snapshot)
                    .AsEnumerable(); // cast to IEnumerable to avoid SnapshotNeedUpdate property not mapped exception
            var sources = documents.Where(x => x.SnapshotNeedUpdate).ToList();

            foreach (var document in sources)
            {
                var versionsNeedToUpdate = document.Versions.Where(x =>
                    x.VersionNumber > (document.Snapshot?.CurVersion == null
                        ? new VersionNumber(0, 0)
                        : document.Snapshot.CurVersion.VersionNumber)).ToList();
                foreach (var version in versionsNeedToUpdate)
                {
                    // get file name
                    var documentInfoResult = _fileAppService.GetFileInfoById(_token, document.EntityId);
                    var fileName = documentInfoResult.Data.FileName;
                    var fileStream = await DownloadSingleFileVersionStreamAsync(version.VersionId);
                    var uploadFileResult = document.Snapshot == null
                        ? await Uploader.UploadFileAsync(_token, fileName, fileStream, document.SnapshotFolderId)
                        : await Uploader.UpdateFileAsync(_token, document.Snapshot!.EntityId, fileName
                            , fileStream, document.SnapshotFolderId, UpdateUpgradeStrategy.MinorUpgrade);

                    // create snapshot if not record
                    if (document.Snapshot == null) document.UpdateSnapshot(new Document(uploadFileResult.File.FileId));

                    // publish version if is major version
                    if (version.VersionNumber.IsMajorVersion())
                        _fileAppService.PublishFileVersion(new FileDto
                            {FileId = uploadFileResult.File.FileId, Token = _token});
                    var snapshotInfoResult = _fileAppService.GetFileInfoById(_token, uploadFileResult.File.FileId);
                    document.Snapshot!.UpdateVersion(new DocVersion
                    {
                        VersionId = snapshotInfoResult.Data.CurrentVersionId,
                        VersionNumber = VersionNumber.FromFileCurVerNumStr(snapshotInfoResult.Data.FileCurVerNumStr)
                    });
                }
            }

            return await context.SaveChangesAsync();
        }

        #endregion
    }
}