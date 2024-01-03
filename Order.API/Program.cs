using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Model;
using SharedLibrary.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//masstransit
builder.Services.AddMassTransit(x =>
{
    //hangi mesagebroker kullan�caz onu belirt.
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ")); //hostunu tan�mla
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