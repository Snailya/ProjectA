using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectA.Infrastructure.Data
{
    public class DocumentContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private readonly IMediator _mediator;

        public DocumentContextFactory(IMediator mediator)
        {
            _mediator = mediator;
        }

        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=document.db");

            return new AppDbContext(optionsBuilder.Options, _mediator);
        }
    }
}