using Microsoft.AspNetCore.Mvc;
using Webhooks_System_Library.Entities;
using Webhooks_System_Library.Repositories;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebHooks.Controllers
{
 
    [ApiController]  
    [Route("api")]
    public class WebhookApiController : ControllerBase
    {
        private readonly IWebHookRepo _webHookRepo;
        // GET: api/<WebhookApiController>
        //contructor to inject IWebHookRepository
        public WebhookApiController(IWebHookRepo webHookRepo)
        {   
            _webHookRepo = webHookRepo;
        }

        //Endpoint to register new endpoint
        [HttpPost("endpoints")]
        public async Task<IActionResult> RegisterEndpoint([FromBody] WebhookEp endpoint)
        {
            endpoint.IsActive = true;
            var endpointId = await _webHookRepo.CreateEndpointAsync(endpoint);
            return CreatedAtAction(nameof(RegisterEndpoint), new { id = endpointId.Id }, new {  
                endpoint.Id,
                endpoint.Name,
                endpoint.Url,
                endpoint.IsActive,
                endpoint.CreatedAt,
                endpoint.ApiKey
            });
        }

        //Endpoint to send event to an endpoint

        [HttpPost("events")]
        public async Task<IActionResult> SendEvent([FromBody] WebhookEvent evt)
        {
            var eventId = await _webHookRepo.CreateEventAsync(evt);
            return CreatedAtAction(nameof(SendEvent), new { id = eventId.Id }, new {
            EventId = eventId.Id,
            EndPointId = eventId.EventPointId,
            eventId.EventType,
            eventId.Payload,
            eventId.CreatedAt,
            eventId.Status,
            eventId.AttemptCount,
            eventId.NextAttemptAt,
            eventId.LastAttemptAt,
            eventId.LastResponsecode,
            eventId.LastErrorMessage,
            eventId.DeliveredAt
            });
        }

        //Endpoint to get event status
        [HttpGet("events/{id}/status")]
        public async Task<IActionResult> GetEventStatus(int id)
        {
            var evt = await _webHookRepo.GetEventByIdAsync(id);
            if (evt == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                evt.Id,
                Status = evt.Status.ToString(),
                evt.AttemptCount,
                evt.LastResponsecode,
                evt.LastAttemptAt,
                evt.DeliveredAt,
                evt.LastErrorMessage
            });
        }

        //endpoint to get dead letter queue :
        [HttpGet("deadletter")]
        public async Task<IActionResult> GetDeadLetterQueue()
        {
            var deadLetters = await _webHookRepo.GetDeadLetterEventsAsync();
            return Ok(deadLetters);
        }

        //toggle active endpoints
        [HttpPatch("/endpoints/{id}/toggle")]
        public async Task<IActionResult> ToggleEndpoints(int id)
        {
            var endpoints = await _webHookRepo.GetEndpointByIdAsync(id);
            if (endpoints == null)
            {
                return NotFound();
            }
            endpoints.IsActive = !endpoints.IsActive;
            await _webHookRepo.UpdateEndpointAsync(endpoints);
            return Ok(new {endpoints.Id, endpoints.IsActive});
        }
    }
}
