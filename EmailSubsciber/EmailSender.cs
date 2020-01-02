using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Logging;
using NServiceBus.Transport;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UserService.Messages.Commands;
using UserService.Messages.Events;

namespace EmailSubsciber
{
    public class EmailSender : IHandleMessages<UserService.Messages.Events.ICorporateUserCreatedEvent>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmailSender));

        SqlConnection _Con = null;
        SqlCommand _cmd = null;

        public async Task Handle(UserService.Messages.Events.ICorporateUserCreatedEvent message, IMessageHandlerContext context)
        {
            
            await Task.Run(() =>
            {
                
                log.InfoFormat("Sending welcome email to {0} with CorporateUserId {1}", message.EncryptedEmailAddress, message.CorporateUserId);

                _Con = new SqlConnection(@"Data Source=GUR00251545L\SQLEXPRESS;Initial Catalog=update_and_publish;User ID=sa;Password=Password@123");
                _cmd = new SqlCommand("INSERT INTO [update_and_publish].[dbo].[Orders] VALUES (7)", _Con);
                _Con.Open();
                _cmd.ExecuteNonQuery();

                var sendOptions = new SendOptions();
                //var transportTransaction = new TransportTransaction();
                //transportTransaction.Set(_Con);
                //sendOptions.GetExtensions().Set(transportTransaction);
                sendOptions.SetDestination("Samples.TestDomain");

                context.Send(new TestCmd { TestString = "This is a test message....." }, sendOptions).ConfigureAwait(false);
            });
        }
    }
}
