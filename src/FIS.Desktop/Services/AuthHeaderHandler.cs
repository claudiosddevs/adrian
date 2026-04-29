using System.Net.Http.Headers;

namespace FIS.Desktop.Services;

/// <summary>
/// DelegatingHandler que añade el JWT Bearer a cada request saliente.
/// </summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly TokenStore _tokens;

    public AuthHeaderHandler(TokenStore tokens) => _tokens = tokens;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(_tokens.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer", _tokens.AccessToken);

        return base.SendAsync(request, ct);
    }
}
