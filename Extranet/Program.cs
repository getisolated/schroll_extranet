using Extranet.Controllers;
using Extranet.Models;
using Extranet.Models.Settings;
using Extranet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(AuthController.securityscheme)
                .AddCookie(AuthController.securityscheme, options =>
                {
                    options.Cookie = new CookieBuilder
                    {
                        //Domain = "",
                        HttpOnly = true,
                        Name = ".EspaceClient.Security.Cookie",
                        Path = "/",
                        SameSite = SameSiteMode.Lax,
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.LoginPath = new PathString("/Connexion");
                    options.ReturnUrlParameter = "RequestPath";
                    options.SlidingExpiration = true;
                    options.LogoutPath = new PathString("/Connexion");
                });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
      .RequireAuthenticatedUser()
      .Build();
});

builder.Services.AddDistributedMemoryCache();

// Add services to the container.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

/*
#if DEBUG
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
#else
builder.Services.AddControllersWithViews();
#endif
*/

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Configure our options values
WebSettings settings = builder.Configuration.GetSection("WebSettings")?.Get<WebSettings>();
builder.Services.Configure<WebSettings>(builder.Configuration.GetSection("WebSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(settings.NavSettings.FilePath),
    RequestPath = new PathString(DataProvider._defaultFilePathName),
    EnableDirectoryBrowsing = false
});

//app.UseFileServer(new FileServerOptions
//{
//    FileProvider = new PhysicalFileProvider(settings.NavSettings.SharedFilePath),
//    RequestPath = new PathString(DataProvider._defaultSharedFilePathName),
//    EnableDirectoryBrowsing = false
//});

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Accueil}/{id?}");
app.MapControllerRoute("Connexion", "Connexion", new { controller = "Auth", action = "Login" });

app.Run();
