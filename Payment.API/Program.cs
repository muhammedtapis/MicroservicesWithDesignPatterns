using MassTransit;
using Payment.API.Consumers;
using SharedLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMassTransit(x =>
{
    //consumer bu servis hangi consumera sahip ?
    x.AddConsumer<StockReservedEventConsumer>();

    //hangi mesagebroker kullan�caz onu belirt.
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ")); //hostunu tan�mla

        //bak buras� �ok �nemli sen burada ,stockReservedEvent nereye yolluyosa queueyi o kuyru�a bakacaks�n!!
        //buras� StockREservedEvent bize send metoduyla geldi�i zamna b�yle oluyor ��nk� orada belirriyosun kuyru�u!!!
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockReservedEventQueueName, e =>
        {//e �zerinden bu kuyru�u hangi consumer dinleyecek onu belirtiyoruz.
            e.ConfigureConsumer<StockReservedEventConsumer>(context);
        });
    });
});

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