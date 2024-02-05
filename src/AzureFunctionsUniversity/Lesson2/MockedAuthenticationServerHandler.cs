using System.Text;
using System.Text.Json;

namespace AzureFunctionsUniversity.Lesson2.Contract;

/// <summary>
/// This handler mocks the authentication request and shortcuts the pipeline. It provides just a dummy token.
/// </summary>
public sealed class MockedAuthenticationServerHandler : DelegatingHandler
{
	private const string DUMMY_TOKEN = "eyJhbGciOiJoczI1NiIsInR5cCI6ICJKV1QifQ.eyJzdWIiOiJtZSJ9.signature";

	/// <summary>
	/// This method does not actually send request because in this project just for educational purposes HttpBinOrgApi was used.
	/// It does not have authentication features so there is no difference in using a real API or providing dummy token.
	/// </summary>
	/// <param name="request">Request that is processed in Http request pipeline.</param>
	/// <param name="cancellationToken"></param>
	/// <returns>HttpResponseMessage with provided dummy AccessToken. Retunn shortcuts the pipeline and does not calls for base.SendAsync(request, cancellationToken).</returns>
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var tokenResponse = new GetAccessTokenResponse
		{
			AccessToken = DUMMY_TOKEN
		};

		var tokenResponseJson = JsonSerializer.Serialize(tokenResponse);

		var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
		response.Content = new StringContent(tokenResponseJson, Encoding.UTF8, "application/json");

		return Task.FromResult(response);

		// by *NOT* proceeding any further
		// we will short-circuit the pipeline
		// HTTP request will *NOT* be sent over the wire

		// return base.SendAsync(request, cancellationToken);
	}
}