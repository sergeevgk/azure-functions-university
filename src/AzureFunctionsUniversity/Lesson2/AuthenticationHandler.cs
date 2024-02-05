using System.Net.Http.Headers;
namespace AzureFunctionsUniversity.Lesson2.Contract;

public sealed class AuthenticationHandler : DelegatingHandler
{
	private readonly IRequestTokenProvider _requestTokenProvider;

	public AuthenticationHandler(IRequestTokenProvider requestToken)
	{
		_requestTokenProvider = requestToken;
	}

	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken
	)
	{
		var tokenResponse = await _requestTokenProvider.GetAccessToken();
		if (tokenResponse != null)
		{
			var accessToken = tokenResponse.AccessToken;
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		}

		return await base.SendAsync(request, cancellationToken);
	}
}