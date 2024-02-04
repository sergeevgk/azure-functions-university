using Refit;

namespace AzureFunctionsUniversity.Lesson2.Contract
{
	public interface IHttpBinOrgApi
	{
		[Get("/status/{code}")]
		Task<HttpContent> StatusCodes(int code);

		[Post("/post")]
		Task<PostRequestParameters> PostRequestParameters(Stream content = null, [Query] IDictionary<string, string> query = default);
	}
}