using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJKP.AzureBootcamp2015.TwitterEventHubReader
{
    public class CustomCheckpointManager : ICheckpointManager
    {
        public Task CheckpointAsync(Lease lease, string offset, long sequenceNumber)
        {
            throw new NotImplementedException();
        }
    }
}
