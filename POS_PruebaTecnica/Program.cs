
using Microsoft.AspNetCore.Localization;
using OfficeOpenXml;
using POS_PruebaTecnica.Models.Reports;
using POS_PruebaTecnica.Models.ServiciosRest;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
ExcelPackage.License.SetNonCommercialPersonal("Testing");
var defaultCulture = new CultureInfo("es-SV");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new[] { defaultCulture },
    SupportedUICultures = new[] { defaultCulture }
};
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});
// Add services to the container.
builder.Services.AddControllersWithViews(); 
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<Login>();
builder.Services.AddScoped<Producto_Service>();
builder.Services.AddScoped<POS_Service>();
builder.Services.AddScoped<Reportes_Service>();
builder.Services.AddScoped<GenerarExcel>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
var app = builder.Build();

app.UseRequestLocalization(localizationOptions);
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

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
