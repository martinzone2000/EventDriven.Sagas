using CustomerService.Configuration;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Integration.Handlers;
using CustomerService.Repositories;
using EventDriven.DependencyInjection.URF.Mongo;
using EventDriven.Sagas.DependencyInjection;
using Integration.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add automapper
builder.Services.AddAutoMapper(typeof(Program));

// Add database settings
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddMongoDbSettings<CustomerDatabaseSettings, Customer>(builder.Configuration);

// Add command handlers
builder.Services.AddCommandHandlers();

// Add Dapr Event Bus and event handler
builder.Services.AddDaprEventBus(builder.Configuration, true);
builder.Services.AddDaprMongoEventCache(builder.Configuration);
builder.Services.AddSingleton<CustomerCreditReserveRequestedEventHandler>();
builder.Services.AddSingleton<CustomerCreditReserveReleaseEventHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

// Map Dapr Event Bus subscribers
app.UseCloudEvents();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapSubscribeHandler();
    endpoints.MapDaprEventBus(eventBus =>
    {
        var customerCreditRequestedEventHandler = app.Services.GetRequiredService<CustomerCreditReserveRequestedEventHandler>();
        var customerCreditReleasedEventHandler = app.Services.GetRequiredService<CustomerCreditReserveReleaseEventHandler>();
        eventBus.Subscribe(customerCreditRequestedEventHandler, nameof(CustomerCreditReserveRequested), "v1");
        eventBus.Subscribe(customerCreditReleasedEventHandler, nameof(CustomerCreditReleaseRequested), "v1");
    });
});

app.Run();