using Microsoft.AspNetCore.Identity;
using FarmersMarket.Model;

using System.Linq.Expressions;
using FarmersMarket.Model;
using FarmersMarket.Model;
using AspNetCore.ReCaptcha;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using FarmersMarket.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<GoogleCaptchaConfig>(builder.Configuration.GetSection("GoogleRecaptcha"));
builder.Services.AddTransient(typeof(GoogleCaptchaService));
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
	// Default Lockout settings.
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60);
	options.Lockout.MaxFailedAccessAttempts = 3;
	options.Lockout.AllowedForNewUsers = true;
});
// This is for credit card protection
builder.Services.AddDataProtection();
// Authorize
builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.LoginPath = "/Login";
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(100);
	options.Cookie.IsEssential = true;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseFileServer();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();


