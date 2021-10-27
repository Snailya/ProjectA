using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectA.Models;
using ProjectA.Services;

namespace ProjectA.Test
{
    public class RepositoryTest
    {
        private RepositoryService _service;
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection().AddSingleton<RepositoryService>()
                .AddDbContext<DocumentContext>(options => options.UseInMemoryDatabase("document"))
                .BuildServiceProvider();

            _service = _serviceProvider.GetRequiredService<RepositoryService>();

            PrepareTestData();
        }

        private void PrepareTestData()
        {
            using var dbContext = new DocumentContext(
                _serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());

            // clear test database
            if (dbContext.Documents.Any())
                foreach (var item in dbContext.Documents)
                    dbContext.Documents.Remove(item);
            dbContext.SaveChanges();

            // instance a document as snapshot, and another document link with this snapshot
            var document = new Document(new Random().Next());
            var documentWithSnapshot = new Document(new Random().Next(), new Random().Next());
            documentWithSnapshot.UpdateSnapshot(document);

            // persist
            dbContext.Documents.Add(document);
            dbContext.Documents.Add(documentWithSnapshot);
            dbContext.SaveChanges();
        }

        [Test]
        public void ListDocuments_ReturnsTwoPresetDocuments()
        {
            var documents = _service.List().ToList();

            Assert.NotNull(documents.SingleOrDefault(x => x.Snapshot != null));
            Assert.NotNull(documents.SingleOrDefault(x => x.Snapshot == null));
        }

        [Test]
        [TestCase(668407, 75696)]
        [TestCase(668421)]
        public void AddDocument_ReturnsDocument(int entityId,
            int snapshotFolderId = default)
        {
            _service.AddDocument(entityId, snapshotFolderId);

            var document = _service.List().SingleOrDefault(x => x.EntityId == entityId);
            Assert.NotNull(document);
            Assert.AreEqual(snapshotFolderId, document.SnapshotFolderId);
        }

        [Test]
        public void AddDocument_ThrowsRepositoryException_IfDocumentAlreadyExistInDatabase()
        {
            // get an exist document id
            var entityId = _service.List().First().EntityId;

            Assert.Throws<RepositoryException>(() => { _service.AddDocument(entityId); });
        }

        [Test]
        public void SetSnapshotFolderId_ReturnsDocumentWithNewSnapshotFolderId()
        {
            var snapshotFolderId = new Random().Next();

            var document = _service.List()
                .First(x => x.SnapshotFolderId is default(int));
            _service.SetSnapshotFolder(document.EntityId, snapshotFolderId);

            Assert.AreEqual(snapshotFolderId,
                _service.List().Single(x => x.EntityId == document.EntityId).SnapshotFolderId);
        }

        [Test]
        public void SetSnapshotFolderId_ThrowsRepositoryException_IfSnapshotHasSet()
        {
            var document = _service.List().ToList().FirstOrDefault(x => x.Snapshot != null);

            Assert.Throws<RepositoryException>(() =>
                _service.SetSnapshotFolder(document!.EntityId, new Random().Next()));
        }

        [Test]
        public void DeleteDocument_ReturnsNoContent_AfterDelete()
        {
            var entityId = _service.List().First().EntityId;

            _service.DeleteDocument(entityId);

            Assert.Null(_service.List().SingleOrDefault(x => x.EntityId == entityId));
        }
    }
}