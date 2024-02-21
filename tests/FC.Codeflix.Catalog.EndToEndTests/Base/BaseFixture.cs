using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

public class BaseFixture
{
    protected Faker Faker { get; set; }
    public ApiClient ApiClient { get; set; }
    public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
    public HttpClient HttpClient { get; set; }
    private readonly string _dbConnectionString;

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);
        var configuration = WebAppFactory.Services.GetService(typeof(IConfiguration));
        ArgumentNullException.ThrowIfNull(configuration);
        _dbConnectionString = ((IConfiguration)configuration).GetConnectionString("CatalogDb")!;
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseMySql(
                _dbConnectionString,
                ServerVersion.AutoDetect(_dbConnectionString))
            .Options
            );
        return dbContext;
    }

    public void ClearPersistence()
    {
        var dbContext = CreateDbContext();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

}
