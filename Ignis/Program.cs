

using Serilog.Sinks.PostgreSQL;
using Serilog;
using Ignis.Data;
using Microsoft.EntityFrameworkCore;
using Ignis.Models.Util;
using System;
using Ignis.Models;
using Microsoft.AspNetCore.Identity;

var configuration = new ConfigurationBuilder()    
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .Build();

var _configuration = configuration;

// get connection string
var _connectionString = configuration.GetConnectionString("IgnisDb");

// init logger
Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
    .MinimumLevel.Warning()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.PostgreSQL(
    connectionString: _connectionString,
    tableName: "AppLogs",
    columnOptions: new Dictionary<string, ColumnWriterBase>
        {
                    { "Message", new RenderedMessageColumnWriter() },
                    { "Level", new LevelColumnWriter(true, NpgsqlTypes.NpgsqlDbType.Text) }, // Forces level to plain text
                    { "Timestamp", new TimestampColumnWriter() },
                    { "Exception", new ExceptionColumnWriter() },
                    { "Properties", new PropertiesColumnWriter() }
        },
    needAutoCreateTable: false
    ).CreateLogger();


var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");;

//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AppDbContext>();



builder.Host.ConfigureServices((ctx, service) =>
{
    IConfiguration config = ctx.Configuration;
    service.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(ctx.Configuration.GetConnectionString("IgnisDb"));
        options.EnableSensitiveDataLogging(false);
    });
    
});

builder.Services.AddIdentityCore<AppUser>(o =>
{
    o.SignIn.RequireConfirmedAccount = true;

}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddTransient<ITagDefs,TagDefs>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // authenticate users
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
