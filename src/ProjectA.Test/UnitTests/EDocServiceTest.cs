namespace ProjectA.Test.UnitTests
{
    public class EDocServiceTest
    {
        // [Test]
        // public void UpdateDocumentAsync_ShouldFillUpDocumentInfo_IfDocumentVersionExist()
        // {
        //     // arrange
        //     var entityId = new Random().Next();
        //     const string fileName = "JetSnail";
        //     const string versionStr = "1.0";
        //     var version = new EDocFileVerInfoResult {FileName = fileName, FileCurVerNumStr = versionStr};
        //     _fileAppServiceMock.Setup(x => x.GetFileVerListByFileId(It.IsAny<string>(), entityId))
        //         .Returns(new ReturnValueResult<List<EDocFileVerInfoResult>>
        //             {Result = 0, Data = new List<EDocFileVerInfoResult> {version}});
        //
        //     var document = new Document(entityId);
        //
        //     // act 
        //     _dmsService.UpdateDocumentAsync(document);
        //
        //     // assert
        //     Assert.IsNotEmpty(document.Versions);
        //     Assert.AreEqual(fileName, document.FileName);
        //     Assert.AreEqual(versionStr, document.Versions.Last().VersionNumber.ToString());
        // }
        //
        // [Test]
        // public void UpdateDocumentAsync_ShouldThrowEDocApiException_IfFailedToGetVersionList()
        // {
        //     // arrange
        //     var entityId = new Random().Next();
        //     _fileAppServiceMock.Setup(x => x.GetFileVerListByFileId(It.IsAny<string>(), entityId))
        //         .Returns(new ReturnValueResult<List<EDocFileVerInfoResult>>
        //             {Result = -1});
        //
        //     var document = new Document(entityId);
        //
        //     // act & assert
        //     Assert.Throws<EDocApiException>(() => _dmsService.UpdateDocumentAsync(document));
        // }
        //
        // [Test]
        // public void UpdateDocumentAsync_ShouldThrowEDocApiException_IfDocumentHasNoVersionList()
        // {
        //     // arrange
        //     var entityId = new Random().Next();
        //     _fileAppServiceMock.Setup(x => x.GetFileVerListByFileId(It.IsAny<string>(), entityId))
        //         .Returns(new ReturnValueResult<List<EDocFileVerInfoResult>>
        //         {
        //             Result = 0, Data = new List<EDocFileVerInfoResult>()
        //         });
        //
        //     var document = new Document(entityId);
        //
        //     // act & assert
        //     Assert.Throws<EDocApiException>(() => _dmsService.UpdateDocumentAsync(document));
        // }
    }
}