using Microsoft.EntityFrameworkCore;
using ConnectDB.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Lấy DATABASE_URL từ Render
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

string connectionString;

// ✅ Nếu có DATABASE_URL (deploy trên Render)
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    connectionString = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = uri.AbsolutePath.Trim('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    }.ToString();
}
else
{
    // ✅ Chạy local
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// 🔗 Kết nối DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 🔧 Services
builder.Services.AddControllers();
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

app.UseHttpsRedirection();
app.MapControllers();

// 🔥 Auto migrate (không crash app)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("✅ Database migrated successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Migration failed: " + ex.Message);
    }
}

app.Run();