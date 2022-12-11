﻿namespace YourBrand.Orders.Domain.Events;

public sealed record ProductPriceChanged(string ProductId, decimal Price, decimal? CompareAtPrice) : DomainEvent;