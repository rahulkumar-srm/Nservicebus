using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Messages.Events;

namespace WelcomeEmailService
{
    public class EmailSender : IHandleMessages<IUserCreatedEvent>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmailSender));

        public async Task Handle(IUserCreatedEvent message, IMessageHandlerContext context)
        {
            await Task.Run(() => {
                log.InfoFormat("Sending welcome email to {0} with UserId {1}", message.EncryptedEmailAddress, message.UserId);
            });
        }
    }
}
