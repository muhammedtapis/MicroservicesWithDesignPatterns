using EventSourcing.API.EventStores;
using EventSourcing.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//extension eventstore ekle
builder.Services.AddEventStore(builder.Configuration);
builder.Services.AddSingleton<ProductStream>(); //productstream tan�t�m singleton eklendi

builder.Services.AddMediatR(Assembly.GetExecutingAssembly()); //mediatr eklemesi

//dbcontext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

//----------
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