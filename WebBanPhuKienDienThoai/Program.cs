using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Controllers;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Repositories;
using WebBanPhuKienDienThoai.Respository;
using WebBanPhuKienDienThoai.Services;

var builder = WebApplication.CreateBuilder(args);

//Thêm đoạn này để sử dụng localization
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

//Thêm đoạn này để sử dụng localization 
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});
// Thêm đoạn này để sử dụng localization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("vi-VN")
    };
    options.DefaultRequestCulture = new RequestCulture("vi-VN");
    options.SupportedUICultures = supportedCultures;
});



builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<PayPalService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
        googleOptions.CallbackPath = "/signin-google";
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IDeviceTypeRepository, EFDeviceTypeRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IDiscountRepository, EFDiscountRepository>();

builder.Services.AddScoped<IRatingRepository, EFRatingRepository>();
builder.Services.AddScoped<ICommentRepository, EFCommentRepository>();

// Đăng ký UserManager
builder.Services.AddScoped<UserManager<ApplicationUser>>();

builder.Services.AddRazorPages();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IGeminiService, GeminiService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    await DataInitializer.SeedData(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization();//Thêm dòng này để sử dụng localization

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();