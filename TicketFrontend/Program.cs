using TicketFrontend.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// MVC
// ============================================================

builder.Services.AddControllersWithViews();

// ============================================================
// Backend API URL
// ============================================================

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException(
        "Backend API BaseUrl is not configured.");
}

// ============================================================
// HTTP Client
// ============================================================

builder.Services.AddHttpClient<TicketApiService>(client =>
{
    client.BaseAddress = new Uri(
        apiBaseUrl.EndsWith("/")
            ? apiBaseUrl
            : apiBaseUrl + "/");
});

// ============================================================
// Session
// ============================================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout =
        TimeSpan.FromHours(8);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ============================================================
// HTTP Pipeline
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// ============================================================
// Session
// ============================================================

app.UseSession();

app.UseAuthorization();

// ============================================================
// Default Route
// ============================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
