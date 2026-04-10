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

    // 🔥 FIX PORT = -1
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
    // ✅ Chạy local
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// 👉 Debug log để xem trên Render
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

// 🔥 Auto migrate (để thấy lỗi thật nếu có)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    Console.WriteLine("✅ Database migrated");
}

app.Run();