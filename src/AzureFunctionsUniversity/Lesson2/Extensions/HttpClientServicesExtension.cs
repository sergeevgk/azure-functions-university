using AzureFunctionsUniversity.Lesson2.Contract;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace AzureFunctionsUniversity.Lesson2.Extensions;


public static class HttpClientServicesExtension
{
	const string HTTPBINORGAPI_HOST = "http://httpbin.org";
	public static void AddRefitService(this IServiceCollection services)
	{
		services
			.AddHttpClient(nameof(IHttpBinOrgApiClient), ConfigureHttpClient)
			.AddTypedClient(c => RestService.For<IHttpBinOrgApiClient>(c))
			.AddHttpMessageHandler<AuthenticationHandler>()
			.AddHttpMessageHandler<MockedUnauthorizedHandler>();

		services.AddTransient<MockedUnauthorizedHandler>();
		services.AddTransient<AuthenticationHandler>();
		services.AddTransient<IRequestTokenProvider, RequestTokenProvider>();
		services.AddSingleton(
			new GetAccessTokenRequestCredentials()
			{
				ClientId = "please-include-client-id-here",
				ClientSecret = "please-retrieve-client-secret-from-application-settings",
				Resource = HTTPBINORGAPI_HOST
			}
		);
	}

	/// <summary>
	/// HttpBinOrgApi does not have authentication API, we just configure the HttpClient in the same way. 
	/// Mock handler is used to avoid requesting any real service.
	/// </summary>
	/// <param name="services"></param>
	public static void AddAuthenticationService(this IServiceCollection services)
	{
		services
			.AddHttpClient(nameof(IAuthenticationClient), ConfigureHttpClient)
			.AddTypedClient(c => RestService.For<IAuthenticationClient>(c))
			.AddHttpMessageHandler<MockedAuthenticationServerHandler>();

		services.AddTransient<MockedAuthenticationServerHandler>();
	}

	static void ConfigureHttpClient(IServiceProvider provider, HttpClient client)
	{
		client.BaseAddress = new Uri(HTTPBINORGAPI_HOST);
		client.DefaultRequestHeaders.Add("Accept", "application/json");
	}
}
