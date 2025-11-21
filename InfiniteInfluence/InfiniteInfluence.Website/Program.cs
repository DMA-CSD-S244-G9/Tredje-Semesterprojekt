using InfiniteInfluence.ApiClient;
using InfiniteInfluence.DataAccessLibrary.Dao.Interfaces;

namespace InfiniteInfluence.Website
{
    public class Program
    {
        // The URL address for the REST API
        // private static readonly string _apiUrl = "https://localhost:7777";
        private static readonly string _apiUrl = "https://localhost:32775";


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container. 
            builder.Services.AddControllersWithViews();

            // Sets up the dependency injection using the IInfluencerDao in the API
            builder.Services.AddScoped<IInfluencerDao>((_) => new InfluencerApiClient(_apiUrl));
          
            // Sets up the dependency injection using the IcompanyDao in the API
            builder.Services.AddScoped<ICompanyDao>((_) => new CompanyApiClient(_apiUrl));

            // Sets up the dependency injection using the AnnouncementApiClient in the API
            builder.Services.AddScoped<IAnnouncementDao>((_) => new AnnouncementApiClient(_apiUrl));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();


            // Sends 404 and other status codes to the Error/404 page
            app.UseStatusCodePagesWithReExecute("/Error/404");


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
