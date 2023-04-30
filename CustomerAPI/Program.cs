using Common.Configuration;
using Common.Persistence;
using CustomerAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
builder.Configuration.AddJsonFile("appsettings.k8s.json", optional: true, reloadOnChange: false);

// Add services to the container.
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IRabbitMQChannelRegistry>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var rabbitMQOptions = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();

    if (rabbitMQOptions.UseStub)
        return new StubRabbitMQChannelRegistry();
    else
        return new RabbitMQChannelRegistry();
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();