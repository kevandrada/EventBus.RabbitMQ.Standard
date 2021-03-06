﻿using Autofac;
using EventBus.RabbitMQ.Standard.Base;
using EventBus.RabbitMQ.Standard.Options;
using EventBus.RabbitMQ.Standard.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.RabbitMQ.Standard
{
    public static class EventBus
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, EventBusOptions options)
        {
            services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
            {
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var lifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionManager>();

                var brokerName = options.BrokerName;
                var autofacScopeName = options.AutofacScopeName;
                var queueName = options.QueueName;
                var retryCount = 5;

                if (!string.IsNullOrEmpty(options.RetryCount))
                {
                    retryCount = int.Parse(options.RetryCount);
                }

                return new EventBusRabbitMq(rabbitMqPersistentConnection,
                    lifetimeScope,
                    eventBusSubscriptionsManager,
                    brokerName,
                    autofacScopeName,
                    queueName,
                    retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();

            return services;
        }
    }
}