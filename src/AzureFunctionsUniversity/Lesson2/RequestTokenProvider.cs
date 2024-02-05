namespace AzureFunctionsUniversity.Lesson2.Contract;

public sealed class RequestTokenProvider : IRequestTokenProvider
{
	private readonly IAuthenticationClient _client;
	private readonly GetAccessTokenRequestCredentials _credentials;

	public RequestTokenProvider(IAuthenticationClient client, GetAccessTokenRequestCredentials credentials)
	{
		_client = client;
		_credentials = credentials;
	}

	public Task<GetAccessTokenResponse> GetAccessToken()
	{
		// this requests a token on every call
		// implement caching for better performance

		return _client.GetAccessToken(_credentials);
	}
}