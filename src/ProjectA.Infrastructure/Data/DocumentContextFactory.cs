using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectA.Infrastructure.Data
{
    public class DocumentContextFactory : IDesignTimeDbContextFactory<DocumentContext>
    {
        private readonly IMediator _mediator;

        public DocumentContextFactory(IMediator mediator)
        {
            _mediator = mediator;
        }

        public DocumentContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DocumentContext>();
            optionsBuilder.UseSqlite("Data Source=document.db");

            return new DocumentContext(optionsBuilder.Options, _mediator);
        }
    }
}