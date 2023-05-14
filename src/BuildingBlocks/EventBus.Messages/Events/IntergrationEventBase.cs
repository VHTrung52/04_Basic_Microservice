using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public class IntergrationEventBase
    {
        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }

        public IntergrationEventBase()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public IntergrationEventBase(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }
    }
}
