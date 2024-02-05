using Refit;

namespace AzureFunctionsUniversity.Lesson2.Contract;

public interface IAuthenticationClient
{
	[Post("/oauth/token")]
	Task<GetAccessTokenResponse> GetAccessToken([Body(BodySerializationMethod.UrlEncoded)] GetAccessTokenRequestCredentials request);
}