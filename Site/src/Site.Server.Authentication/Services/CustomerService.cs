﻿using Site.Server.Authentication.Data;
using Microsoft.EntityFrameworkCore;
using YourBrand.Customers;
using System.Threading;

namespace Site.Server.Authentication.Services;

public class CustomerService : ICustomerService
{
    private readonly UsersContext usersContext;

    public CustomerService(UsersContext usersContext)
    {
        this.usersContext = usersContext;
    }

    public async Task AddUser(User user, CancellationToken cancellationToken = default)
    {
        usersContext.Users.Add(user);
        await usersContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUserByCustomerId(int customerId, CancellationToken cancellationToken = default)
    {
        return await usersContext.Users.FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

    }

    public async Task UpdateUser(User user, CancellationToken cancellationToken = default)
    {
        usersContext.Users.Update(user);
        await usersContext.SaveChangesAsync(cancellationToken);
    }
}


