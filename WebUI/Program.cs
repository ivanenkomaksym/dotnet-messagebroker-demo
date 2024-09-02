using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Common.Configuration;
using Common.Extensions;
using Common.Routing;
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

builder.AddServiceDefaults();
builder.Host.ConfigureOpenTelemetry();

// Add services to the container.
builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(ApplicationOptions.Name));
builder.Services.AddSingleton<IEnvironmentRouter, EnvironmentRouter>();

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

builder.Services.AddMassTransit((Action<IBusRegistrationConfigurator>)(x =>
{
    x.AddConsumer<UserReserveStockResultConsumer>();
    x.AddConsumer<UserPaymentResultConsumer>();
    x.AddConsumer<UserShipmentResultConsumer>();

    x.UsingRabbitMq(AspireExtensions.ConfigureRabbitMq);
}));

var gatewayAddress = builder.Configuration.GetGatewayAddress();

builder.Services.AddIfFeatureEnabledHttpClientBased<ICustomerService, CustomerService, StubCustomerService>(FeatureFlags.Customer, gatewayAddress);

builder.Services.AddIfFeatureEnabledHttpClientBased<IProductService, ProductService, StubProductService>(FeatureFlags.Product, gatewayAddress);

builder.Services.AddIfFeatureEnabledHttpClientBased<IShoppingCartService, ShoppingCartService, StubShoppingCartService>(FeatureFlags.ShoppingCart, gatewayAddress);

builder.Services.AddIfFeatureEnabledHttpClientBased<IDiscountService, DiscountService, EmptyDiscountService>(FeatureFlags.Discount, gatewayAddress);

builder.Services.AddIfFeatureEnabledServiceBased<IFeedbackService, FeedbackService, EmptyFeedbackService, IGraphQLClient>(FeatureFlags.Feedback);

builder.Services.AddHttpClient<IOrderService, OrderService>(options =>
{
    options.BaseAddress = new Uri(gatewayAddress);
});

builder.Services.AddSingleton<IUserProvider, DefaultUserProvider>();
builder.Services.AddSingleton<IMassTransitConsumersRegistry, MassTransitConsumersRegistry>();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<IGraphQLClient>(s => new GraphQLHttpClient(gatewayAddress + "/gateway/graphql", new NewtonsoftJsonSerializer()));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.EventsType = typeof(CustomCookieAuthenticationEvents);
    });

builder.Services.AddScoped<CustomCookieAuthenticationEvents>();

var app = builder.Build();

app.MapDefaultEndpoints();

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