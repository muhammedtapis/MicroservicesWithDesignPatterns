using MassTransit;
using SharedLibrary;
using SharedLibrary.Events;
using SharedLibrary.Interfaces;
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
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }

        //bize yukarıdaki event geldiğinde oluşacak olan yeni stateimiz bu olacak!!
        public State OrderCreated { get; private set; }

        //stockservisi reserve ettiyse bunu statemachine dinleyecek ardından paymenta bi event gönderecek.
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }

        public State StockReserved { get; private set; } //stock reserve edildiyse değiştirilecek state

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState); //ilk oluştuulduğında current initial olsun.

            //burası önemli !!
            //OrderCreatedREquestEvent geldiğinde Correlate by da verdiğimiz int orderId integer olduğu için int correlate karşlaştırma yap
            //eventten gelen OrderId ile bu OrderStateInstance tablosundaki OrderId karşılaştır varsa bir şey yapma eğer yoksa bu orderdan
            //bu tabloya bu orderi ekle ve OrderStateInstance modelimizde CorrelationId vardı onun için yeni guid değer oluştur.
            Event(() => OrderCreatedRequestEvent, y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId).SelectId(context => Guid.NewGuid()));

            //eğer yukarıdaki yerde veritabanına bu order eklendiyse  Initial stateden OrderCreated state geçicez.

            //burada yapacağımız iş created olduktan sonra bu gelen ordereventindeki bilgilerle bizim OrderStateInstancetaki bilgileri doldurmamız lazım
            //OrderCreatedReqestEvent geldiğinde Then yaparak onun içindeki business kod çalışsın
            Initially(When(OrderCreatedRequestEvent).Then(context =>
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
            }).Publish(context => new OrderCreatedEvent(context.Instance.CorrelationId) { OrderItems = context.Data.OrderItems })
            //transitionTo kısmı state değiştirme alanı
            .TransitionTo(OrderCreated)
            .Then(context =>
            {
                Console.WriteLine($"OrderCreatedRequestEvent after : {context.Instance}");
            })); //sonra bu işlem bitince hangi state geçicek onu da veriyoruz.

            //THEN bizim business kodu çalıştırcağımızyer.

            //eğer ilgili sipariş için orderCreated state inde ise , ve bu statete When metoduyla birlikte StockReservedEvent gelirse bundan sonra
            //business kodumuzu yazıypruz ve paymenta StockReservedRequestEvent gönderiyoruz.
            During(OrderCreated,
                When(StockReservedEvent)
                .TransitionTo(StockReserved)
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
              })
                );
        }
    }
}