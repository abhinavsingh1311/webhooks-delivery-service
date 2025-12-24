using Microsoft.AspNetCore.Components;
using Webhooks_System_Library.Entities;
using Webhooks_System_Library.Repositories;

namespace WebHooks.Components.Pages
{
    public partial class Events
    {


        #region variables

        private WebHookStatus? selectedFilter = null;

        private List<WebhookEvent> events = [];

        private List<WebhookEp> endpoints = [];

        private WebhookEvent newEvent = new();

        private string feedbackMessage = string.Empty;

        private List<string> errors = [];

        #endregion

        [Inject]
        IWebHookRepo WebHookRepo { get; set; } = default!;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadEvents();
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while initializing the Events component.", ex);
               
            }

        }

        private async Task LoadEvents()
        {
            // Implementation for loading events
            try
            {
                if (selectedFilter is null)
                {
                    events = await WebHookRepo.GetAllEventsAsync();
                }
                //  convert selectedFilter to WebHookStatus enum
                else
                {
                    events = await WebHookRepo.GetEventsByStatusAsync(selectedFilter.Value);
                }

                endpoints = await WebHookRepo.GetAllEndpointsAsync();

                feedbackMessage = "Events loaded successfully.";

            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while loading events.", ex);
            }

        }

        private async Task FilterChanged(WebHookStatus? status)
        {
            try {
                selectedFilter = status;
                await LoadEvents();
            }
            catch(Exception ex)
            {
                errors.Add(ex.Message);
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while changing the filter.", ex);
            }
        }

        // submit handler function

        private async Task SubmitHandler()
        {
            try
            {
                await WebHookRepo.CreateEventAsync(newEvent);
                ResetForm();
                await LoadEvents();
                feedbackMessage = "Event created successfully!";
            }
            catch(Exception ex)
            {           
                errors.Add(ex.Message);
                // Handle exception (e.g., log it)
                throw new Exception("An error occurred while submitting the new event.", ex);
            }

          
        }

        private void ResetForm()
        {
         newEvent = new(); 
        }

    }
}
