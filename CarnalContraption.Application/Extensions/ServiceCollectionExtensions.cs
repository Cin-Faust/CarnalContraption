using CarnalContraption.Application.Services;
using CarnalContraption.Application.Services.Lovense;
using CarnalContraption.Application.Services.PiShock;
using Microsoft.Extensions.DependencyInjection;

namespace CarnalContraption.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(serviceConfiguration => serviceConfiguration.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
        serviceCollection.AddSingleton<IHttpApiClient, HttpApiClient>();
        serviceCollection.AddSingleton<IPiShockService, PiShockService>();
        serviceCollection.AddSingleton<ILovenseService, LovenseService>();
        return serviceCollection;
    }
}