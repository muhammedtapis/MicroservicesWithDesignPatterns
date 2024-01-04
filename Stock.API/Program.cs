using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using Stock.API.Consumer;
using Stock.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//masstransit
builder.Services.AddMassTransit(x =>
{
    //hangi mesagebroker kullan�caz onu belirt.
    x.AddConsumer<OrderCreatedRequestEventConsumer>();
    x.AddConsumer<StockRollBackRequestMessageConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ")); //hostunu tan�mla
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockOrderCreatedEventQueueName, e =>
        {//e �zerinden bu kuyrugu hangi consumer dinleyecek onu belirtiyoruz.
            e.ConfigureConsumer<OrderCreatedRequestEventConsumer>(context); //kuyrukla tetiklencek consumer
        });

        cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockRollBackRequestMessageQueueName, e =>
        {//e �zerinden bu kuyrugu hangi consumer dinleyecek onu belirtiyoruz.
            e.ConfigureConsumer<StockRollBackRequestMessageConsumer>(context); //kuyrukla tetiklencek consumer
        });
    });
});

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseInMemoryDatabase("StockDb"); //InMemory db kullan
//});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
    //options.EnableSensitiveDataLogging(false);
    //options.UseLoggerFactory(null);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//seed data scope
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    context.Stocks.Add(new Stock.API.Models.Stock() { Id = 1, ProductId = 1, Count = 100 });
//    context.Stocks.Add(new Stock.API.Models.Stock() { Id = 2, ProductId = 2, Count = 100 });
//    var list = context.Stocks.ToList();
//}

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