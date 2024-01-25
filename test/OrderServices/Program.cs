using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderServices.Consumers;
using OrderServices.Data;
using OrderServices.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddMassTransit(x =>
{

    x.AddConsumersFromNamespaceContaining<ProductCreatedConsumer>();
    x.AddConsumersFromNamespaceContaining<ProductUpdatedConsumer>();
    x.AddConsumersFromNamespaceContaining<ProductDeletedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("order", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("order-service-created", e =>
        {
            e.ConfigureConsumer<ProductCreatedConsumer>(context);
        });
        cfg.ReceiveEndpoint("order-service-updated", e =>
        {
            e.ConfigureConsumer<ProductUpdatedConsumer>(context);
        });
        cfg.ReceiveEndpoint("order-service-deleted", e =>
        {
            e.ConfigureConsumer<ProductDeletedConsumer>(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
