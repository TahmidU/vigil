using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Vigil.Data.Extensions;
using Vigil.Data.Persistence;
using Vigil.Data.Seed;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ── Data + Identity + Core Services ──────────────────────────────────────────
// This single call is the only place Vigil.Api touches Vigil.Data types.
// Everything else in Vigil.Api goes through Vigil.Core interfaces.
builder.Services.AddVigilData(builder.Configuration);

// ── Cookie ────────────────────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    opt.Cookie.SameSite = SameSiteMode.Strict;
    opt.ExpireTimeSpan = TimeSpan.FromHours(8);
    opt.SlidingExpiration = true;

    opt.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    opt.Events.OnRedirectToAccessDenied = ctx =>
    {
        ctx.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opt => opt.AddPolicy("vigil", policy =>
    policy
        .WithOrigins(builder.Configuration["Cors:Origin"] ?? "http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ── Migrations + Seed ─────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedService.SeedAsync(scope.ServiceProvider, builder.Configuration);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("vigil");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();

