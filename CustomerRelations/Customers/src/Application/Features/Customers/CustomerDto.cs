﻿using System;

using YourBrand.Customers.Application.Features.Addresses;
using YourBrand.Customers.Domain.Enums;

namespace YourBrand.Customers.Application.Features.Customers;

public record CustomerDto(int Id, CustomerType CustomerType, string Name, string? FirstName, string? LastName, string? SSN, string? OrgNo, string? VATNo, string? Phone, string? PhoneMobile, string? Email, AddressDto Address);
