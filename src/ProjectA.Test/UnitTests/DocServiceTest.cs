using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ProjectA.Core.Interfaces;
using ProjectA.Core.Models.DocAggregate;
using ProjectA.Core.Models.DocAggregate.Specifications;
using ProjectA.Core.Services;
using ProjectA.Infrastructure.Services;
using ProjectA.SharedKernel.Interfaces;

namespace ProjectA.Test.UnitTests
{
    [TestFixture]
    public class DocServiceTest
    {
        [TearDown]
        public void ClearMockSetup()
        {
            _repositoryMock.Invocations.Clear();
            _dmsServiceMock.Invocations.Clear();
        }

        private readonly Mock<ILogger<DocumentHelper>> _loggerMock = new();
        private readonly Mock<IRepository<Document>> _repositoryMock = new();
        private readonly Mock<IFileSystemService> _dmsServiceMock = new();
        private readonly IDocumentHelper _documentHelper;

        public DocServiceTest()
        {
            _documentHelper = new DocumentHelper(_loggerMock.Object, _repositoryMock.Object,
                _dmsServiceMock.Object);
        }

        [Test]
        public async Task Down_ReturnsNewestDocument_IfDocumentExist()
        {
            // arrange
            var entityId = new Random().Next();

            var documentRemoted = new Document(entityId) {FileName = "Test File"};
            documentRemoted.UpdateVersion(new DocumentVersion
                {VersionId = new Random().Next(), VersionNumber = new DocumentVersionNumber(1, 0)});

            var documentLocal = new Document(entityId);
            _repositoryMock.As<IRepository<Document>>().Setup(x =>
                    x.GetBySpecAsync(It.IsAny<DocumentByEntityIdSpec>(), new CancellationToken()))
                .ReturnsAsync(documentLocal);
            _dmsServiceMock.Setup(x => x.GetVersions(entityId)).Returns(documentRemoted.Versions);
            _dmsServiceMock.Setup(x => x.GetDocument(entityId)).Returns(documentRemoted);

            // act
            var actual = await _documentHelper.Down(entityId);

            // assert
            _repositoryMock.Verify(x => x.GetBySpecAsync(It.IsAny<DocumentByEntityIdSpec>(), new CancellationToken()),
                Times.Once);
            Assert.AreEqual(documentRemoted.FileName, actual.FileName);
            Assert.AreEqual(documentRemoted.Versions.Last().VersionId, actual.Versions.Last().VersionId);
        }

        [Test]
        public void Down_ThrowsException_IfDocumentNotExist()
        {
            // arrange
            var entityId = new Random().Next();

            _repositoryMock.As<IRepository<Document>>().Setup(x =>
                    x.GetBySpecAsync(It.IsAny<DocumentByEntityIdSpec>(), new CancellationToken()))
                .ReturnsAsync((Document) null);

            // act & assert
            Assert.ThrowsAsync<Exception>(() => _documentHelper.Down(entityId));
        }

        [Test]
        public async Task CopyDocumentAsync_InvokeCopySingleDocumentOnce()
        {
            // arrange
            _dmsServiceMock.Setup(x => x.CopySingleDocument(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Random().Next);

            // act
            var copy = await _documentHelper.CopyDocumentAsync(new Random().Next(), new Random().Next());

            // assert
            _dmsServiceMock.Verify(x => x.CopySingleDocument(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task SynchronizeDocumentAsync_InvokeMethodsRightTimes()
        {
            // arrange
            var sourceId = new Random().Next();
            var sourceVersions = new List<DocumentVersion>
            {
                new() {VersionNumber = new DocumentVersionNumber(1, 0), VersionId = new Random().Next()},
                new() {VersionNumber = new DocumentVersionNumber(2, 0), VersionId = new Random().Next()}
            };
            var targetId = new Random().Next();
            var targetVersions = new List<DocumentVersion>
            {
                new() {VersionNumber = new DocumentVersionNumber(1, 0), VersionId = new Random().Next()}
            };

            _dmsServiceMock.Setup(x => x.GetVersions(sourceId))
                .Returns(sourceVersions);
            _dmsServiceMock.Setup(x => x.GetVersions(targetId))
                .Returns(targetVersions);
            _dmsServiceMock.Setup(x => x.DownloadVersionAsync(It.IsAny<int>()))
                .ReturnsAsync(new MemoryStream());
            _dmsServiceMock.Setup(x => x.UpdateVersionAsync(It.IsAny<int>(), It.IsAny<Stream>()));

            // act
            await _documentHelper.SynchronizeDocumentOnFileSystem(sourceId, targetId);

            // assert
            _dmsServiceMock.Verify(x => x.GetVersions(It.IsAny<int>()), Times.Exactly(2));
            _dmsServiceMock.Verify(x => x.DownloadVersionAsync(It.IsAny<int>()), Times.Once);
            _dmsServiceMock.Verify(x => x.UpdateVersionAsync(It.IsAny<int>(), It.IsAny<Stream>()), Times.Once);
        }
    }
}