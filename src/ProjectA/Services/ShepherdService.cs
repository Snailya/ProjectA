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
using Newtonsoft.Json;
using ProjectA.Data;
using ProjectA.Models;
using ProjectA.Services.Exceptions;

namespace ProjectA.Services
{
    public class ShepherdService
    {
        private readonly HttpClient _client;
        private readonly IDbContextFactory<DocumentContext> _contextFactory;
        private readonly IFileAppService _fileAppService;
        private readonly IOrgAppService _orgAppService;
        private string _token = string.Empty;

        public ShepherdService(IDbContextFactory<DocumentContext> contextFactory,
            IOrgAppService orgAppService, IFileAppService fileAppService,
            HttpClient client)
        {
            if (string.IsNullOrWhiteSpace(SdkBaseInfo.BaseUrl))
                throw new ArgumentNullException(nameof(SdkBaseInfo.BaseUrl), "SdkBaseInfo.BaseUrl必须先设置才能正常使用接口！");

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

        public async Task<int> SyncDocVersionsFromEDocAsync()
        {
            ValidateToken();

            await using var context = _contextFactory.CreateDbContext();

            foreach (var document in context.Documents.Include(x => x.Snapshot))
            {
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

        public async Task<int> UpdateSnapshotInTargetFolderAsync()
        {
            ValidateToken();

            await using var context = _contextFactory.CreateDbContext();

            var sources = context.Documents.Include(x => x.Snapshot)
                .Where(x => x.SnapshotFolderId != default || x.Snapshot != null).ToList();
            var validSources = sources.Where(x => x.Versions.Any() && x.CurVersion.VersionNumber >
                (x.Snapshot == null ? new VersionNumber(0, 0) : x.Snapshot.CurVersion.VersionNumber)).ToList();

            foreach (var document in validSources)
            {
                Debug.WriteLine($"SYNCHRONIZING FILE ID: {document.EntityId}");

                var versionsNeedToUpdate = document.Versions.Where(x =>
                    x.VersionNumber > (document.Snapshot == null
                        ? new VersionNumber(0, 0)
                        : document.Snapshot.CurVersion.VersionNumber)).ToList();
                foreach (var version in versionsNeedToUpdate)
                {
                    Debug.WriteLine($"VERSION ID: {version.VersionId}");
                    Debug.WriteLine($"VERSION NUMBER: {version.VersionNumber}");

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