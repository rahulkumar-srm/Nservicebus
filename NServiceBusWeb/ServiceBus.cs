using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using UserService;
using UserService.Messages.Commands;

namespace NServiceBusWeb
{
    public sealed class ServiceBus
    {
        public static IEndpointInstance EndpointInstance { get; private set; }
        //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public static async void Init()
        {
            if (EndpointInstance != null)
                return;

            await semaphoreSlim.WaitAsync();
            try
            {
                if (EndpointInstance != null)
                    return;

                var endpointConfiguration = new EndpointConfiguration("NServiceBusWeb");
                endpointConfiguration.SendOnly();

                var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                transport.UseConventionalRoutingTopology();

                endpointConfiguration.UsePersistence<LearningPersistence>();
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                endpointConfiguration.PurgeOnStartup(true);
                endpointConfiguration.EnableInstallers();

                endpointConfiguration.Conventions()
                    .DefiningCommandsAs(t => t.Namespace != null
                    && t.Namespace.EndsWith("Commands"));

                endpointConfiguration.Conventions()
                    .DefiningEventsAs(t => t.Namespace != null
                    && t.Namespace.EndsWith("Events"));

                endpointConfiguration.SendFailedMessagesTo("error");
                endpointConfiguration.AuditProcessedMessagesTo("audit");

                //endpointConfiguration.ExecuteTheseHandlersFirst(typeof(DeferMessagesUntilReady));

                EndpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                semaphoreSlim.Release();
            }

        }
    }
}