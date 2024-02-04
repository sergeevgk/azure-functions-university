using AzureFunctionsUniversity.Lesson2.Contract;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace AzureFunctionsUniversity.Lesson2.Extensions;


public static class RefitServiceExtension
{
    const string HttpBinOrgApiHost = "http://httpbin.org";
    public static void AddRefitService(this IServiceCollection services)
    {
        services
            .AddHttpClient("HttpBinOrgApi", (provider, client) =>
            {
                client.BaseAddress = new Uri(HttpBinOrgApiHost);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddTypedClient(c => RestService.For<IHttpBinOrgApi>(c));
    }
}
