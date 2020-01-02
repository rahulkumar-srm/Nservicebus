using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Transport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Messages.Commands;
using UserService.Messages.Events;

namespace EmailSubsciber
{
    public class ICorporateUserCreatedEvent : IHandleMessages<UserService.Messages.Events.ICorporateUserCreatedEvent>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ICorporateUserCreatedEvent));

        SqlConnection _Con = null;
        SqlCommand _cmd = null;

        public async Task Handle(UserService.Messages.Events.ICorporateUserCreatedEvent message, IMessageHandlerContext context)
        {
            try
            {
                await Task.Run(() =>
                {
                    //log.InfoFormat(message.TestString);
                    _Con = new SqlConnection(@"Data Source=GUR00251545L\SQLEXPRESS;Initial Catalog=update_and_publish;User ID=sa;Password=Password@123");
                    //_cmd = new SqlCommand("INSERT INTO [update_and_publish].[dbo].[Orders] VALUES (5)", _Con);
                    _Con.Open();
                    //_cmd.ExecuteNonQuery();
                });
            }
            finally
            {
                if (_Con.State == ConnectionState.Open)
                    _Con.Close();
            }
        }
    }
}
