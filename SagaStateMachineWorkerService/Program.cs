using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachineWorkerService;
using SagaStateMachineWorkerService.Models;
using SharedLibrary;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>  //burada hostcontext ekledik �nemli yoksa configuration.getconnection apsettings okumas� yapamay�z.
    {
        services.AddMassTransit(cfg =>
        {
            //statemachine ver hangi instance kullanack onu ver ard�ndan hangi repository kullancan onu ver ver ayarlar� tan�mla
            cfg.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
            {
                opt.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                {
                    builder.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlConnection"), m =>
                    {
                        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    });
                });
            });

            //rabbitMQ ayar�

            cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                configure.Host(hostContext.Configuration.GetConnectionString("RabbitMQ"));

                //ilk f�rlat�lan eventteki tet�klenme olay� SagaStateMachine initialdan OrderCreated a tetiklenniyor
                //dinledi�i kuyruk OrderSaga kuyru�u buraya mesah geldi�inde orderState tetikleniyor.

                configure.ReceiveEndpoint(RabbitMQSettingsConst.OrderSaga, e =>
                {
                    //tetiklenme olay�n� yaz ukar�dakki kuyrup�a bir haber geldi�inde a�a��daki s�n�ftan nesne �rne�i al�n�p
                    //veritaban�na sat�r yaz�lcak, kuyru�a bi event gelirse bize orderstateinstance olu�tur ki bunun i�ini doldurup db ye kaydedelim.
                    e.ConfigureSaga<OrderStateInstance>(provider);
                });
            }));
        });

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();