using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Model;
using SharedLibrary;
using SharedLibrary.Events;
using SharedLibrary.Interfaces;
using SharedLibrary.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderSucceedRequestEventConsumer>();
    x.AddConsumer<OrderFailedRequestEventConsumer>();
    //hangi mesagebroker kullan�caz onu belirt.
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ")); //hostunu tan�mla

        //a�a��da kuyruk tan�ml�yoruz ismini de RabbitMQSettingsConst tan al�yor,StateMachine OrderSucceedRequestEvent ini
        //publish ediyor biz de o eventi yakalamak i�in kuyruk olu�turuyoruz , Consumerda o eventi verdi�imiz i�in bz bu kuyruk ismiyle o eventi
        //yakalayabiliyoruz.
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.OrderSucceedRequestEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderSucceedRequestEventConsumer>(context);
        });

        cfg.ReceiveEndpoint(RabbitMQSettingsConst.OrderFailedRequestEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderFailedRequestEventConsumer>(context);
        });
    });
});

//dbcontext ver ba�lant�y� al
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

builder.Services.AddAutoMapper(typeof(MapProfile));

//bunu kendisi AddMassTransit metodunda ekliyormu� yeni versiyonda
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