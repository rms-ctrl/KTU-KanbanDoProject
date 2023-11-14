using LATESTReactKanBanDo.Data.Interfaces;
using LATESTReactKanBanDo.Data.Repositories;
using LATESTReactKanBanDo.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDbContext<KanbanDoDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING"),
        providerOptions => providerOptions.EnableRetryOnFailure()));

builder.Services.AddTransient<IViewsRepository, ViewsRepository>();
builder.Services.AddTransient<IColumnRepository, ColumnRepository>();
builder.Services.AddTransient<ITaskItemRepository, TaskItemRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
