using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using WebHooks.Components;
using WebHooks.Middleware;
using WebHooks.Workers;
using Webhooks_System_Library;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBackendDependencies(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("WebhooksDeliveryDatabase")));

builder.Services.AddControllers();

builder.Services.AddHostedService<DeliveryWorker>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ApiKeyMiddleware>();
app.MapControllers();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
