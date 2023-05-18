using WebUI.Services;
using WebUI.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<ICustomerService, CustomerService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]);
});

builder.Services.AddHttpClient<ICatalogService, CatalogService>(options =>
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
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
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
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
