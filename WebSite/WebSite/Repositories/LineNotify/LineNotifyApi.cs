using System.Text.Json;

namespace WebSite.Repositories.LineNotify;

using Flurl;
using Flurl.Http;

/// <summary>
/// The line notify api class
/// </summary>
public class LineNotifyApi : ILineNotifyApi
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LineNotifyApi"/> class
    /// </summary>
    public LineNotifyApi()
    {
    }

    /// <summary>
    /// Gets the oauth token using the specified parameter
    /// </summary>
    /// <param name="parameter">The parameter</param>
    /// <returns>The result</returns>
    public async Task<LineNotifyOauthToken> GetOauthTokenAsync(OauthTokenParameter parameter)
    {
        var values = new Dictionary<string, string>
        {
            { "grant_type", parameter.GrantType ?? "authorization_code" },
            { "code", parameter.Code },
            { "redirect_uri", parameter.RedirectUri },
            { "client_id", parameter.ClientId ?? "7u1Lu4cdIldcwOx9ueDBaJ" },
            { "client_secret", parameter.ClientSecret ?? "lF22d9LXSZXj0AdYNkpoSLuRFU3OaOo8lVS4qDnmgJg" }
        };

        var content = new FormUrlEncodedContent(values);
        const string url = "https://notify-bot.line.me/oauth/token";
        var response = await url
            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            .PostAsync(content);
        var result = await response.GetJsonAsync<LineNotifyOauthToken>();
        return result;
    }
    
    /// <summary>
    /// 撤銷 access token
    /// </summary>
    /// <param name="accessToken">要撤銷的 access token</param>
    /// <returns>A task containing the revoke result</returns>
    public async Task<RevokeResult> RevokeAsync(string accessToken)
    {
        const string url = "https://notify-api.line.me/api/revoke";
        return await url.WithOAuthBearerToken(accessToken).GetJsonAsync<RevokeResult>();
    }
}