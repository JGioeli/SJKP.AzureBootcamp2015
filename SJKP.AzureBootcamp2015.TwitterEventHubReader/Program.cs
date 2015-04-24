using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJKP.AzureBootcamp2015.TwitterEventHubReader
{
    class Program
    {
        private const string EventHubName = "TweetHub";

        static void Main(string[] args)
        {
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"]);
            EventHubClient eventHubReceiveClient = EventHubClient.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"], EventHubName);
            var consumerGroup = eventHubReceiveClient.GetDefaultConsumerGroup();
            EventHubDescription eventHub = namespaceManager.GetEventHub(EventHubName);

            // Register event processor with each shard to start consuming messages 
            foreach (var partitionId in eventHub.PartitionIds)
            {
                consumerGroup.RegisterProcessor<CustomEventProcessor>(new Lease()
                {
                    PartitionId = partitionId
                }, new CustomCheckpointManager());
            }

            // Wait for the user to exit this application. 
            Console.WriteLine("\nPress ENTER to exit...\n");
            Console.ReadLine();
        }
    }
}
