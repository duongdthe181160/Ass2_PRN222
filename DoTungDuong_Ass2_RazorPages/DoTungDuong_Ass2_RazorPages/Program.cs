using DoTungDuong_Ass2_RazorPages.Hubs;
using DoTungDuongBLL.Services;
using DoTungDuongDAL.Models;
using DoTungDuongDAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Razor Pages and Database
builder.Services.AddRazorPages();
builder.Services.AddDbContext<FunewsManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection - Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();

// Dependency Injection - Services
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<SystemAccountService>();
builder.Services.AddScoped<TagService>();

// Special handling for NewsArticleService with SignalR integration
builder.Services.AddScoped<NewsArticleService>(provider =>
{
    var newsRepo = provider.GetRequiredService<INewsArticleRepository>();
    var tagRepo = provider.GetRequiredService<IRepository<Tag>>();
    var categoryRepo = provider.GetRequiredService<IRepository<Category>>();
    var hubContext = provider.GetRequiredService<IHubContext<NewsHub>>();
    
    var service = new NewsArticleService(newsRepo, tagRepo, categoryRepo);
    service.SetSignalRNotifier(async (message) => 
        await hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", message));
    return service;
});

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Redirect logic for authenticated users
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        if (!context.User.Identity?.IsAuthenticated ?? false)
        {
            context.Response.Redirect("/Login");
            return;
        }

        var role = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        context.Response.Redirect(role switch
        {
            "Admin" => "/AccountPage/Index",
            "Staff" => "/NewsArticlePage/Index", 
            "Lecturer" => "/ViewNewsPage/Index",
            _ => "/Login"
        });
        return;
    }
    await next();
});

// Routing endpoints
app.MapRazorPages();
app.MapHub<NewsHub>("/newsHub");

app.Run();
