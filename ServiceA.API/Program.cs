using Polly;
using Polly.Extensions.Http;
using ServiceA.API;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//a servisinin b servisine istek yapmasý için HTTPCLient nesnesine ihtiyacý var
//kendisi newleyerek almaktansa buradan servisten alsýn istiyoruz,bunu eklediðimizde kütüphane bakacak daha önce yaratýlmýþ b i httpclientmi yoksa
//sýfýrdan mý oluþturuluyor biz deðil program yappacka bunu.soketleri tüketmemek için
builder.Services.AddHttpClient<ProductService>(opt => //burda  ProductService belirterek o serviste http  client nesnesi istediðimizde buradan gelecek
{
    //bu product servis nereye baðlanacak ? B servisindeki products controller
    opt.BaseAddress = new Uri("https://localhost:7294/api/productb/");
})
    .AddPolicyHandler(GetAdvenceCircuitBreakerPolicy()); //burada ekle policy yoksa çalýþmýyor.

//retry mekanizmasý þartname policy bunu AddPolicyHandler() metodunun içine =>{} girip de yazabilirsin.
IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    //hata meydana gelirse retrymekanizamasý çalýþsýn - veya OrResult diyerek status codes not foud ise ekstra dönüþ kodu belirtebiliriz.
    //yani belirlemiþ olduðumuz þartlarda çalýþsýn.
    return HttpPolicyExtensions.HandleTransientHttpError().OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        //baþarýsýz olursa 5 kez tekrar et, retry attempt giriþ yapma sayýsý burada her bir retry arasýndaki süreyi belirtebiliri.
        .WaitAndRetryAsync(5, retryAttempt =>
        {
            Debug.WriteLine($"Retry Count : {retryAttempt}");
            return TimeSpan.FromSeconds(10); //her bir tekrar arasýnda 10 sn bekle
        }, onRetryAsync: onRetryAsync);
}

//denemeden önce busness kodu çalýþtýrabiliriz ona örnek,yukarýda verdik,ayný þekilde bu metodu onRetryAsync iiçine =>{} girip yazabilirsin.
Task onRetryAsync(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2)
{
    Debug.WriteLine($"Request is made again : {arg2.TotalMilliseconds}");

    return Task.CompletedTask;
}

//Circuit mekanizmasý

//basit olan pattern
IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    //ard arda 3 tane baþarýsýz istek olursa 10 saniye bekle sadeece bunu yapýyor.ona göre devreyi open -half-open durumuna getirir.
    //ard arda 3 istek baþarýsýz ise open durumuna getirir, ardýndan 10 sn bekler daha sonra tekrar istek alýr 11. sn de b servisine tekrar istek yollar
    return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(10), onBreak: (arg1, arg2) =>
    {
        Debug.WriteLine("circuit breaker status => on BREAK");
    }, onReset: () =>
    {
        Debug.WriteLine("circuit breaker status => on RESET");
    }, onHalfOpen: () =>
    {
        Debug.WriteLine("circuit breaker status => on HALF-OPEN");
    });
}

//zor olan pattern

IAsyncPolicy<HttpResponseMessage> GetAdvenceCircuitBreakerPolicy()
{
    //30 saniyede baþarýsýz olan isteklerin sayý yüzdesi 0.1 yani %10 den fazla ise circuit devreye girecek ve minimum baþarýsýz sayýsý da 3 olsun
    //yani 30 saniyede  2 istek yaptý birisi baþarýsýz oldu yine%10 oluyor bu minimum baþarýsýz sayýsý ise onu 3 e kadar tamamlamasý gerektiðini belirtiyor.
    //35 saniye sonra ise sýfýrlansýn yani circuitin open(açýk) olma durumu ne kadar sürecek.
    return HttpPolicyExtensions.HandleTransientHttpError().AdvancedCircuitBreakerAsync(0.1, TimeSpan.FromSeconds(30), 3, TimeSpan.FromSeconds(35), onBreak: (arg1, arg2) =>
    {
        Debug.WriteLine("circuit breaker status => on BREAK");
    }, onReset: () =>
    {
        Debug.WriteLine("circuit breaker status => on RESET");
    }, onHalfOpen: () =>
    {
        Debug.WriteLine("circuit breaker status => on HALF-OPEN");
    });
}

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