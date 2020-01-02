using Autofac;
using Autofac.Integration.Mvc;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UserService.Messages.Commands;

namespace NServiceBusWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ServiceBus_Init();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //ServiceBus.Init();
        }

        private async void ServiceBus_Init()
        {
            IEndpointInstance EndpointInstance;

            var endpointConfiguration = new EndpointConfiguration("NServiceBusWeb");
            //endpointConfiguration.SendOnly();

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();

            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.PurgeOnStartup(true);
            endpointConfiguration.EnableInstallers();

            var routing = transport.Routing();
                routing.RouteToEndpoint(
                messageType: typeof(CreateNewUserCmd),
                destination: "UserService"
            );

            endpointConfiguration.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null
                && t.Namespace.EndsWith("Commands"));

            endpointConfiguration.Conventions()
                .DefiningEventsAs(t => t.Namespace != null
                && t.Namespace.EndsWith("Events"));

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            EndpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            // To send a message every 5 minutes
            //await EndpointInstance.ScheduleEvery(
            //    timeSpan: TimeSpan.FromSeconds(2),
            //    task: pipelineContext =>
            //    {
            //        var cmd = new CreateNewUserCmd
            //        {
            //            Name = "Rahul" + new Random().Next(),
            //            EmailAddress = "rahul" + new Random().Next() + "@gmail.com"
            //        };
            //        return pipelineContext.Send(cmd);
            //    }
            //).ConfigureAwait(false);

            var mvcContainerBuilder = new ContainerBuilder();
            mvcContainerBuilder.RegisterInstance(EndpointInstance);

            // Register MVC controllers.
            mvcContainerBuilder.RegisterControllers(typeof(MvcApplication).Assembly);

            var mvcContainer = mvcContainerBuilder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(mvcContainer));
        }
    }
}
