using EventSourcing.Shared.Events;
using EventStore.ClientAPI;
using System.Text;
using System.Text.Json;

namespace EventSourcing.API.EventStores
{
    //eventstore ile ilgili client küütphanesini yükklemen lazım
    public abstract class AbstractStream
    {
        //eventlerimizi tutacağımız bir liste
        protected readonly LinkedList<IEvent> Events = new LinkedList<IEvent>();

        //stream ismini al ?
        private string? _streamName { get; }

        private readonly IEventStoreConnection _eventStoreConnection; //DI ekleme yap eventstore event eklemekj için kullanılıyo

        protected AbstractStream(string? streamName, IEventStoreConnection eventStoreConnection)
        {
            _streamName = streamName;
            _eventStoreConnection = eventStoreConnection;
        }

        //eventstore kaydediceğimiz yer. bu abstract sınıfı ProductStream gibi sınıflar miras alcak
        public async Task SaveAsync()
        {
            //şimdi bu streame eventlerimiz gelecek biz bu eventlerin her birinden bir eventData nesnesi türetmek zorundayız
            //eventstore event kaydedebilmek için mutlaka eventdata nesnesine ihtiyyacımız var.
            var newEvents = Events.ToList().Select(x => new EventData(
                Guid.NewGuid(),
                x.GetType().Name, //
                true,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x, inputType: x.GetType())), //eventdatası json serialize
                Encoding.UTF8.GetBytes(x.GetType().FullName!) //metadata
                )).ToList();

            await _eventStoreConnection.AppendToStreamAsync(_streamName, ExpectedVersion.Any, newEvents);

            //daha sonra eventlerin olduğu listeyi temizle
            Events.Clear();
        }
    }
}