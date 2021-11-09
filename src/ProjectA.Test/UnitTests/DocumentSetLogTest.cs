using System;
using System.Linq;
using NUnit.Framework;
using ProjectA.Core.Models.DocSetAggregate;

namespace ProjectA.Test.UnitTests
{
    [TestFixture]
    public class DocumentSetLogTest
    {
        [Test]
        public void AddDocument_AddANewDocumentWithAnAddLog_IfDocumentNotIncludeInDocumentSet()
        {
            // arrange
            var set = new DocumentSet();
            var doc = Guid.NewGuid();

            // act 
            set.Add(doc);

            // assert
            Assert.AreEqual(doc,set.Documents.LastOrDefault());
            Assert.AreEqual(doc,set.Logs.Last().Document);
            Assert.AreEqual(DocumentSetLogType.Added,set.Logs.Last().Type);
        }

        [Test]
        public void UpdateDocument_AddAUpdateLog_IfDocumentIncludeInDocumentSet()
        {
            // arrange
            var set = new DocumentSet();
            var doc = Guid.NewGuid();
            set.Add(doc);
            
            // act
            set.Update(doc);
            
            // assert
            Assert.AreEqual(doc,set.Logs.Last().Document);
            Assert.AreEqual(DocumentSetLogType.Updated,set.Logs.Last().Type);
        }
        
        [Test]
        public void DeleteDocument_DeleteTheDocumentWithADeleteLog_IfDocumentIncludeInDocumentSet()
        {
            // arrange
            var set = new DocumentSet();
            var doc = Guid.NewGuid();
            set.Add(doc);
            
            // act
            set.Delete(doc);
            
            // assert
            Assert.IsTrue(set.Documents.All(x=>x != doc));
            Assert.AreEqual(doc,set.Logs.Last().Document);
            Assert.AreEqual(DocumentSetLogType.Deleted,set.Logs.Last().Type);
        }
        
        [Test]
        public void DeleteDocument_ThrowsInvalidOperationException_IfDocumentNotIncludeInDocumentSet()
        {
            // arrange
            var set = new DocumentSet();

            // act & assert
            Assert.Throws<InvalidOperationException>(() =>
                set.Delete(Guid.NewGuid())
            );
        }
    }
}