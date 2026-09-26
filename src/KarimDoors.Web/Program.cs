using KarimDoors.Application;
using KarimDoors.Infrastructure;
using KarimDoors.Infrastructure.Persistence;
using KarimDoors.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await InitialiseDatabaseAsync(app);
await app.RunAsync();

static async Task InitialiseDatabaseAsync(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        return;
    }

    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<KarimDoorsDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().SeedAsync();
}
