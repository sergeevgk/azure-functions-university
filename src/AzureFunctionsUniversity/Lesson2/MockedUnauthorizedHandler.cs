using System.Net;
using System.Net.Http.Headers;

namespace AzureFunctionsUniversity.Lesson2;

/// <summary>
/// Shortcuts the Http request pipeline in case there is no Authorization header or it has empty parameters.
/// </summary>
public sealed class MockedUnauthorizedHandler : DelegatingHandler
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var authorization = request.Headers?.Authorization ?? new AuthenticationHeaderValue("Bearer");
		if (string.IsNullOrWhiteSpace(authorization.Parameter))
		{
			var unauthorized = new HttpResponseMessage(HttpStatusCode.Unauthorized)
			{
				RequestMessage = new(),
			};

			return Task.FromResult(unauthorized);
		}

		return base.SendAsync(request, cancellationToken);
	}
}