using Microsoft.EntityFrameworkCore;
using Persistence;

public class Program {

    private static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Initialize SQLitePCL provider
    SQLitePCL.Batteries.Init();

    // Add services to the container.
    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    // Add authorization services
    builder.Services.AddAuthorization();

    // Add controllers
    builder.Services.AddControllers();

    // Build the host and get the service provider
    var app = builder.Build();

    // Ensure database migration is applied
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<DataContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            // Handle exception as needed
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Run the application
    app.Run();
}

}