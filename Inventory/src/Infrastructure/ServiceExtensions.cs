﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using YourBrand.Inventory.Infrastructure.BackgroundJobs;
using YourBrand.Inventory.Infrastructure.Persistence;
using YourBrand.Inventory.Infrastructure.Services;
using MediatR;
using YourBrand.Inventory.Infrastructure.Idempotence;

namespace YourBrand.Inventory.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);

            services.AddScoped<IDateTime, DateTimeService>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IBlobStorageService, BlobStorageService>();

            try
            {
                services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
            }
            catch { }

            services.AddQuartz(configure =>
            {
                var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

                configure
                    .AddJob<ProcessOutboxMessagesJob>(jobKey)
                    .AddTrigger(trigger => trigger.ForJob(jobKey)
                        .WithSimpleSchedule(schedule => schedule
                            .WithIntervalInSeconds(10)
                            .RepeatForever()));

                configure.UseMicrosoftDependencyInjectionJobFactory();
            });

            services.AddQuartzHostedService();

            return services;
        }
    }
}
