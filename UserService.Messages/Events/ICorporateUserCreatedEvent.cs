using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Messages.Events
{
    public interface ICorporateUserCreatedEvent : IUserCreatedEvent
    {
        Guid CorporateUserId { get; set; }
    }
}
