﻿using YourBrand.Catalog.Services;

namespace YourBrand.Catalog.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}

