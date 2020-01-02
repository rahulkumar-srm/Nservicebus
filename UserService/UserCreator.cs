using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Messages.Commands;
using NServiceBus;
using NServiceBus.Logging;
using UserService.Messages.Events;

namespace UserService
{
    public class UserCreator : IHandleMessages<CreateNewUserCmd>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserCreator));
       
        public async Task Handle(CreateNewUserCmd message, IMessageHandlerContext context)
        {
            await Task.Run(() =>
            {
                log.InfoFormat("Creating user '{0}' with email '{1}'", message.Name, message.EmailAddress);
            });

            await context.Publish<ICorporateUserCreatedEvent>(evt =>
            {
                evt.UserId = Guid.NewGuid();
                evt.CorporateUserId = Guid.NewGuid();
                evt.Name = message.Name;
                evt.EncryptedEmailAddress = message.EmailAddress;
            }).ConfigureAwait(false);

            //log.InfoFormat($"Handling {nameof(CreateNewUserCmd)} with MessageId:{context.MessageId}");
            //throw new Exception("An exception occurred in the handler.");
        }
    }
}