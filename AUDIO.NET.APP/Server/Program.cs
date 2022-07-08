using AUDIO.NET.APP.Shared;
using AUDIO.NET.APP.Shared.Interfaces;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Sinks.File;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>loggingBuilder.AddSerilog(Log.Logger));

var spotifyListener = new ListenerBuilder()
    .UseRedirectUrl("https://localhost:5001/auth/Token")
    .WithClientId("18037b72c0344bffa9a093fdb0c9c3de")
    .AndClientSecret("55bf9ad995f64409bb4f5649d773d1bf")

    .Build();

builder.Services.AddSingleton<ISpotify>(spotifyListener);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

Process process = new Process();
process.StartInfo.UseShellExecute = false;
process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
process.StartInfo.FileName = "app-win.exe";
process.Start();
app.Lifetime.ApplicationStopping.Register(
    () => process.Kill()
);
app.Run();

