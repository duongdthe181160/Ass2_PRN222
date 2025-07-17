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
builder.Services.AddScoped<NewsArticleService>();
builder.Services.AddScoped<SystemAccountService>();
builder.Services.AddScoped<TagService>();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied"; // Add a page if needed
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

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Response.Redirect("/Login");
            return;
        }

        context.Response.Redirect("/Index");
        return;
    }
    await next();
});

// Routing endpoints
app.MapRazorPages();
app.MapHub<NewsHub>("/newsHub");

app.Run();
