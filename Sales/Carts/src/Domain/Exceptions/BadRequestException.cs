﻿namespace YourBrand.Carts.Domain.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string title)
    {
        Title = title;
    }

    public string Title { get; }
}
