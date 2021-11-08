using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectA.Core.Models;
using ProjectA.Core.Services.Exceptions;
using ProjectA.Infrastructure.Data;

namespace ProjectA.Infrastructure.Services
{
    public class RepositoryService
    {
        private readonly IDbContextFactory<DocumentContext> _dbContextFactory;

        public RepositoryService(IDbContextFactory<DocumentContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IEnumerable<Document> List()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return context.Documents.AsEnumerable();
        }

        public Task<IEnumerable<Document>> ListAsync(CancellationToken cancellationToken)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return Task.FromResult(context.Documents.AsEnumerable());
        }

        public Document AddDocument(int entityId, int snapshotFolderId = default)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // validate operation
            if (context.Documents.SingleOrDefault(x => x.EntityId == entityId) is not null)
                throw new RepositoryException($"The document: {entityId} has already been tracked.");

            // persist
            var document = snapshotFolderId is default(int)
                ? new Document(entityId)
                : new Document(entityId, snapshotFolderId);

            context.Documents.Add(document);
            context.SaveChanges();

            return document;
        }

        public void DeleteDocument(int entityId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // check if not exist
            if (context.Documents.SingleOrDefault(x => x.EntityId == entityId) is not { } document)
                throw new RepositoryException($"The document: {entityId} not found.");

            context.Documents.Remove(document);
            context.SaveChanges();
        }

        public Document UpdateSnapshot(int entityId, int snapshotId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // check if not exist
            if (context.Documents.SingleOrDefault(x => x.EntityId == entityId) is not { } document)
                throw new RepositoryException($"The document: {entityId} not found.");

            if (context.Documents.SingleOrDefault(x => x.EntityId == snapshotId) is not { } snapshot)
                snapshot = new Document(snapshotId);
            document.UpdateSnapshot(snapshot);
            context.SaveChanges();

            return document;
        }

        public void SetSnapshotFolder(int entityId, int snapshotFolderId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // check if not exist
            if (context.Documents.SingleOrDefault(x => x.EntityId == entityId) is not { } document)
                throw new RepositoryException($"The document: {entityId} not found.");

            try
            {
                document.SetSnapshotFolder(snapshotFolderId);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                throw new RepositoryException(invalidOperationException.Message);
            }

            context.SaveChanges();
        }
    }
}