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
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Infrastructure.Data;
using ProjectA.Infrastructure.Services;
using ProjectA.Infrastructure.Services.Exceptions;

// namespace ProjectA.Test
// {
//     public class ShepherdTest : DatabaseFixture
//     {
//         [OneTimeSetUp]
//         public void Init()
//         {
//             var services = new ServiceCollection();
//             ConfigureServices(services);
//
//             // register eDoc service
//             SdkBaseInfo.BaseUrl = "http://doc.scivic.com.cn:8889";
//             foreach (var type in AppDomain.CurrentDomain.GetAllTypes().Where(type =>
//                          type.IsInterface && typeof(IApplicationService).IsAssignableFrom(type)))
//                 services.AddSingleton(type, builder =>
//                 {
//                     var client = new HttpApiClient(SdkBaseInfo.BaseUrl);
//                     return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, client);
//                 });
//
//             // register other services
//             services.AddScoped<HttpClient>();
//             services.AddSingleton<IDocService, DocService>();
//
//             // build service provider
//             Build(services);
//         }
//
//         [SetUp]
//         public void Setup()
//         {
//             ClearFileCachesInBothSourceAndTargetFolders();
//             ClearTestData();
//         }
//
//         private FileVersionInfoResult UploadAndPublishFileInFolder(int folderId)
//         {
//             var fileService = ServiceProvider.GetRequiredService<IFileAppService>();
//             var uploadFileResult =
//                 Uploader.UploadFile(GetToken(), TestFilePath, folderId, UpgradeStrategy.MajorUpgrade);
//             return fileService.PublishFileVersion(new FileDto
//                 {FileId = uploadFileResult.File.FileId, Token = GetToken()}).Data;
//         }
//
//         private FileVersionInfoResult UpdateAndPublishFileInFolder(int entityId, int folderId)
//         {
//             var fileService = ServiceProvider.GetRequiredService<IFileAppService>();
//             var uploadFileResult =
//                 Uploader.UpdateFile(GetToken(), entityId, TestFilePath, folderId, UpdateUpgradeStrategy.MinorUpgrade);
//             return fileService.PublishFileVersion(new FileDto
//                 {FileId = uploadFileResult.File.FileId, Token = GetToken()}).Data;
//         }
//
//         private const string TestFilePath = "TESTFILE";
//         private const int SourceFolderId = 75695;
//         private const int TargetFolderId = 75696;
//
//
//         [Test]
//         public async Task SyncDocVersionsFromEDoc_UpdateDocVersions()
//         {
//             // arrange
//             var fileVersionInfoResult = UploadAndPublishFileInFolder(SourceFolderId);
//             var document = new Document(fileVersionInfoResult.FileId);
//
//             var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
//             using (var dbContext = dbContextFactory.CreateDbContext())
//             {
//                 dbContext.Add(document);
//                 await dbContext.SaveChangesAsync();
//             }
//
//             // act
//             var shepherdService = ServiceProvider.GetRequiredService<IDocService>();
//             await shepherdService.BatchDownAsync();
//             int actual;
//             using (var dbContext = dbContextFactory.CreateDbContext())
//             {
//                 actual = dbContext.Documents.Single(x => x.EntityId == fileVersionInfoResult.FileId)!.CurVersion!
//                     .VersionId;
//             }
//
//             // assert
//             Assert.AreEqual(fileVersionInfoResult.FileCurVerId,
//                 actual);
//         }
//
//         [Test]
//         public async Task SyncSnapshot_CopyDocToFolder_IfNotExist()
//         {
//             // arrange
//             var sourceFileVersionInfo = UploadAndPublishFileInFolder(SourceFolderId);
//             var document = new Document(sourceFileVersionInfo.FileId, TargetFolderId);
//             document.UpdateVersion(new DocumentVersion
//             {
//                 VersionId = sourceFileVersionInfo.FileCurVerId,
//                 VersionNumber = DocumentVersionNumber.FromFileCurVerNumStr(sourceFileVersionInfo.FileCurVerNumStr)
//             });
//
//             var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
//             using (var dbContext = dbContextFactory.CreateDbContext())
//             {
//                 dbContext.Add(document);
//                 await dbContext.SaveChangesAsync();
//             }
//
//             // act & assert
//             var shepherdService = ServiceProvider.GetRequiredService<IDocService>();
//             await shepherdService.BatchSynchronizeDocumentsAsync();
//
//             // act & assert
//             Document snapshot;
//             using (var dbContext = dbContextFactory.CreateDbContext())
//             {
//                 snapshot = dbContext.Documents.Include(x => x.LinkedDoc).Single(x => x.EntityId == document.EntityId)
//                     .LinkedDoc;
//             }
//
//             Assert.NotNull(snapshot);
//
//             // act & assert
//             var snapshotFileInfo = GetFileVersionInfo(snapshot.EntityId);
//
//             Assert.AreEqual(snapshotFileInfo.CurrentVersionId, snapshot.CurVersion!.VersionId);
//             Assert.AreEqual(snapshotFileInfo.FileCurVerNumStr, snapshot.CurVersion.VersionNumber.ToString());
//         }
//
//         private FileInfoForSdkResult GetFileVersionInfo(int entityId)
//         {
//             var fileService = ServiceProvider.GetRequiredService<IFileAppService>();
//             return fileService.GetFileInfoById(GetToken(), entityId).Data;
//         }
//
//         [Test]
//         public async Task SyncSnapshot_UpdateDocInFolder_IfVersionNotMatch()
//         {
//             // arrange
//             var fileVersionInfoResult = UploadAndPublishFileInFolder(SourceFolderId);
//             fileVersionInfoResult = UpdateAndPublishFileInFolder(fileVersionInfoResult.FileId, SourceFolderId);
//             var document = new Document(fileVersionInfoResult.FileId, TargetFolderId);
//             document.UpdateVersion(new DocumentVersion
//             {
//                 VersionId = fileVersionInfoResult.FileCurVerId,
//                 VersionNumber = DocumentVersionNumber.FromFileCurVerNumStr(fileVersionInfoResult.FileCurVerNumStr)
//             });
//
//             fileVersionInfoResult = UploadAndPublishFileInFolder(TargetFolderId);
//             var snapshot = new Document(fileVersionInfoResult.FileId);
//             snapshot.UpdateVersion(new DocumentVersion
//             {
//                 VersionId = fileVersionInfoResult.FileCurVerId,
//                 VersionNumber = DocumentVersionNumber.FromFileCurVerNumStr(fileVersionInfoResult.FileCurVerNumStr)
//             });
//
//             var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
//             using (var dbContext = dbContextFactory.CreateDbContext())
//             {
//                 dbContext.Add(document);
//                 document.MakeLink(snapshot);
//                 await dbContext.SaveChangesAsync();
//             }
//
//             // act & assert
//             var shepherdService = ServiceProvider.GetRequiredService<IDocService>();
//             await shepherdService.BatchSynchronizeDocumentsAsync();
//
//             // act & assert
//             DocumentVersionNumber documentDocumentVersionNumber;
//             DocumentVersionNumber snapshotDocumentVersionNumber;
//             using (var dbContext = dbContextFactory.CreateDbContext())
//             {
//                 documentDocumentVersionNumber = dbContext.Documents.Include(x => x.LinkedDoc)
//                     .Single(x => x.EntityId == document.EntityId)
//                     .CurVersion!.VersionNumber;
//                 snapshotDocumentVersionNumber = dbContext.Documents.Include(x => x.LinkedDoc)
//                     .Single(x => x.EntityId == document.EntityId)
//                     .LinkedDoc!
//                     .CurVersion!.VersionNumber;
//             }
//
//             Assert.AreEqual(documentDocumentVersionNumber, snapshotDocumentVersionNumber);
//
//             // act & assert
//             var snapshotFileInfo = GetFileVersionInfo(snapshot.EntityId);
//
//             Assert.AreEqual(snapshotFileInfo.FileCurVerNumStr, snapshotDocumentVersionNumber.ToString());
//         }
//
//         private void ClearFileCachesInBothSourceAndTargetFolders()
//         {
//             foreach (var folderId in new[] {TargetFolderId, SourceFolderId}) RemoveFilesInFolder(folderId);
//         }
//
//         private void RemoveFilesInFolder(int folderId)
//         {
//             var docService = ServiceProvider.GetRequiredService<IDocAppService>();
//             var childrenListResult = docService.GetChildListByFolderId(GetToken(), folderId);
//             if (childrenListResult.Result == 0)
//                 docService.RemoveFolderListAndFileList(new FileListAndFolderListDto
//                 {
//                     FileIdList = childrenListResult.Data.FilesInfo.Select(x => x.FileId).ToList(),
//                     Token = GetToken()
//                 });
//         }
//
//
//         private string GetToken()
//         {
//             var orgAppService = ServiceProvider.GetRequiredService<IOrgAppService>();
//
//             var userLoginDto = new UserLoginIntegrationByUserLoginNameDto
//             {
//                 IntegrationKey = "46aa92ec-66af-4818-b7c1-8495a9bd7f17",
//                 IPAddress = "192.222.222.100",
//                 LoginName = "6470"
//             };
//             var loginResult = orgAppService.UserLoginIntegrationByUserLoginName(userLoginDto);
//             if (loginResult.Result == 0) return loginResult.Data;
//
//             throw new EDocApiException("Unable to get token.");
//         }
//     }
// }