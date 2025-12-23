using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Webhooks_System_Library.Data;
using Webhooks_System_Library.Services;

namespace Webhooks_System_Library
{      // This is an extension method that extends the IServiceCollection interface.
       // It is typically used in ASP.NET Core applications to configure and register services.

    public static class WebhookExtensions
    {

        // The method name can be anything, but it must match the name used when calling it in
        // your Program.cs file using builder.Services.XXXX(options => ...).
        public static void AddBackendDependencies(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            // Register the context class, which is the DbContext for your application,
            // with the service collection. This allows the DbContext to be injected into other
            // parts of your application as a dependency.

            // The 'options' parameter is an Action<DbContextOptionsBuilder> that typically
            // configures the options for the DbContext, including specifying the database
            // connection string.
            services.AddDbContext<WebhooksDeliveryContext>(options);

            //  Register the DeliveryService class as a transient service.

            //  adding any services that you create in the class library (BLL)
            //  using .AddTransient<t>(...
            services.AddTransient<DeliveryService>((ServiceProvider) =>
            {
                //  Retrieve an instance of context from the service provider.
                var context = ServiceProvider.GetService<WebhooksDeliveryContext>();

                // Create a new instance of deliveryservice,
                //   passing the  instance as a parameter.
                return new DeliveryService(context);
            });

        }
    }
}
