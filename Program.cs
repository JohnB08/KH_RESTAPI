using Microsoft.EntityFrameworkCore;
using MovieAPI.DatabaseContext;
using MovieAPI.Util;

namespace MovieAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>();

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


            // Liten hack for å gjøre prototyping med sqlite mye lettere, slett, remake og seed on startup.
            // Gjør at man kan endre og kose seg med schemas + tables + annet uten å måtte stresse med migration og packages. 
            // Denne fanger ikke schema endringer, men ser bare om context schema matcher databaseschema.
            // Hvis man gjør endringer, må man kjøre migreringer for å få endringene i databasen. 
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
                SeedMovies.SeedData(context);
                SeedTvShows.SeedData(context);
            }

            app.Run();
        }
    }
}
