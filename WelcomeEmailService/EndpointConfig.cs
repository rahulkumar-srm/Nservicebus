
namespace WelcomeEmailService
{
    using NServiceBus;
    using NServiceBus.Features;
    using UserService.Messages.Shared;

    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            //TODO: NServiceBus provides multiple durable storage options, including SQL Server, RavenDB, and Azure Storage Persistence.
            // Refer to the documentation for more details on specific options.
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

            endpointConfiguration.ConfigurationEncryption();

            // NServiceBus will move messages that fail repeatedly to a separate "error" queue. We recommend
            // that you start with a shared error queue for all your endpoints for easy integration with ServiceControl.
            endpointConfiguration.SendFailedMessagesTo("error");

            // NServiceBus will store a copy of each successfully process message in a separate "audit" queue. We recommend
            // that you start with a shared audit queue for all your endpoints for easy integration with ServiceControl.
            endpointConfiguration.AuditProcessedMessagesTo("audit");
        }
    }
}
