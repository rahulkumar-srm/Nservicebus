using NServiceBus;
using System;
using System.Threading.Tasks;
using UserService.Messages.Shared;

namespace EmailSubsciber
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Samples.PubSub.Subscriber";
            var endpointConfiguration = new EndpointConfiguration("Samples.PubSub.Subscriber");
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            transport.UseConventionalRoutingTopology();

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.EnableInstallers();

            endpointConfiguration.Conventions()
                    .DefiningCommandsAs(t => t.Namespace != null
                    && t.Namespace.EndsWith("Commands"));

            endpointConfiguration.Conventions()
                .DefiningEventsAs(t => t.Namespace != null
                && t.Namespace.EndsWith("Events"));

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Immediate(
                delayed =>
                {
                    delayed.NumberOfRetries(3);
                });
            recoverability.Delayed(
            delayed =>
            {
                delayed.NumberOfRetries(0);
            });

            endpointConfiguration.ConfigurationEncryption();

            endpointConfiguration.EnableOutbox();

            var unitOfWork = endpointConfiguration.UnitOfWork();
            unitOfWork.WrapHandlersInATransactionScope();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
