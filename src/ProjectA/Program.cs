﻿using System;
using System.Linq;
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
            if (!dbContext.Documents.Any()) SeedData.PopulateTestData(dbContext);
            dbContext.Dispose();
            
            Console.WriteLine("Hello World!");
        }
    }
}