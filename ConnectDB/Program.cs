using Microsoft.EntityFrameworkCore;
using ConnectDB.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ================= CONTROLLERS =================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ================= CORS (QUAN TRỌNG) =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ================= SWAGGER =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ================= PIPELINE =================

// 🔥 Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger";
});

// 🔥 Static file (phải đặt sớm)
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

// 🔥 CORS đúng chuẩn
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();