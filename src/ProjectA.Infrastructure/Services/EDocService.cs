using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EDoc2.IAppService;
using EDoc2.IAppService.Model;
using EDoc2.Sdk;
using EDoc2.Sdk.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Services.Exceptions;

namespace ProjectA.Infrastructure.Services
{
    public class EDocService : IDMSService
    {
        private readonly IDocAppService _docAppService;
        private readonly IFileAppService _fileAppService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EDocService> _logger;
        private readonly IOrgAppService _orgAppService;
        private string _token = string.Empty;

        public EDocService(ILogger<EDocService> logger, HttpClient httpClient,
            IOrgAppService orgAppService, IFileAppService fileAppService, IDocAppService docAppService)
        {
            if (string.IsNullOrWhiteSpace(SdkBaseInfo.BaseUrl))
                throw new ArgumentNullException(nameof(SdkBaseInfo.BaseUrl), "SdkBaseInfo.BaseUrl必须先设置才能正常使用接口！");

            _logger = logger;
            _httpClient = httpClient;
            _orgAppService = orgAppService;
            _fileAppService = fileAppService;
            _docAppService = docAppService;
        }

        public async Task<MemoryStream> DownloadVersionAsync(int versionId)
        {
            _logger.LogInformation(nameof(DownloadVersionAsync));

            ValidateToken();

            var regionHash = await GetDownloadRegionHashAsync(versionId);

            // build download url
            var downloadUrl =
                $"{SdkBaseInfo.BaseUrl.TrimEnd('/')}/downLoad/index?token={_token}&regionHash={regionHash}&async=true";
            var byteArray = await _httpClient.GetByteArrayAsync(downloadUrl);

            Debug.WriteLine($"DOWNLOAD URL: {downloadUrl}");

            return new MemoryStream(byteArray);
        }

        public async Task<int> UpdateVersionAsync(int fileId, Stream fileStream)
        {
            _logger.LogInformation(nameof(UpdateVersionAsync));

            ValidateToken();

            // get file info
            var file = _fileAppService.GetFileInfoById(_token, fileId);
            if (file.Result != 0)
                throw new EDocApiException($"Unable to get file info with error code {file.Result}");

            // update file
            var updated = await Uploader.UpdateFileAsync(_token, fileId, file.Data.FileName
                , fileStream, file.Data.ParentFolderId, UpdateUpgradeStrategy.MinorUpgrade);

            return updated.File.FileVerId;
        }

        public int CopySingleDocument(int sourceId, int targetFolderId)
        {
            var copy = _docAppService.CopySingleFile(new CopySingleFileDto
                {FileId = sourceId, Token = _token, TargetFolderId = targetFolderId});
            if (copy.Result != 0)
                throw new EDocApiException(
                    $"Unable to copy file {sourceId} to folder {targetFolderId} with error code {copy.Result}.");
            return copy.Data.FileId;
        }

        public void PublishVersion(int fileId)
        {
            _fileAppService.PublishFileVersion(new FileDto
                {FileId = fileId, Token = _token});
        }

        public Document GetDocument(int fileId)
        {
            var file = _fileAppService.GetFileInfoById(_token, fileId);
            if (file.Result != 0) throw new EDocApiException("Unable to get file info from EDoc server.");

            return new Document(file.Data.FileId)
            {
                FileName = file.Data.FileName, FilePath = file.Data.FilePath, UpdatedAt = file.Data.FileModifyTime,
                UpdatedBy = file.Data.EditorName
            };
        }

        public IEnumerable<DocumentVersion> GetVersions(int fileId)
        {
            var versions = _fileAppService.GetFileVerListByFileId(_token, fileId);
            if (versions.Result != 0)
                throw new EDocApiException("Unable to get version info from EDoc server.");
            if (!versions.Data.Any())
                throw new EDocApiException("File version info is empty.");

            return versions.Data.Select(x => new DocumentVersion
            {
                VersionId = x.FileCurVerId,
                VersionNumber = DocumentVersionNumber.FromFileCurVerNumStr(x.FileCurVerNumStr)
            });
        }

        private async Task<string> GetDownloadRegionHashAsync(int versionId)
        {
            var checkUrl =
                $" {SdkBaseInfo.BaseUrl.TrimEnd('/')}/downLoad/DownLoadCheck?token={_token}&ver_id={versionId}&r={new Random().Next()}";
            var httpResponseMessage = await _httpClient.GetAsync(checkUrl);
            httpResponseMessage.EnsureSuccessStatusCode();

            Debug.WriteLine($"CHECK DOWNLOAD URL: {checkUrl}");

            var message = JsonConvert.DeserializeObject<dynamic>(await httpResponseMessage.Content.ReadAsStringAsync());
            if (message!.nResult != 0)
                throw new EDocApiException(
                    $"Unable to download file with error code {message.nResult}"); // throw if on error
            return message.RegionHash;
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
    }
}