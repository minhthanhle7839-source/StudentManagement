using Microsoft.EntityFrameworkCore;
using ConnectDB.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables();
    
// 🔥 Lấy DATABASE_URL từ Render
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

string connectionString;

// ✅ Ưu tiên dùng DATABASE_URL (production)
if (!string.IsNullOrEmpty(databaseUrl))
{
    // Fix postgres:// → postgresql://
    if (databaseUrl.StartsWith("postgres://"))
    {
        databaseUrl = databaseUrl.Replace("postgres://", "postgresql://");
    }

    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    // 🔥 FIX PORT
    var port = uri.Port > 0 ? uri.Port : 5432;

    connectionString =
        $"Host={uri.Host};" +
        $"Port={port};" +
        $"Database={uri.AbsolutePath.Trim('/')};" +
        $"Username={userInfo[0]};" +
        $"Password={userInfo[1]};" +
        $"SSL Mode=Require;" +
        $"Trust Server Certificate=true";
}
else
{
    // ✅ Local
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// 👉 Debug log
Console.WriteLine("DATABASE_URL = " + databaseUrl);
Console.WriteLine("CONNECTION STRING = " + connectionString);

// 🔗 Kết nối DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 🔧 FIX JSON vòng lặp (QUAN TRỌNG)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 📘 Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger";
});
app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHttpsRedirection();
app.MapControllers();

// 🔥 Auto migrate (có log lỗi rõ ràng)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("✅ Database migrated");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Migration failed: " + ex.Message);
        throw;
    }
}

app.Run();