using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddDbContext<DocumentContext>(options => options.UseSqlite("Data Source=document.db;"))
                .BuildServiceProvider();

            // populate test data
            using var dbContext = new DocumentContext(
                serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            SeedData.PopulateTestData(dbContext);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}