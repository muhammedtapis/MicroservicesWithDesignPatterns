﻿using MassTransit;

namespace SharedLibrary.Interfaces
{
    public interface IStockReservedEvent : CorrelatedBy<Guid>
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}