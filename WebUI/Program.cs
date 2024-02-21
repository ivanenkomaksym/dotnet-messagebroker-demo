using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Common.Extensions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.FeatureManagement;
using NToastNotify;
using WebUI;
using WebUI.Consumers;
using WebUI.Notifications;
using WebUI.Services;
using WebUI.Users;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureOpenTelemetry();

// Add services to the container.
builder.Services.AddRazorPages().AddNToastNotifyToastr(new ToastrOptions
{
    ProgressBar = true,
    TimeOut = 5000
}).AddRazorPagesOptions(ops =>
{
    ops.Conventions.AuthorizeFolder("/Admin", "RequireAdmins");
});

builder.Services.AddFeatureManagement();

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

var gatewayAddress = builder.Configuration["ApiSettings:GatewayAddress"];

builder.Services.AddTransient<ICustomerService>(provider =>
{
    var featureManager = provider.GetRequiredService<IFeatureManager>();

    if (featureManager.IsEnabledAsync(FeatureFlags.Customer).GetAwaiter().GetResult())
    {
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient(typeof(ICustomerService).FullName);
        httpClient.BaseAddress = new Uri(gatewayAddress);
        return new CustomerService(httpClient);
    }

    return new StubCustomerService();
});

builder.Services.AddTransient<IProductService>(provider =>
{
    var featureManager = provider.GetRequiredService<IFeatureManager>();

    if (featureManager.IsEnabledAsync(FeatureFlags.Product).GetAwaiter().GetResult())
    {
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient(typeof(IProductService).FullName);
        httpClient.BaseAddress = new Uri(gatewayAddress);
        return new ProductService(httpClient);
    }

    return new StubProductService();
});

builder.Services.AddHttpClient<IShoppingCartService, ShoppingCartService>(options =>
{
    options.BaseAddress = new Uri(gatewayAddress);
});

builder.Services.AddHttpClient<IOrderService, OrderService>(options =>
{
    options.BaseAddress = new Uri(gatewayAddress);
});

builder.Services.AddTransient<IDiscountService>(provider =>
{
    var featureManager = provider.GetRequiredService<IFeatureManager>();

    if (featureManager.IsEnabledAsync(FeatureFlags.Discount).GetAwaiter().GetResult())
    {
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient(typeof(IDiscountService).FullName);
        httpClient.BaseAddress = new Uri(gatewayAddress);
        return new DiscountService(httpClient);
    }

    return new EmptyDiscountService();
});

builder.Services.AddSingleton<IUserProvider, DefaultUserProvider>();
builder.Services.AddSingleton<IMassTransitConsumersRegistry, MassTransitConsumersRegistry>();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<IGraphQLClient>(s => new GraphQLHttpClient(gatewayAddress + "/gateway/graphql", new NewtonsoftJsonSerializer()));
builder.Services.AddSingleton<IFeedbackService>(services =>
{
    var featureManager = services.GetRequiredService<IFeatureManager>();

    if (featureManager.IsEnabledAsync(FeatureFlags.Feedback).GetAwaiter().GetResult())
    {
        var graphClient = services.GetRequiredService<IGraphQLClient>();
        return new FeedbackService(graphClient);
    }

    return new EmptyFeedbackService();
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
