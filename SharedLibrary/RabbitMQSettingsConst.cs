namespace SharedLibrary
{
    public class RabbitMQSettingsConst
    {
        //stock apide ORderCreatedEvent bekleyen kuyruğumuzun adı.
        public const string StockOrderCreatedEventQueueName = "stock-order-created-queue"; //stock için, orderCreatedevent dinleyeceği kuyruk

        //payment apide stockreservedevent bekleyen kuyruk
        public const string StockReservedEventQueueName = "stock-reserved-queue"; //stoğun kendi fıralttığı eventin girdiği kuyruk send metoduyla gönderdiğimiz event kuyruğu

        //bu kuyruk ismini kullanmak için StockREserved eventini send olarak değil de publish olarak göndermemiz gerekiyordu,
        //yukarıdaki stockReservedEventQueue üzerinden send edildiği için paymentta yine aynı yukarıdaki stockreservedqueue üzerinden yakalayabilir.
        //public const string PaymentStockReservedEventQueueName = "payment-stock-reserved-queue"; //payment için subscribe olduğumuz kuyruk ismi ama bu çalışmadı  üsttekini kullandık

        //Order.API de bulunan PaymentSucceedEventConsumer için bu kuyruk paymenttan gelen eventi yakalamak için
        public const string OrderPaymentSucceedEventQueueName = "order-payment-succeed-queue";

        public const string OrderPaymentFailedEventQueueName = "order-payment-failed-queue";

        public const string OrderStockNotReservedEventQueueName = "order-stock-not-reserved-queue";

        public const string StockPaymentFailedEventQueueName = "stock-payment-failed-queue";

        //<------------Orchestration ------------>

        public const string OrderSaga = "order-saga-queue";

        //sendle gönderdik alttakine statemachine de bu kuyruğa sendlemiştik payment okumak için aynı kuyruğa kaydolcak.
        public const string PaymentStockReservedRequestEventQueueName = "payment-stock-reserved-request-queue"; //orderstatein paymenta gönderdiği tetikleme event

        //önceden orderda 3 tame cosumer vardı artık iki tane var sadece fail success
        public const string OrderSucceedRequestEventQueueName = "order-succeed-request-queue";

        public const string OrderFailedRequestEventQueueName = "order-failed-request-queue";

        public const string StockRollBackRequestMessageQueueName = "stock-rollback-request-message-queue";
    }
}