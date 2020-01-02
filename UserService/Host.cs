using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using UserService.Messages.Shared;

namespace UserService
{
    class Host
    {
        // TODO: optionally choose a custom logging library
        // https://docs.particular.net/nservicebus/logging/#custom-logging
        // LogManager.Use<TheLoggingFactory>();
        static readonly ILog log = LogManager.GetLogger<Host>();

        IEndpointInstance endpointInstance;

        // TODO: give the endpoint an appropriate name
        public string EndpointName => "UserService";

        public async Task Start()
        {
            try
            {
                // TODO: consider moving common endpoint configuration into a shared project
                // for use by all endpoints in the system
                var endpointConfiguration = new EndpointConfiguration(EndpointName);

                // TODO: ensure the most appropriate serializer is chosen
                // https://docs.particular.net/nservicebus/serialization/
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

                endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);

                // TODO: remove this condition after choosing a transport, persistence and deployment method suitable for production
                if (Environment.UserInteractive && Debugger.IsAttached)
                {
                    // TODO: choose a durable transport for production
                    // https://docs.particular.net/transports/
                    var transportExtensions = endpointConfiguration.UseTransport<RabbitMQTransport>();
                    transportExtensions.UseConventionalRoutingTopology();

                    // TODO: choose a durable persistence for production
                    // https://docs.particular.net/persistence/
                    endpointConfiguration.UsePersistence<LearningPersistence>();

                    endpointConfiguration.Conventions()
                    .DefiningCommandsAs(t => t.Namespace != null
                    && t.Namespace.EndsWith("Commands"));

                    endpointConfiguration.Conventions()
                        .DefiningEventsAs(t => t.Namespace != null
                        && t.Namespace.EndsWith("Events"));

                    var recoverability = endpointConfiguration.Recoverability();
                    //recoverability.CustomPolicy(MyCustomRetryPolicy);
                    recoverability.Immediate(
                        delayed =>
                        {
                            delayed.NumberOfRetries(3);
                        });
                        recoverability.Delayed(
                        delayed =>
                        {
                            delayed.NumberOfRetries(0);
                        //delayed.TimeIncrease(TimeSpan.FromMinutes(5));
                     });

                    endpointConfiguration.ConfigurationEncryption();

                    endpointConfiguration.PurgeOnStartup(true);

                    // TODO: create a script for deployment to production
                    endpointConfiguration.EnableInstallers();
                }

                // NServiceBus will move messages that fail repeatedly to a separate "error" queue. We recommend
                // that you start with a shared error queue for all your endpoints for easy integration with ServiceControl.
                endpointConfiguration.SendFailedMessagesTo("error");

                // NServiceBus will store a copy of each successfully process message in a separate "audit" queue. We recommend
                // that you start with a shared audit queue for all your endpoints for easy integration with ServiceControl.
                endpointConfiguration.AuditProcessedMessagesTo("audit");

                endpointConfiguration.RegisterComponents(reg =>
                {
                    reg.ConfigureComponent<IEndpointInstance>(
                    DependencyLifecycle.SingleInstance);
                });

                // TODO: perform any futher start up operations before or after starting the endpoint
                endpointInstance = await Endpoint.Start(endpointConfiguration);
            }
            catch (Exception ex)
            {
                FailFast("Failed to start.", ex);
            }
        }

        public async Task Stop()
        {
            try
            {
                // TODO: perform any futher shutdown operations before or after stopping the endpoint
                await endpointInstance?.Stop();
            }
            catch (Exception ex)
            {
                FailFast("Failed to stop correctly.", ex);
            }
        }

        async Task OnCriticalError(ICriticalErrorContext context)
        {
            // TODO: decide if stopping the endpoint and exiting the process is the best response to a critical error
            // https://docs.particular.net/nservicebus/hosting/critical-errors
            // and consider setting up service recovery
            // https://docs.particular.net/nservicebus/hosting/windows-service#installation-restart-recovery
            try
            {
                await context.Stop();
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        void FailFast(string message, Exception exception)
        {
            try
            {
                log.Fatal(message, exception);

                // TODO: when using an external logging framework it is important to flush any pending entries prior to calling FailFast
                // https://docs.particular.net/nservicebus/hosting/critical-errors#when-to-override-the-default-critical-error-action
            }
            finally
            {
                //Environment.FailFast(message, exception);
            }
        }
    }
}
