using EventSourcing.API.DTOs;
using EventSourcing.Shared.Events;
using EventStore.ClientAPI;

namespace EventSourcing.API.EventStores
{
    public class ProductStream : AbstractStream
    {
        //burada DTOlardan gelen bilgilerle eventlerimizi EVentStorea gidecek olan EVents listesine ekleniyor daha SAveAsync metodunu çağırmadık üst sınıftaki
        //o metodu çağırdıktan sonra eventData oluşturup eventstore ekliyor.
        //stream ismi göndermemiz lazım
        public static string StreamName => "ProductStream"; //sadece get var seti yok lambda işareti var.

        //bu groupname ProductReadModelEventStore içindeki executeasync metodunda ConnectToPersistentSubscriptionAsync metodunu çağırdığımız zaman
        //productstreami okuyup dinleyebilmek için gerekli AbstractStream içinde saveasync metoduyla eventleri streame gönderiyoruz
        //ProductReadModelEventStore ise o streame sub oluyoruz ve eventleri alıyoruz.
        public static string GroupName => "agroup"; //eventstoreda stream oluştururken bu grup ismini verdik o sebeple burada da aynı ismi aldık.

        public ProductStream(IEventStoreConnection eventStoreConnection) : base(StreamName, eventStoreConnection) //burada oluşturduğumuz streamı gönder
        {
        }

        public void Created(CreateProductDTO createProductDTO)
        {
            //eventimizi ekliyoruz. AddLast metodu LinkedList olduğu için geliyor.
            Events.AddLast(new ProductCreatedEvent()
            {
                Id = Guid.NewGuid(),
                Name = createProductDTO.Name,
                Price = createProductDTO.Price,
                Stock = createProductDTO.Stock,
                UserId = createProductDTO.UserId
            });
        }

        public void NameChanged(ChangeProductNameDTO changeProductNameDTO)
        {
            Events.AddLast(new ProductNameChangedEvent() { Id = changeProductNameDTO.Id, ChangedName = changeProductNameDTO.Name });
        }

        public void PriceChanged(ChangeProductPriceDTO changeProductPriceDTO)
        {
            Events.AddLast(new ProductPriceChangedEvent() { Id = changeProductPriceDTO.Id, ChangedPrice = changeProductPriceDTO.Price });
        }

        public void Deleted(Guid id)
        {
            Events.AddLast(new ProductDeletedEvent() { Id = id });
        }
    }
}