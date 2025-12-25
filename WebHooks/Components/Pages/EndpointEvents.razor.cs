using Microsoft.AspNetCore.Components;
using Webhooks_System_Library.Entities;
using Webhooks_System_Library.Repositories;

namespace WebHooks.Components.Pages
{
    public partial class EndpointEvents
    {
        #region parameters
        [Parameter]
        public int endpointId { get; set; }

        private List<string> errors = [];

        private WebhookEp? endpoint;

        private List<WebhookEvent> events = [];

        private string feedbackMessage = string.Empty;

        private WebHookStatus? selectedFilter = null;

        private WebhookEvent newEvent = new();


        [Inject]
        IWebHookRepo WebHookRepo { get; set; } = default!;

        #endregion

        #region methods

        protected override async Task OnInitializedAsync()
        {
            try
            {
                endpoint = await WebHookRepo.GetEndpointByIdAsync(endpointId);
                await LoadEvents();
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
        }


        private async Task LoadEvents()
        {
            var allEvents = await WebHookRepo.GetEventsByEndpointIdAsync(endpointId);

            if (selectedFilter.HasValue)
            {
                events = allEvents.Where(e => e.Status == selectedFilter.Value).ToList();
            }
            else
            {
                events = allEvents;
            }
        }


        private async Task FilterChanged(WebHookStatus? status)
        {
            try
            {
                selectedFilter = status;
                await LoadEvents();
            }
            catch (Exception ex)
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
                newEvent.EventPointId = endpointId;
                await WebHookRepo.CreateEventAsync(newEvent);
                ResetForm();
                await LoadEvents();
                feedbackMessage = "Event created successfully!";
            }
            catch (Exception ex)
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
        #endregion
    }
}
