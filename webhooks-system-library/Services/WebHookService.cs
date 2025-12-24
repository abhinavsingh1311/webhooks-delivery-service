using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using Webhooks_System_Library.Data;
using Webhooks_System_Library.Entities;
using Webhooks_System_Library.Repositories;

namespace Webhooks_System_Library.Services
{
    public class WebHookService(WebhooksDeliveryContext context) : IWebHookRepo
    {
        private readonly WebhooksDeliveryContext _context = context;

        //Endpoints
        //Task<List<WebhookEp>> GetAllEndpointsAsync();
        public async Task<List<WebhookEp>> GetAllEndpointsAsync()
        {
            return await _context.WebhookEps.ToListAsync();
        }
        //Task<WebhookEp> GetEndpointByIdAsync(int id);

        public async Task<WebhookEp?> GetEndpointByIdAsync(int id)
        {
            return await _context.WebhookEps.FindAsync(id);
        }
        //Task<WebhookEp> CreateEndpointAsync(WebhookEp endpoint);

        public async Task<WebhookEp> CreateEndpointAsync(WebhookEp endpoint)
        {
            try
            {
                _context.WebhookEps.Add(endpoint);
                await _context.SaveChangesAsync();
                return endpoint;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while creating the endpoint.", ex);
            }
        }

        //Task UpdateEndpointAsync(WebhookEp endpoint);

        public async Task UpdateEndpointAsync(WebhookEp endpoint)
        {
            try
            {
                _context.WebhookEps.Update(endpoint);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while updating the endpoint.", ex);
            }
        }
       
        //Task DeleteEndpointAsync(int id);

        public async Task DeleteEndpointAsync(int id)
        {
            try
            {
                if(await GetEndpointByIdAsync(id) == null)
                {
                    throw new Exception("Endpoint not found.");
                }
                _context.WebhookEps.Remove( await GetEndpointByIdAsync(id));
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while deleting the endpoint.", ex);
            }
        }


        ////Events

        //Task<List<WebhookEvent>> GetAllEventsAsync();
        public async Task<List<WebhookEvent>> GetAllEventsAsync()
        {
            return await _context.WebhookEvents.Include(e => e.EventPoint).OrderByDescending(e=> e.CreatedAt).ToListAsync();
        }

        //Task<List<WebhookEvent>> GetEventsByStatusAsync(WebHookStatus status);
        public async Task<List<WebhookEvent>> GetEventsByStatusAsync(WebHookStatus status)
        {
            Console.WriteLine($"Filtering by status: {status} (value: {(byte)status})");

            var results = await _context.WebhookEvents
                .Include(e => e.EventPoint)
                .Where(e => e.Status == status)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            Console.WriteLine($"Found {results.Count} events");
            return results;
        }

        //Task<WebhookEvent?> GetEventByIdAsync(int id);

        public async Task<WebhookEvent?> GetEventByIdAsync(int id)
        {
            return await _context.WebhookEvents.Include(e => e.EventPoint).FirstOrDefaultAsync(e => e.Id == id);
        }

        //Task<WebhookEvent> CreateEventAsync(WebhookEvent webhookEvent);

        public async Task<WebhookEvent> CreateEventAsync(WebhookEvent webhookEvent)
        {
            try
            {
                _context.WebhookEvents.Add(webhookEvent);
                await _context.SaveChangesAsync();
                return webhookEvent;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while creating the webhook event.", ex);
            }
        }

        //Task UpdateEventAsync(WebhookEvent webhookEvent);
           
        public async Task UpdateEventAsync(WebhookEvent webhookEvent)
        {
            try
            {
                _context.WebhookEvents.Update(webhookEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while updating the webhook event.", ex);
            }
        }

        //Task<List<WebhookEvent>> ClaimPendingEventsAsync(int maxCount);

        public async Task<List<WebhookEvent>> ClaimPendingEventsAsync(int maxCount)
        {
            try
            {
                var now = DateTime.UtcNow;
                var lockUntil = now.AddMinutes(5); // Lock for 5 minutes

                //raw SQL to claim events
                var events = await _context.WebhookEvents
                                            .FromSqlRaw(@"
                        Update Top({0}) WebhookEvents WITH (ROWLOCK, READPAST)
                        SET ProcessingLockUntil = {1}
                        OUTPUT INSERTED.*
                        Where Status = 0 
                        And (NextAttemptAt is null or NextAttemptAt<= {2})
                        And(ProcessingLockUntil is null or ProcessingLockUntil < {2})",
                        maxCount, lockUntil, now)
                    .AsNoTracking()
                    .ToListAsync();

                return events;

            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while claiming pending webhook events.", ex);
            }
        }

        //Task<List<WebhookEvent>> GetDeadLetterEventsAsync();

        public async Task<List<WebhookEvent>> GetDeadLetterEventsAsync()
        {
            try
            {
                var deadletters  = await _context.WebhookEvents.Include(e=> e.EventPoint)
                    .Where(e => e.Status == WebHookStatus.DeadLetter)
                    .OrderBy(e => e.CreatedAt)
                    .ToListAsync();
                return deadletters;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while retrieving dead letter webhook events.", ex);
            }
        }


    }
}
