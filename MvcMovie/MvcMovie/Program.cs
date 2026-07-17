using Microsoft.EntityFrameworkCore;
//using MvcMovie.Models; //This is for the SeedData.cs
using MvcMovie.Services; //This is for the New Movie Seeder

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MvcMovieContext") ?? throw new InvalidOperationException("Connection string 'MvcMovieContext' not found.");

builder.Services.AddDbContext<MvcMovieContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IMovieSeedService, MovieSeedService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    //SeedData Initializer
    //var services = scope.ServiceProvider;
    //SeedData.Initialize(services); 

    var seedService = scope.ServiceProvider.GetRequiredService<IMovieSeedService>();
    await seedService.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
