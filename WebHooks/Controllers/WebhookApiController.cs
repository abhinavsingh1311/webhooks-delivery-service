using Microsoft.AspNetCore.Mvc;
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
    }
}
