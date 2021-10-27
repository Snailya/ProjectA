using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectA.Models;
using ProjectA.Services.Exceptions;

namespace ProjectA.Services
{
    public class RepositoryService
    {
        private readonly DocumentContext _context;

        public RepositoryService(DocumentContext context)
        {
            _context = context;
        }

        public IEnumerable<Document> List()
        {
            return _context.Documents.AsEnumerable();
        }

        public Task<IEnumerable<Document>> ListAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Documents.AsEnumerable());
        }

        public Document AddDocument(int entityId, int snapshotFolderId = default)
        {
            // validate operation
            if (_context.Documents.Find(entityId) is { })
                throw new RepositoryException($"The document: {entityId} has already been tracked.");

            // persist
            var document = snapshotFolderId is default(int)
                ? new Document(entityId)
                : new Document(entityId, snapshotFolderId);

            _context.Documents.Add(document);
            _context.SaveChanges();

            return document;
        }

        public void DeleteDocument(int entityId)
        {
            // check if not exist
            if (_context.Documents.Find(entityId) is not { } document)
                throw new RepositoryException($"The document: {entityId} not found.");

            _context.Documents.Remove(document);
            _context.SaveChanges();
        }

        public void SetSnapshotFolder(int entityId, int snapshotFolderId)
        {
            // check if not exist
            if (_context.Documents.Find(entityId) is not { } document)
                throw new RepositoryException($"The document: {entityId} not found.");

            try
            {
                document.SetSnapshotFolder(snapshotFolderId);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                throw new RepositoryException(invalidOperationException.Message);
            }

            _context.SaveChanges();
        }
    }
}