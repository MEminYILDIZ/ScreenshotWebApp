using Hangfire;
using Hangfire.MemoryStorage;
using ScreenshotWebApp.Services;
using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Chromium indir sadece gerekliyse
var browserFetcher = new BrowserFetcher();
var revisionInfo = await browserFetcher.GetRevisionInfoAsync();
if (!revisionInfo.Local)
{
    Console.WriteLine("Chromium indiriliyor...");
    await browserFetcher.DownloadAsync();
    Console.WriteLine("İndirme tamamlandı.");
}

builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

builder.Services.AddSingleton<ScreenshotService>();
builder.Services.AddHostedService<HangfireJobService>();

var app = builder.Build();

app.UseRouting();

app.UseHangfireDashboard(); 

app.MapControllers();
app.MapGet("/", () => "Uygulama çalışıyor!");

app.Run();
