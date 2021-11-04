using System;
using NUnit.Framework;
using ProjectA.Core.Models;

namespace ProjectA.Test
{
    [TestFixture]
    public class DocumentTest
    {
        [Test]
        public void SetSnapshotFolderId_ThrowsInvalidOperationException_IfSnapshotHasSet()
        {
            // arrange
            var snapshot = new Document(new Random().Next());
            var document = new Document(new Random().Next());
            document.UpdateSnapshot(snapshot);

            // act & assert
            Assert.Throws<InvalidOperationException>(() =>
                document.SetSnapshotFolder(new Random().Next())
            );
        }
    }
}