using MassTransit;
using SharedLibrary;
using SharedLibrary.Events;
using SharedLibrary.Interfaces;
using SharedLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaStateMachineWorkerService.Models
{
    //transactionu yönetecek ve veeritabanında stateleri değiştirecek sınıf , bu sınıf hangi instance model üzerinde çalışacak onu belirttik.
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        //StateMachine bu event geldiğinde her şey tetiklenmeye başlayacak bu event gelince veritabanına satır oluşturacak.
        //bu event bize Order Servisten geliyor tüm bu olayları başlatan servis.
        public Event<IOrderCreatedEvent> OrderCreatedEvent { get; set; }

        //bize yukarıdaki event geldiğinde oluşacak olan yeni stateimiz bu olacak!!
        public State OrderCreated { get; private set; }

        //stockservisi reserve ettiyse bunu statemachine dinleyecek ardından paymenta bi event gönderecek.
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }

        public State StockReserved { get; private set; } //stock reserve edildiyse değiştirilecek state

        //paymentsucceed ise dinleyip ordera eevent göndericek

        public Event<IPaymentSucceedEvent> PaymentSucceedEvent { get; set; }
        public State PaymentSucceed { get; private set; }

        //----------------olumsuz durum için event ve states

        //buraya tanımlaıdğımız eventlerin hepsini statemachine dinliyo.
        public Event<IStockNotReservedEvent> StockNotReservedEvent { get; set; }

        public State StockNotReserved { get; private set; }

        public Event<IPaymentFailedEvent> PaymentFailedEvent { get; set; }
        public State PaymentFailed { get; private set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState); //ilk oluştuulduğında current initial olsun.

            //burası önemli !! status için gerekli correlate
            //OrderCreatedREquestEvent geldiğinde Correlate by da verdiğimiz int orderId integer olduğu için int correlate karşlaştırma yap
            //eventten gelen OrderId ile bu OrderStateInstance tablosundaki OrderId karşılaştır varsa bir şey yapma eğer yoksa bu orderdan
            //bu tabloya bu orderi ekle ve OrderStateInstance modelimizde CorrelationId vardı onun için yeni guid değer oluştur.
            Event(() => OrderCreatedEvent, y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId).SelectId(context => Guid.NewGuid()));
            //eğer yukarıdaki yerde veritabanına bu order eklendiyse  Initial stateden OrderCreated state geçicez.

            //StockReservedEvent fırlatıldığında veritabanında hangi correlationid ye sahip olan satır onu bulup onun state değiştir onu belirtiyoruz.
            Event(() => StockReservedEvent, y => y.CorrelateById(x => x.Message.CorrelationId)); //message eventten gelen oluyo.

            //paymentSuccedEvent fırlatıldıpında jangi satır değişcek hangi idye sahip
            Event(() => PaymentSucceedEvent, y => y.CorrelateById(x => x.Message.CorrelationId));

            //------olmsuz event ilişkilendirmesi OrderStateInstance dbdeki correlation id ile event correlation id ilişkilendirmesi
            //StockNotreserved fırlatıldıpında jangi satır değişcek hangi idye sahip
            Event(() => StockNotReservedEvent, y => y.CorrelateById(x => x.Message.CorrelationId));

            //burada yapacağımız iş created olduktan sonra bu gelen ordereventindeki bilgilerle bizim OrderStateInstancetaki bilgileri doldurmamız lazım
            //OrderCreatedReqestEvent geldiğinde Then yaparak onun içindeki business kod çalışsın
            Initially(When(OrderCreatedEvent).Then(context =>
            {
                //instance veritabanındaki alanlara karşılık geliyorken Data Eventteki alanlara karşılık geliyorlar.
                context.Instance.BuyerId = context.Data.BuyerId;

                context.Instance.OrderId = context.Data.OrderId;
                context.Instance.CreatedDate = DateTime.Now;

                context.Instance.CardName = context.Data.Payment.CardName;
                context.Instance.CardNumber = context.Data.Payment.CardNumber;
                context.Instance.CVV = context.Data.Payment.CVV;
                context.Instance.Expiration = context.Data.Payment.Expiration;
                context.Instance.TotalPrice = context.Data.Payment.TotalPrice;
            }).Then(context =>
            {
                Console.WriteLine($"OrderCreatedRequestEvent before : {context.Instance}");
                //Stock servisi için gönderceğimiz OrderCreatedEvent
            }).Publish(context => new OrderCreatedRequestEvent(context.Instance.CorrelationId) { OrderItems = context.Data.OrderItems })
            //transitionTo kısmı state değiştirme alanı
            .TransitionTo(OrderCreated)
            .Then(context =>
            {
                Console.WriteLine($"OrderCreatedRequestEvent after : {context.Instance}");
            })); //sonra bu işlem bitince hangi state geçicek onu da veriyoruz.

            //THEN bizim business kodu çalıştırcağımızyer.

            //sipariş OrderCreated durumundayken Stock reserve olduysa veya olmadıysa !!!
            //eğer ilgili sipariş için orderCreated state inde ise , ve bu statete When metoduyla birlikte StockReservedEvent gelirse bundan sonra
            //business kodumuzu yazıypruz ve paymenta StockReservedRequestEvent gönderiyoruz.
            During(OrderCreated,
                When(StockReservedEvent)
                .TransitionTo(StockReserved) //yukarıda Event(()=>) şeklinde hangi satırda bu state değiştime işlemi yapoılıyor onu belirtmen lazım
                .Send(new Uri($"queue:{RabbitMQSettingsConst.PaymentStockReservedRequestEventQueueName}")
                , context => new StockReservedRequestEvent(context.Message.CorrelationId)
                {
                    OrderItems = context.Data.OrderItems,
                    Payment = new PaymentMessage() //ödeme bilgilerini veritabanında tuttuüumuz için isntance dan aldık.
                    {
                        CardName = context.Instance.CardName,
                        CardNumber = context.Instance.CardNumber,
                        CVV = context.Instance.CVV,
                        Expiration = context.Instance.Expiration,
                        TotalPrice = context.Instance.TotalPrice
                    },
                    BuyerId = context.Instance.BuyerId
                }).Then(context =>
              {
                  Console.WriteLine($"StockReservedRequestEvent after : {context.Instance}");
              }),
                //Stockservisten olumsuz event gelmsi halinde orderservise requestevent göndercez !! önceden stock not reserved gönderirdik ama artık order
                // sadece succeed ve failed gönderiyoruz
                When(StockNotReservedEvent).TransitionTo(StockNotReserved)
                .Publish(context => new OrderFailedRequestEvent()
                {
                    OrderId = context.Instance.OrderId,
                    Message = context.Data.Message //stocknotreserved den bize hata mesajı geliyo.
                }).Then(context =>
                {
                    Console.WriteLine($"OrderFailedRequestEvent after : {context.Instance}");
                })
                );

            //StockReserve iken Payment başarılı veya başarısız olursa
            During(StockReserved //stock reserved durumundayken
                , When(PaymentSucceedEvent)  //paymentsucceedevent geldiğinde
                .TransitionTo(PaymentSucceed).Publish(context => new OrderSucceedRequestEvent()
                {
                    OrderId = context.Instance.OrderId
                }).Then(context =>
                {
                    Console.WriteLine($"OrderSucceedRequestEvent after : {context.Instance}");
                }).Finalize(),

                //eğerpayment fail olursa

                When(PaymentFailedEvent).Publish(context => new OrderFailedRequestEvent() //orderın dinleyeceği event.
                {
                    OrderId = context.Instance.OrderId,
                    Message = context.Data.Message //stocknotreserved den bize hata mesajı geliyo.
                })
                //stock a rollback içinmesaj yolluyoruz bu sefer send yaptık çünkü stock rollbackini stock servis yapacak sadece o dinlsin
                .Send(new Uri($"queue:{RabbitMQSettingsConst.StockRollBackRequestMessageQueueName}")
                , context => new StockRollBackRequestMessage() //stockun rollback için dinleyeceği mesaj
                {
                    OrderItems = context.Data.OrderItems
                })
                .TransitionTo(PaymentFailed).Then(context =>
                {
                    Console.WriteLine($"StockRollBackRequestMessage after : {context.Instance}");
                })
                ); //veritabanında işlemi sonlandır artık bitmiş her şey başarılıysa son servsten de succed gelirse

            //eğer order status final ise yani order oluşturulmuş ise InstanceTablosunda durmasına gerek yok onu sil!!
            SetCompletedWhenFinalized();
        }
    }
}