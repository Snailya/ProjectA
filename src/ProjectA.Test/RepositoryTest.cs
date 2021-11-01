using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectA.Data;
using ProjectA.Models;
using ProjectA.Services;
using ProjectA.Services.Exceptions;

namespace ProjectA.Test
{
    [TestFixture]
    public class RepositoryTest : DatabaseFixture
    {
        [OneTimeSetUp]
        public void Init()
        {
            var services = ConfigureServices();
            services.AddSingleton<RepositoryService>();
            _serviceProvider = services.BuildServiceProvider();

            PopulateTestData(new DocumentContext(
                _serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>()));
        }

        private ServiceProvider _serviceProvider;

        protected override void PopulateTestData(DocumentContext dbContext)
        {
            base.PopulateTestData(dbContext);

            // append 
            var document = new Document(new Random().Next(), new Random().Next());
            document.UpdateSnapshot(dbContext.Documents.First());
            dbContext.SaveChanges();
        }

        [Test]
        public void ListDocuments_ReturnsDocumentList()
        {
            // arrange
            var repositoryService = _serviceProvider.GetRequiredService<RepositoryService>();

            // act
            var actual = repositoryService.List();

            // assert
            Assert.IsNotEmpty(actual);
        }

        [Test]
        [TestCase]
        [TestCase(1)]
        public void AddDocument_ReturnsDocument(int snapshotFolderId = default)
        {
            // arrange
            var repositoryService = _serviceProvider.GetRequiredService<RepositoryService>();
            var entityId = new Random().Next();

            // act
            var document = repositoryService.AddDocument(entityId, snapshotFolderId);

            // assert
            Assert.NotNull(document);
            Assert.AreEqual(snapshotFolderId, document.SnapshotFolderId);
        }

        [Test]
        public void AddDocument_ThrowsRepositoryException_IfDocumentAlreadyExistInDatabase()
        {
            // arrange
            var repositoryService = _serviceProvider.GetRequiredService<RepositoryService>();

            // act & assert
            Assert.Throws<RepositoryException>(() =>
            {
                repositoryService.AddDocument(repositoryService.List().First().EntityId);
            });
        }

        [Test]
        public void UpdateSnapshot_PersistANewSnapshotInDatabase_IfSnapshotNotExist()
        {
            // arrange
            var repositoryService = _serviceProvider.GetRequiredService<RepositoryService>();
            var document = repositoryService.List().First();
            var snapshotId = new Random().Next();

            // act
            document = repositoryService.UpdateSnapshot(document.EntityId, snapshotId);

            // assert
            Assert.AreEqual(snapshotId, document.Snapshot!.EntityId);
        }

        [Test]
        public void SetSnapshotFolderId_ReturnsDocumentWithNewSnapshotFolderId()
        {
            // arrange
            var repositoryService = _serviceProvider.GetRequiredService<RepositoryService>();
            var document = repositoryService.AddDocument(new Random().Next());
            var snapshotFolderId = new Random().Next();

            // act
            repositoryService.SetSnapshotFolder(document.EntityId, snapshotFolderId);

            // assert
            Assert.AreEqual(snapshotFolderId,
                repositoryService.List().Single(x => x.EntityId == document.EntityId).SnapshotFolderId);
        }

        [Test]
        public void SetSnapshotFolderId_ThrowsRepositoryException_IfSnapshotHasSet()
        {
            // arrange
            var repositoryService = _serviceProvider.GetRequiredService<RepositoryService>();
            var document = repositoryService.AddDocument(new Random().Next());
            document = repositoryService.UpdateSnapshot(document.EntityId, new Random().Next());

            // act & assert
            Assert.Throws<RepositoryException>(() =>
                repositoryService.SetSnapshotFolder(document!.EntityId, new Random().Next()));
        }

        [Test]
        public void DeleteDocument_ReturnsNoContent_AfterDelete()
        {
            // arrange
            var repositoryService = _serviceProvider.GetRequiredService<RepositoryService>();
            var entityId = repositoryService.List().First().EntityId;

            // act
            repositoryService.DeleteDocument(entityId);

            // assert
            Assert.Null(repositoryService.List().SingleOrDefault(x => x.EntityId == entityId));
        }
    }
}