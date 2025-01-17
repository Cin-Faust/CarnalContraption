using CarnalContraption.Application.Services;
using CarnalContraption.Application.Services.Lovense;
using CarnalContraption.Application.Services.PiShock;
using CarnalContraption.Application.Storage.PiShock;
using Microsoft.Extensions.DependencyInjection;

namespace CarnalContraption.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IHttpApiClient, HttpApiClient>();
        serviceCollection.AddSingleton<IUserRepository, UserRepository>();
        serviceCollection.AddSingleton<IGuildUserRepository, GuildUserRepository>();
        serviceCollection.AddSingleton<IClient, Client>();
        serviceCollection.AddSingleton<ILovenseService, LovenseService>();
        serviceCollection.AddMediatR(serviceConfiguration => serviceConfiguration.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
        return serviceCollection;
    }
}