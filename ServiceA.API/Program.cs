using Polly;
using Polly.Extensions.Http;
using ServiceA.API;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//a servisinin b servisine istek yapmas� i�in HTTPCLient nesnesine ihtiyac� var
//kendisi newleyerek almaktansa buradan servisten als�n istiyoruz,bunu ekledi�imizde k�t�phane bakacak daha �nce yarat�lm�� b i httpclientmi yoksa
//s�f�rdan m� olu�turuluyor biz de�il program yappacka bunu.soketleri t�ketmemek i�in
builder.Services.AddHttpClient<ProductService>(opt => //burda  ProductService belirterek o serviste http  client nesnesi istedi�imizde buradan gelecek
{
    //bu product servis nereye ba�lanacak ? B servisindeki products controller
    opt.BaseAddress = new Uri("https://localhost:7294/api/productb/");
})
    .AddPolicyHandler(GetAdvenceCircuitBreakerPolicy()); //burada ekle policy yoksa �al��m�yor.

//retry mekanizmas� �artname policy bunu AddPolicyHandler() metodunun i�ine =>{} girip de yazabilirsin.
IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    //hata meydana gelirse retrymekanizamas� �al��s�n - veya OrResult diyerek status codes not foud ise ekstra d�n�� kodu belirtebiliriz.
    //yani belirlemi� oldu�umuz �artlarda �al��s�n.
    return HttpPolicyExtensions.HandleTransientHttpError().OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        //ba�ar�s�z olursa 5 kez tekrar et, retry attempt giri� yapma say�s� burada her bir retry aras�ndaki s�reyi belirtebiliri.
        .WaitAndRetryAsync(5, retryAttempt =>
        {
            Debug.WriteLine($"Retry Count : {retryAttempt}");
            return TimeSpan.FromSeconds(10); //her bir tekrar aras�nda 10 sn bekle
        }, onRetryAsync: onRetryAsync);
}

//denemeden �nce busness kodu �al��t�rabiliriz ona �rnek,yukar�da verdik,ayn� �ekilde bu metodu onRetryAsync ii�ine =>{} girip yazabilirsin.
Task onRetryAsync(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2)
{
    Debug.WriteLine($"Request is made again : {arg2.TotalMilliseconds}");

    return Task.CompletedTask;
}

//Circuit mekanizmas�

//basit olan pattern
IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    //ard arda 3 tane ba�ar�s�z istek olursa 10 saniye bekle sadeece bunu yap�yor.ona g�re devreyi open -half-open durumuna getirir.
    //ard arda 3 istek ba�ar�s�z ise open durumuna getirir, ard�ndan 10 sn bekler daha sonra tekrar istek al�r 11. sn de b servisine tekrar istek yollar
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
    //30 saniyede ba�ar�s�z olan isteklerin say� y�zdesi 0.1 yani %10 den fazla ise circuit devreye girecek ve minimum ba�ar�s�z say�s� da 3 olsun
    //yani 30 saniyede  2 istek yapt� birisi ba�ar�s�z oldu yine%10 oluyor bu minimum ba�ar�s�z say�s� ise onu 3 e kadar tamamlamas� gerekti�ini belirtiyor.
    //35 saniye sonra ise s�f�rlans�n yani circuitin open(a��k) olma durumu ne kadar s�recek.
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