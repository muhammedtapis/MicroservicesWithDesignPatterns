using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Model;
using SharedLibrary;
using SharedLibrary.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentSucceedEventConsumer>(); //paymentten gelen eventi dinleyecek consumer !!
    x.AddConsumer<PaymentFailedEventConsumer>();
    x.AddConsumer<StockNotReservedEventConsumer>();
    //hangi mesagebroker kullanýcaz onu belirt.
    x.UsingRabbitMq((context, cfg) =>

    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ")); //hostunu tanýmla
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.OrderPaymentSucceedEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentSucceedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.OrderPaymentFailedEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.OrderStockNotReservedEventQueueName, e =>
        {
            e.ConfigureConsumer<StockNotReservedEventConsumer>(context);
        });
        //cfg.ConfigureEndpoints(context);
    });
});

//dbcontext ver baðlantýyý al
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

builder.Services.AddAutoMapper(typeof(MapProfile));

//bunu kendisi AddMassTransit metodunda ekliyormuþ yeni versiyonda
//builder.Services.AddMassTransitHostedService();
//builder.Services.Addmasstra

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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