using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks_System_Library.Repositories
{
    public interface IDeliveryService
    {
        Task ProcessPendingEventsAsync();
        string GenerateSignature(string payload, string secret);
        DateTime CalculateNextRetry(int attemptCount);
    }
}
