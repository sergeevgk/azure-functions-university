namespace AzureFunctionsUniversity.Lesson2.Contract;

public interface IRequestTokenProvider
{
	Task<GetAccessTokenResponse> GetAccessToken();
}