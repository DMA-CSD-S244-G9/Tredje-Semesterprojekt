
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;
using InfiniteInfluence.DataAccessLibrary.Dao.SqlServer;

namespace InfiniteInfluence.API;

public class Program
{
    // The database connection string used for dependency injection of any of the DAO classes (InfluencerDao, CompanyDao etc.)
    //private const string _dataBaseConnectionString = "Data Source=host.docker.internal;Initial Catalog=InfiniteInfluence;Persist Security Info=True;User ID=sa;Password=@12tf56so;Encrypt=True;Trust Server Certificate=True";
    private const string _dataBaseConnectionString = "Data Source=localhost;Initial Catalog=InfiniteInfluence;User ID=sa;Password=@12tf56so;Trust Server Certificate=True";



    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        // TODO: Write a bit more about this later on...
        // Register the AuthorDao for MSSqlServer for dependency injection
        builder.Services.AddScoped<IInfluencerDao>((_) => new InfluencerDao(_dataBaseConnectionString));

        // Register the CompanyDao for MSSqlServer for dependency injection
        builder.Services.AddScoped<ICompanyDao>((_) => new CompanyDao(_dataBaseConnectionString));



        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
