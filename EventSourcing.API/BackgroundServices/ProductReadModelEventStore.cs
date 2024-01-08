using EventSourcing.API.EventStores;
using EventSourcing.API.Models;
using EventSourcing.Shared.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace EventSourcing.API.BackgroundServices
{//EVENTLERİN İŞLENDİĞİ YER
    //SUB OLAN YERİN İŞLEMLERİ eventstoredaki grupa streame sub olunca ordan gelen event tiplerine göre işlem yapıyor.
    //bu bir background servis uygulama ayaa kalkınca eventstore sub olcak.
    //background servis singleton dbcontext ensnesi scnope o sebeple erişemezsin bunu aşmak için serviceprovider alcaz.
    public class ProductReadModelEventStore : BackgroundService
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger<ProductReadModelEventStore> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ProductReadModelEventStore(IEventStoreConnection eventStoreConnection, ILogger<ProductReadModelEventStore> logger, IServiceProvider serviceProvider)
        {
            _eventStoreConnection = eventStoreConnection;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        //uygulama ayağa kalktığında bu metod bi kez çalışır.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(ProductStream.StreamName, ProductStream.GroupName, EventAppearedAsync,
                autoAck: false); // burası true olursa mesajı gönderdi hata olmadan eventappeared çalışırsa bi daha o eventi göndermez
            //bu işlemi mnuel yapabiliriz her işlem başarılıysa en son eventappeared metodunda event id gönderiyorz eventstore bilgi veriyoruz bu event bana geldi.
        }

        //ayapa kalkarken
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        //kapanırken
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        //connecttopersistensub metodu için oluşturduğumuz metod
        //bu metod bizim sub dashboarddaki eventler her sub olana gönderildiğinde burası tetiklencek
        private async Task EventAppearedAsync(EventStorePersistentSubscriptionBase arg1, ResolvedEvent arg2)
        {
            //_logger.LogInformation("The message processing..");

            //gelen eventin tipini al,bunu gelen eventin metadatasını okuyarak ycpaz
            //burada tipi aldığımız için artık bu eventin hangi event olduğunu anlayacağız ve ona göre sql eleme yapacağız.
            var type = Type.GetType($"{Encoding.UTF8.GetString(arg2.Event.Metadata)},EventSourcing.Shared");
            _logger.LogInformation($"The message processing... : {type.ToString()}");
            //datayı al
            var eventData = Encoding.UTF8.GetString(arg2.Event.Data);

            //eventdatayı yukarıdaki tipe deserialize etmen lazım,yani eventin datasını eventin tipine o evente deserialize ediyor.
            var @event = JsonSerializer.Deserialize(eventData, type);

            //datayı aldık elimizde var veritabanına eklicez artık
            using var scope = _serviceProvider.CreateScope(); //scope üzerinden dbcontexte erişeceğiz.

            //Scope ile her bir event dinlediğimizde bunu alıyoruz. sonra kayboluyor sebebiyse 1000 event geldi sonra 10 saat event gelmedi o süre boyunca ayakta olmasına gerek yok.
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Product product = null;

            //burada gelen eventin kontrolünü yapıp ona göre işlemler yapıyoruz.
            switch (@event)
            {
                case ProductCreatedEvent productCreatedEvent:
                    product = new Product()
                    {
                        Id = productCreatedEvent.Id,
                        Name = productCreatedEvent.Name,
                        Price = productCreatedEvent.Price,
                        Stock = productCreatedEvent.Stock,
                        UserId = productCreatedEvent.UserId
                    };
                    context.Products.Add(product);
                    break;

                case ProductNameChangedEvent productNameChangedEvent:
                    product = context.Products.Find(productNameChangedEvent.Id);
                    if (product != null)
                    {
                        product.Name = productNameChangedEvent.ChangedName;
                    }
                    break;

                case ProductPriceChangedEvent productPriceChangedEvent:
                    product = context.Products.Find(productPriceChangedEvent.Id);
                    if (product != null)
                    {
                        product.Price = productPriceChangedEvent.ChangedPrice;
                    }
                    break;

                case ProductDeletedEvent productDeletedEvent:
                    product = context.Products.Find(productDeletedEvent.Id);
                    if (product != null)
                    {
                        context.Products.Remove(product);
                    }
                    break;
            }
            await context.SaveChangesAsync();
            arg1.Acknowledge(arg2.Event.EventId);
        }
    }
}