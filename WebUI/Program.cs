using WebUI.Services;
using WebUI.Users;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using NToastNotify;
using WebUI.Notifications;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebUI.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddNToastNotifyToastr(new ToastrOptions
{
    ProgressBar = true,
    TimeOut = 5000
}).AddRazorPagesOptions(ops =>
{
    ops.Conventions.AuthorizeFolder("/Admin", "RequireAdmins");
});

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("RequireAdmins", policy => policy.RequireRole("Admin"));
});

builder.Services.AddSignalR();

// Add ToastNotification
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

builder.Services.AddToastify(config => { config.DurationInSeconds = 1000; config.Position = Position.Right; config.Gravity = Gravity.Bottom; });

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserReserveStockResultConsumer>();
    x.AddConsumer<UserPaymentResultConsumer>();
    x.AddConsumer<UserShipmentResultConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHttpClient<ICustomerService, CustomerService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]);
});

builder.Services.AddHttpClient<IProductService, ProductService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]);
});

builder.Services.AddHttpClient<IShoppingCartService, ShoppingCartService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]);
});

builder.Services.AddHttpClient<IOrderService, OrderService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]);
});

builder.Services.AddSingleton<IUserProvider, DefaultUserProvider>();
builder.Services.AddSingleton<IMassTransitConsumersRegistry, MassTransitConsumersRegistry>();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.EventsType = typeof(CustomCookieAuthenticationEvents);
    });

builder.Services.AddScoped<CustomCookieAuthenticationEvents>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseNToastNotify();
app.UseNotyf();

app.MapRazorPages();
app.MapHub<NotificationHub>("/notificationHub");

app.MapDefaultControllerRoute();

app.Run();
