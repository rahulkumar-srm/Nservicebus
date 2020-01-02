using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Messages.Events
{
    public interface IUserCreatedEvent //: IEvent
    {
        Guid UserId { get; set; }
        string EncryptedEmailAddress { get; set; }
        string Name { get; set; }
    }
}
