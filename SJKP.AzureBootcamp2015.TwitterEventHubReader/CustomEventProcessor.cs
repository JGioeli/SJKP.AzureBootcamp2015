using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJKP.AzureBootcamp2015.TwitterEventHubReader
{
    public class CustomEventProcessor : IEventProcessor
    {
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("Closing connection");
            return Task.FromResult<object>(null);
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine("Opening connection");
            return Task.FromResult<object>(null);
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var message in messages)
            {
                Console.WriteLine(System.Text.Encoding.UTF8.GetString(message.GetBytes()));
            }
            return Task.FromResult<object>(null);
        }
    }
}
