using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Context.Models
{
    /// <summary>
    /// This Factory class is setup so that an instance of CvWebContext will be created at design time.
    /// This class was implemented to resolve migration issues that arose after adding seed data.
    /// Without this factory, EF Core tools (such as 'dotnet ef migrations add') could not correctly
    /// construct the DbContext because the configuration (such as connection strings) was typically
    /// provided through dependency injection at runtime. Since migrations happen at design time,
    /// EF Core needed a way to instantiate CvWebContext without relying on the application's 
    /// dependency injection system. 
    /// Overall this class can be used for applying EF Core migrations and working with database scaffolding
    /// when the application's dependency injection (DI) container is unavailable.
    /// </summary>
    public class CvWebContextFactory : IDesignTimeDbContextFactory<CvWebContext>
    {
        /// <summary>
        /// The line below creates a new instance of CvWebContext that will be available at design time.
        /// This method is automatically called by EF Core tools (e.g., 'dotnet ef migrations add')
        /// to ensure that the DbContext is properly configured for migrations.
        /// </summary>
        /// <param name="args">Command-line arguments (not used here).</param>
        /// <returns>A new instance of CvWebContext configured with the connection string.</returns>
        public CvWebContext CreateDbContext(string[] args)
        {
            // Load configuration settings from the appsettings.json file located in the CvWebApi project.
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "CvWebApi"))// Navigate up to locate the API project
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)// Load database connection settings
                .Build();

            // Create an options builder to configure the DbContext with the SQL Server connection string.
            var optionsBuilder = new DbContextOptionsBuilder<CvWebContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));// Retrieve the connection string

            // Return a new instance of CvWebContext with the configured options.
            return new CvWebContext(optionsBuilder.Options);
        }
    }
}
