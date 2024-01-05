using EventStore.ClientAPI;

namespace EventSourcing.API.EventStores
{
    public static class EventStoreExtension
    {
        //programcs için extension,ve Eventstore yolunu almak için configuration lazım.
        public static void AddEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = EventStoreConnection.Create(connectionString: configuration.GetConnectionString("EventStore"));

            connection.ConnectAsync().Wait();

            services.AddSingleton(connection);

            //loglama yapıyoruz
            using var logFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
            });

            var logger = logFactory.CreateLogger("Program");

            //yıldırım şeklinde olanlar event demek connection.Connected bir event oluyo o meydana geldiği an bu loglamayı yapıyoruz.
            connection.Connected += (sender, args) =>
            {
                logger.LogInformation("Eventstore connection established");
            };

            connection.ErrorOccurred += (sender, args) =>
            {
                logger.LogError(args.Exception.Message);
            };
        }
    }
}