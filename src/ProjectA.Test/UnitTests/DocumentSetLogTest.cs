using System;
using System.Linq;
using NUnit.Framework;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Test.UnitTests
{
    [TestFixture]
    public class DocumentSetLogTest
    {
        [Test]
        public void UpdateDocumentSetMember_CreateAUpdateLog_IfDocumentVersionHasChanged()
        {
            // arrange
            var documentSet = new DocumentSet {Name = "Test Set"};
            var document = new Document(new Random().Next());
            document.UpdateVersion(new DocumentVersion
                {VersionId = new Random().Next(), VersionNumber = new DocumentVersionNumber(1, 0)});
            documentSet.AddDocument(document);

            var newVersion = new DocumentVersion
                {VersionId = new Random().Next(), VersionNumber = new DocumentVersionNumber(2, 0)};
            document.UpdateVersion(newVersion);

            // act
            documentSet.UpdateDocument(document);

            // assert
            Assert.AreEqual(DocumentSetLogType.Updated, documentSet.Logs.Last().Type);
        }

        [Test]
        public void UpdateDocumentSetMember_Ignored_IfDocumentVersionNotChanged()
        {
            // arrange
            var documentSet = new DocumentSet {Name = "Test Set"};
            var document = new Document(new Random().Next());
            document.UpdateVersion(new DocumentVersion
                {VersionId = new Random().Next(), VersionNumber = new DocumentVersionNumber(1, 0)});
            documentSet.AddDocument(document);

            // act
            documentSet.UpdateDocument(document);

            // assert
            Assert.AreEqual(DocumentSetLogType.Created, documentSet.Logs.Last().Type);
        }

        [Test]
        public void UpdateDocument_Throws_IfDocumentIsNotTheMemberOfTheSet()
        {
            // arrange
            var documentSet = new DocumentSet {Name = "Test Set"};
            var document = new Document(new Random().Next());
            document.UpdateVersion(new DocumentVersion
                {VersionId = new Random().Next(), VersionNumber = new DocumentVersionNumber(1, 0)});

            // act & assert
            Assert.Throws<InvalidOperationException>(() => documentSet.UpdateDocument(document));
        }

        [Test]
        public void GenerateTrackingReports_SkipLog_IfDocumentIsAddThenDeletedWithinPeriod()
        {
            // arrange
            var from = DateTime.Now;
            var documentSet = new DocumentSet {Name = "Test Set"};
            var document = new Document(new Random().Next()) {FileName = "Test File"};
            document.UpdateVersion(new DocumentVersion
                {VersionId = new Random().Next(), VersionNumber = new DocumentVersionNumber(1, 0)});
            documentSet.AddDocument(document);
            documentSet.RemoveDocument(document);

            // act
            var reports = documentSet.GenerateTrackingReports(from);

            // assert
            Assert.IsEmpty(reports);
        }

        [Test]
        public void GenerateTrackingReports_ReturnACreateLogWithLatestVersion_IfDocumentIsAddThenUpdatedWithinPeriod()
        {
            // arrange
            var from = DateTime.Now - TimeSpan.FromDays(1);
            var documentSet = new DocumentSet {Name = "Test Set"};
            var document = new Document(new Random().Next()) {FileName = "Test File"};
            document.UpdateVersion(new DocumentVersion
                {VersionId = new Random().Next(), VersionNumber = new DocumentVersionNumber(1, 0)});
            documentSet.AddDocument(document);
            document.UpdateVersion(new DocumentVersion
                {VersionId = new Random().Next(), VersionNumber = new DocumentVersionNumber(2, 0)});
            documentSet.UpdateDocument(document);

            // act
            var reports = documentSet.GenerateTrackingReports(from);

            // assert
            Assert.AreEqual(new DocumentVersionNumber(2, 0).ToString(),
                reports.Single(x => x.Name == document.FileName).VersionNumber);
        }
    }
}