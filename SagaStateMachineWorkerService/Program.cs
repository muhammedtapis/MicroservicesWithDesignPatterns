using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachineWorkerService;
using SagaStateMachineWorkerService.Models;
using SharedLibrary;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>  //burada hostcontext ekledik önemli yoksa configuration.getconnection apsettings okumasý yapamayýz.
    {
        services.AddMassTransit(cfg =>
        {
            //statemachine ver hangi instance kullanack onu ver ardýndan hangi repository kullancan onu ver ver ayarlarý tanýmla
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

            //rabbitMQ ayarý

            cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                configure.Host(hostContext.Configuration.GetConnectionString("RabbitMQ"));

                //ilk fýrlatýlan eventteki tetþklenme olayý SagaStateMachine initialdan OrderCreated a tetiklenniyor
                //dinlediði kuyruk OrderSaga kuyruðu buraya mesah geldiðinde orderState tetikleniyor.

                configure.ReceiveEndpoint(RabbitMQSettingsConst.OrderSaga, e =>
                {
                    //tetiklenme olayýný yaz ukarýdakki kuyrupða bir haber geldiðinde aþaðýdaki sýnýftan nesne örneði alýnýp
                    //veritabanýna satýr yazýlcak, kuyruða bi event gelirse bize orderstateinstance oluþtur ki bunun içini doldurup db ye kaydedelim.
                    e.ConfigureSaga<OrderStateInstance>(provider);
                });
            }));
        });

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();