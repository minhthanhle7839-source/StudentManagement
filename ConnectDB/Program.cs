using Microsoft.EntityFrameworkCore;
using ConnectDB.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Lấy DATABASE_URL từ Render
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

string connectionString;

// ✅ Ưu tiên dùng DATABASE_URL (production)
if (!string.IsNullOrEmpty(databaseUrl))
{
    // Fix schema postgres:// → postgresql://
    if (databaseUrl.StartsWith("postgres://"))
    {
        databaseUrl = databaseUrl.Replace("postgres://", "postgresql://");
    }

    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    connectionString =
        $"Host={uri.Host};" +
        $"Port={uri.Port};" +
        $"Database={uri.AbsolutePath.Trim('/')};" +
        $"Username={userInfo[0]};" +
        $"Password={userInfo[1]};" +
        $"SSL Mode=Require;" +
        $"Trust Server Certificate=true";
}
else
{
    // ✅ Fallback local / config
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// 👉 Debug (xem log Render)
Console.WriteLine("DATABASE_URL = " + databaseUrl);
Console.WriteLine("CONNECTION STRING = " + connectionString);

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

// 🔥 Auto migrate (KHÔNG nuốt lỗi nữa)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    Console.WriteLine("✅ Database migrated");
}

app.Run();