using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webhooks_System_Library.Entities;

namespace Webhooks_System_Library.Repositories
{
    public interface IWebHookRepo
    {
        //Endpoints
        Task<List<WebhookEp>> GetAllEndpointsAsync();
        Task<WebhookEp> GetEndpointByIdAsync(int id);
        Task<WebhookEp> CreateEndpointAsync(WebhookEp endpoint);

        Task UpdateEndpointAsync(WebhookEp endpoint);
        Task DeleteEndpointAsync(int id);
        
        //Events
        Task<WebhookEvent?> GetEventByIdAsync(int id);
        Task<WebhookEvent> CreateEventAsync(WebhookEvent webhookEvent);
        Task UpdateEventAsync(WebhookEvent webhookEvent);
        Task<List<WebhookEvent>> ClaimPendingEventsAsync(int maxCount);
        Task<List<WebhookEvent>> GetDeadLetterEventsAsync();

    }

    
}
