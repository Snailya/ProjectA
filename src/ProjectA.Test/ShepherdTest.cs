using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EDoc2.IAppService;
using EDoc2.IAppService.Model;
using EDoc2.IAppService.ResultModel;
using EDoc2.Sdk;
using EDoc2.Sdk.Models;
using EDoc2.ServiceProxy;
using EDoc2.ServiceProxy.Client;
using EDoc2.ServiceProxy.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectA.Core.Data;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models;
using ProjectA.Core.Services;
using ProjectA.Core.Services.Exceptions;

namespace ProjectA.Test
{
    public class ShepherdTest : DatabaseFixture
    {
        [OneTimeSetUp]
        public void Init()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            // register eDoc service
            SdkBaseInfo.BaseUrl = "http://doc.scivic.com.cn:8889";
            foreach (var type in AppDomain.CurrentDomain.GetAllTypes().Where(type =>
                         type.IsInterface && typeof(IApplicationService).IsAssignableFrom(type)))
                services.AddSingleton(type, builder =>
                {
                    var client = new HttpApiClient(SdkBaseInfo.BaseUrl);
                    return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, client);
                });

            // register other services
            services.AddScoped<HttpClient>();
            services.AddSingleton<IShepherdService, ShepherdService>();

            // build service provider
            Build(services);
        }

        [SetUp]
        public void Setup()
        {
            ClearFileCachesInBothSourceAndTargetFolders();
            ClearTestData();
        }

        private FileVersionInfoResult UploadAndPublishFileInFolder(int folderId)
        {
            var fileService = ServiceProvider.GetRequiredService<IFileAppService>();
            var uploadFileResult =
                Uploader.UploadFile(GetToken(), TestFilePath, folderId, UpgradeStrategy.MajorUpgrade);
            return fileService.PublishFileVersion(new FileDto
                {FileId = uploadFileResult.File.FileId, Token = GetToken()}).Data;
        }

        private FileVersionInfoResult UpdateAndPublishFileInFolder(int entityId, int folderId)
        {
            var fileService = ServiceProvider.GetRequiredService<IFileAppService>();
            var uploadFileResult =
                Uploader.UpdateFile(GetToken(), entityId, TestFilePath, folderId, UpdateUpgradeStrategy.MinorUpgrade);
            return fileService.PublishFileVersion(new FileDto
                {FileId = uploadFileResult.File.FileId, Token = GetToken()}).Data;
        }

        private const string TestFilePath = "TESTFILE";
        private const int SourceFolderId = 75695;
        private const int TargetFolderId = 75696;


        [Test]
        public async Task SyncDocVersionsFromEDoc_UpdateDocVersions()
        {
            // arrange
            var fileVersionInfoResult = UploadAndPublishFileInFolder(SourceFolderId);
            var document = new Document(fileVersionInfoResult.FileId);

            await using (var dbContext = new DocumentContext(
                             ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>()))
            {
                dbContext.Add(document);
                await dbContext.SaveChangesAsync();
            }

            // act
            var shepherdService = ServiceProvider.GetRequiredService<IShepherdService>();
            var affected = await shepherdService.SyncDocVersionsFromEDocAsync();
            int actual;
            await using (var dbContext = new DocumentContext(
                             ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>()))
            {
                actual = dbContext.Documents.Single(x => x.EntityId == fileVersionInfoResult.FileId)!.CurVersion!
                    .VersionId;
            }

            // assert
            Assert.NotZero(affected);
            Assert.AreEqual(fileVersionInfoResult.FileCurVerId,
                actual);
        }

        [Test]
        public async Task SyncSnapshot_CopyDocToFolder_IfNotExist()
        {
            // arrange
            var sourceFileVersionInfo = UploadAndPublishFileInFolder(SourceFolderId);
            var document = new Document(sourceFileVersionInfo.FileId, TargetFolderId);
            document.UpdateVersion(new DocVersion
            {
                VersionId = sourceFileVersionInfo.FileCurVerId,
                VersionNumber = VersionNumber.FromFileCurVerNumStr(sourceFileVersionInfo.FileCurVerNumStr)
            });

            await using (var dbContext = new DocumentContext(
                             ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>()))
            {
                dbContext.Add(document);
                await dbContext.SaveChangesAsync();
            }

            // act & assert
            var shepherdService = ServiceProvider.GetRequiredService<IShepherdService>();
            var affected = await shepherdService.UpdateSnapshotInTargetFolderAsync();

            Assert.NotZero(affected);

            // act & assert
            Document snapshot;
            await using (var dbContext = new DocumentContext(
                             ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>()))
            {
                snapshot = dbContext.Documents.Include(x => x.Snapshot).Single(x => x.EntityId == document.EntityId)
                    .Snapshot;
            }

            Assert.NotNull(snapshot);

            // act & assert
            var snapshotFileInfo = GetFileVersionInfo(snapshot.EntityId);

            Assert.AreEqual(snapshotFileInfo.CurrentVersionId, snapshot.CurVersion!.VersionId);
            Assert.AreEqual(snapshotFileInfo.FileCurVerNumStr, snapshot.CurVersion.VersionNumber.ToString());
        }

        private FileInfoForSdkResult GetFileVersionInfo(int entityId)
        {
            var fileService = ServiceProvider.GetRequiredService<IFileAppService>();
            return fileService.GetFileInfoById(GetToken(), entityId).Data;
        }

        [Test]
        public async Task SyncSnapshot_UpdateDocInFolder_IfVersionNotMatch()
        {
            // arrange
            var fileVersionInfoResult = UploadAndPublishFileInFolder(SourceFolderId);
            fileVersionInfoResult = UpdateAndPublishFileInFolder(fileVersionInfoResult.FileId, SourceFolderId);
            var document = new Document(fileVersionInfoResult.FileId, TargetFolderId);
            document.UpdateVersion(new DocVersion
            {
                VersionId = fileVersionInfoResult.FileCurVerId,
                VersionNumber = VersionNumber.FromFileCurVerNumStr(fileVersionInfoResult.FileCurVerNumStr)
            });

            fileVersionInfoResult = UploadAndPublishFileInFolder(TargetFolderId);
            var snapshot = new Document(fileVersionInfoResult.FileId);
            snapshot.UpdateVersion(new DocVersion
            {
                VersionId = fileVersionInfoResult.FileCurVerId,
                VersionNumber = VersionNumber.FromFileCurVerNumStr(fileVersionInfoResult.FileCurVerNumStr)
            });

            await using (var dbContext = new DocumentContext(
                             ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>()))
            {
                dbContext.Add(document);
                document.UpdateSnapshot(snapshot);
                await dbContext.SaveChangesAsync();
            }

            // act & assert
            var shepherdService = ServiceProvider.GetRequiredService<IShepherdService>();
            var affected = await shepherdService.UpdateSnapshotInTargetFolderAsync();

            Assert.NotZero(affected);

            // act & assert
            VersionNumber documentVersionNumber;
            VersionNumber snapshotVersionNumber;
            await using (var dbContext = new DocumentContext(
                             ServiceProvider.GetRequiredService<DbContextOptions<DocumentContext>>()))
            {
                documentVersionNumber = dbContext.Documents.Include(x => x.Snapshot)
                    .Single(x => x.EntityId == document.EntityId)
                    .CurVersion!.VersionNumber;
                snapshotVersionNumber = dbContext.Documents.Include(x => x.Snapshot)
                    .Single(x => x.EntityId == document.EntityId)
                    .Snapshot!
                    .CurVersion!.VersionNumber;
            }

            Assert.AreEqual(documentVersionNumber, snapshotVersionNumber);

            // act & assert
            var snapshotFileInfo = GetFileVersionInfo(snapshot.EntityId);

            Assert.AreEqual(snapshotFileInfo.FileCurVerNumStr, snapshotVersionNumber.ToString());
        }

        private void ClearFileCachesInBothSourceAndTargetFolders()
        {
            foreach (var folderId in new[] {TargetFolderId, SourceFolderId}) RemoveFilesInFolder(folderId);
        }

        private void RemoveFilesInFolder(int folderId)
        {
            var docService = ServiceProvider.GetRequiredService<IDocAppService>();
            var childrenListResult = docService.GetChildListByFolderId(GetToken(), folderId);
            if (childrenListResult.Result == 0)
                docService.RemoveFolderListAndFileList(new FileListAndFolderListDto
                {
                    FileIdList = childrenListResult.Data.FilesInfo.Select(x => x.FileId).ToList(),
                    Token = GetToken()
                });
        }


        private string GetToken()
        {
            var orgAppService = ServiceProvider.GetRequiredService<IOrgAppService>();

            var userLoginDto = new UserLoginIntegrationByUserLoginNameDto
            {
                IntegrationKey = "46aa92ec-66af-4818-b7c1-8495a9bd7f17",
                IPAddress = "192.222.222.100",
                LoginName = "6470"
            };
            var loginResult = orgAppService.UserLoginIntegrationByUserLoginName(userLoginDto);
            if (loginResult.Result == 0) return loginResult.Data;

            throw new EDocApiException("Unable to get token.");
        }
    }
}