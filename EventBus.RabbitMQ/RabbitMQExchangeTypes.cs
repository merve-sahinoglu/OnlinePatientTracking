using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public enum RabbitMQExchangeTypes
    {
        Direct,
        Default,
        Topic,
        Fanout,
        Headers,
        DeadLetter
    }
}
