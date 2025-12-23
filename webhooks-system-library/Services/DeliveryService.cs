using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webhooks_System_Library.Data;
using Webhooks_System_Library.Repositories;
using Webhooks_System_Library.Entities;
using System.Security.Cryptography;

namespace Webhooks_System_Library.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IWebHookRepo _webHookRepo;
        private readonly HttpClient _httpClient;
        private const int MaxRetries = 6;

        public DeliveryService(IWebHookRepo webHookRepo, HttpClient httpClient)
        {
            _webHookRepo= webHookRepo; ;
            _httpClient = httpClient;
        }

        //claim pending events from the database and process them
         
          public async Task ProcessPendingEventsAsync()
        {
            try
            {
                // Implementation for processing pending events
                var events = await _webHookRepo.ClaimPendingEventsAsync(10);
                foreach (var webhookEvent in events)
                {
                    // Process each event (e.g., send HTTP request to the webhook endpoint)
                    await ProcessEventAsync(webhookEvent);
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while processing pending events.", ex);
            }
        }

        private async Task ProcessEventAsync(WebhookEvent webhookEvent)
        {
            // Implementation for processing a single event
           var endpoint = await _webHookRepo.GetEndpointByIdAsync(webhookEvent.EventPointId);
              if (endpoint == null || !endpoint.IsActive)
              {
                // Mark event as failed if endpoint is not found or inactive
                webhookEvent.Status = WebHookStatus.Failed;
                webhookEvent.LastErrorMessage = "Endpoint not found or inactive.";
                await _webHookRepo.UpdateEventAsync(webhookEvent);
                return;
            }

            webhookEvent.AttemptCount++;
            webhookEvent.LastAttemptAt = DateTime.UtcNow;
            try
            {
                var signature = GenerateSignature(webhookEvent.Payload, endpoint.Secret);
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint.Url)
                {
                    Content = new StringContent(webhookEvent.Payload, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("X-Webhook-Signature", signature);
                request.Headers.Add("X-Webhook-Event", webhookEvent.EventType);
                var response = await _httpClient.SendAsync(request);
                webhookEvent.LastResponsecode = (int)response.StatusCode;

                if(response.IsSuccessStatusCode)
                {
                    webhookEvent.Status = WebHookStatus.Delivered;
                    webhookEvent.DeliveredAt = DateTime.UtcNow;
                    webhookEvent.NextAttemptAt = null;
                    webhookEvent.ProcessingLockUntil = null;
                }
                else
                {
                  HandleFailure(webhookEvent, $"HTTP {(int)response.StatusCode} - {response.ReasonPhrase}");
                }

            }
            catch(Exception ex)
            {
                webhookEvent.LastResponsecode = null;
                HandleFailure(webhookEvent, ex.Message);    
            }
            await _webHookRepo.UpdateEventAsync(webhookEvent);
        }

        private void HandleFailure(WebhookEvent webhookEvent, string errorMessage)
        {
            webhookEvent.LastErrorMessage = errorMessage.Length>500? errorMessage[..500] : errorMessage;

            webhookEvent.ProcessingLockUntil = null;

            if (webhookEvent.AttemptCount >= MaxRetries)
            {
                webhookEvent.Status = WebHookStatus.DeadLetter;
                webhookEvent.NextAttemptAt = null;
                webhookEvent.ProcessingLockUntil = null;
            }
            else
            {
                webhookEvent.Status = WebHookStatus.Pending;
                webhookEvent.NextAttemptAt = CalculateNextRetry(webhookEvent.AttemptCount);
                webhookEvent.ProcessingLockUntil = null;
            }
        }

        public string GenerateSignature(string payload, string secret)
        {
            // Implementation for generating signature
            using var hmac = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
            return $"sha256={Convert.ToHexString(hash).ToLower()}";
        }

        public DateTime CalculateNextRetry(int attemptCount)
        {
            // Implementation for calculating next retry time
            var delaySeconds = Math.Pow(2, attemptCount);
            var jitter = Random.Shared.NextDouble()*0.3*delaySeconds;
            return DateTime.UtcNow.AddMinutes(delaySeconds+jitter);
        }

    }
}
