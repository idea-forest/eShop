using System.Globalization;
using Blazor.Analytics;
using Blazored.LocalStorage;

namespace Site.Client;

public static class ServiceExtensions 
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration) 
    {
        services.AddHttpClient("Site")
            .AddTypedClient<IItemsClient>((http, sp) => new ItemsClient(http));

        services.AddHttpClient("Site")
            .AddTypedClient<ICartsClient>((http, sp) => new CartsClient(http));

        services.AddHttpClient("Site")
            .AddTypedClient<ICheckoutClient>((http, sp) => new CheckoutClient(http));

        services.AddHttpClient("Site")
            .AddTypedClient<IUserClient>((http, sp) => new UserClient(http));
         
        CultureInfo? culture = new("sv-SE");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        services.AddGoogleAnalytics(configuration["GoogleAnalytics:TrackingId"]);

        services.AddBlazoredLocalStorage();

        return services;
    }
}