using System.Text.Json;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebSite.Database;
using WebSite.Repositories.LineLogin;
using WebSite.Repositories.LineNotify;
using WebSite.Repositories.LineNotifySubscriber;
using WebSite.Setting;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

var connectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";
var keepAliveConnection = new SqliteConnection(connectionString);
keepAliveConnection.Open();
builder.Services.AddDbContext<MemberContext>(options =>
    options.UseSqlite(keepAliveConnection));

builder.Services.AddScoped<ILineNotifySubscriberRepository, LineNotifySubscriberRepository>();
builder.Services.AddScoped<ILineNotifyApi, LineNotifyApi>();
builder.Services.AddScoped<ILineLoginApi, LineLoginApi>();
builder.Services.AddSingleton(builder.Configuration.GetSection("LineNotifySetting").Get<LineNotifySetting>());
builder.Services.AddSingleton(builder.Configuration.GetSection("LineLoginSetting").Get<LineLoginSetting>());

var app = builder.Build();

//確保Database不為Null
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<MemberContext>();
    context.Database.EnsureCreated();
}

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();