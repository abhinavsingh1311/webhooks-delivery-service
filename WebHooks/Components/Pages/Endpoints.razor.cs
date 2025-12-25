using Microsoft.AspNetCore.Components;
using Webhooks_System_Library.Entities;
using Webhooks_System_Library.Repositories;
using Webhooks_System_Library.Services;

namespace WebHooks.Components.Pages
{

    public partial class Endpoints
    {
        #region Properties
        [Inject]
        public IWebHookRepo WebHookService { get; set; } = default!;

        private List<WebhookEp> endpoints = new();

        private WebhookEp  newEndpoint = new();

        [Inject]
        NavigationManager navigation {  get; set; }
      
        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            await LoadEndpoints();
        }

        private async Task LoadEndpoints()
        {
            endpoints = await WebHookService.GetAllEndpointsAsync();
        }
        private async Task AddEndpoint()
        {
            await WebHookService.CreateEndpointAsync(newEndpoint);
            newEndpoint = new WebhookEp(); // Reset the form
            await LoadEndpoints(); // Refresh the list
        }

        private async Task DeleteEndpoint(int id)
        {
            await WebHookService.DeleteEndpointAsync(id);
            await LoadEndpoints();
        }

        private async Task ToggleEndpoints(int id)
        {
            var endpoint = await WebHookService.GetEndpointByIdAsync(id);
            endpoint.IsActive = !endpoint.IsActive;
            await WebHookService.UpdateEndpointAsync(endpoint);
            await LoadEndpoints();
        }

        private void Navigate(int id)
        {
           navigation.NavigateTo($"/endpoints/{id}/events");
        }

        #endregion


    }
}
