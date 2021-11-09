using System;
using NUnit.Framework;
using ProjectA.Core.Models.DocAggregate;

namespace ProjectA.Test.UnitTests
{
    [TestFixture]
    public class DocumentTest
    {
        [Test]
        public void SetLinkedDocFolderId_ThrowsInvalidOperationException_IfLinkedDocHasSet()
        {
            // arrange
            var linkedDoc = new Document(new Random().Next());
            var document = new Document(new Random().Next());
            document.MakeLink(linkedDoc);

            // act & assert
            Assert.Throws<InvalidOperationException>(() =>
                document.SetLinkedDocFolderId(new Random().Next())
            );
        }
    }
}