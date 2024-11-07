using gerenciadorConsultasPICS.Configurations;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .Build();

var culture = "pt-BR";
var cultureInfo = new CultureInfo(culture);
CultureInfo.CurrentCulture = cultureInfo;
CultureInfo.CurrentUICulture = cultureInfo;

#region cors
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
string[] allowedHostsCors = configuration.GetSection("AllowedHostsCors").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin() // Restringir???
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
#endregion

builder.Services.AddMemoryCache()
                .AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(120);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Admin/Login/Login";
                    options.AccessDeniedPath = "/Home/AcessoNegado";
                });

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApenasAdmin", policy => policy.RequireClaim("idPerfil", "1"));
    options.AddPolicy("ApenasInstituicao", policy => policy.RequireClaim("idPerfil", "2"));
});

builder.Services.AddMvc()
                .AddSessionStateTempDataProvider();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

builder.Services.ConfigureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSession();

app.UseCors(MyAllowSpecificOrigins);

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
