using System.Data;
using LostPeople.Application;
using LostPeople.Infrastructure;
using LostPeople.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

var logConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/lostpeople-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14);

var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL");
if (!string.IsNullOrEmpty(seqUrl))
    logConfig = logConfig.WriteTo.Seq(seqUrl);

Log.Logger = logConfig.CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Home/Error";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Public", opt =>
    {
        opt.PermitLimit = 30;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("Report", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromHours(1);
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = 429;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Api", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LostPeopleDbContext>();
    await db.Database.MigrateAsync();

    var connStr = db.Database.GetConnectionString();
    if (!string.IsNullOrEmpty(connStr) && !await QuartzSchemaExistsAsync(connStr))
    {
        await InitializeQuartzSchemaAsync(connStr);
    }

    await DbInitializer.SeedAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' https://cdn.tailwindcss.com 'unsafe-inline'; " +
        "style-src 'self' https://cdn.tailwindcss.com 'unsafe-inline'; " +
        "img-src 'self' data:; " +
        "font-src 'self' https://fonts.googleapis.com https://fonts.gstatic.com; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none';");
    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health").AllowAnonymous();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task<bool> QuartzSchemaExistsAsync(string connStr)
{
    using var conn = new SqlConnection(connStr);
    await conn.OpenAsync();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = 'QRTZ_JOB_DETAILS'";
    var count = (int?)await cmd.ExecuteScalarAsync() ?? 0;
    return count > 0;
}

static async Task InitializeQuartzSchemaAsync(string connStr)
{
    var sql = await File.ReadAllTextAsync("quartz-schema.sql");
    var batches = sql.Split("GO", StringSplitOptions.RemoveEmptyEntries);
    using var conn = new SqlConnection(connStr);
    await conn.OpenAsync();
    foreach (var batch in batches)
    {
        var trimmed = batch.Trim();
        if (trimmed.Length == 0) continue;
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = trimmed;
            cmd.CommandTimeout = 30;
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error ejecutando batch Quartz schema");
        }
    }
    Log.Information("Quartz schema initialized successfully");
}
