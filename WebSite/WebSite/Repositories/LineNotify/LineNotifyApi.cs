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
        var url = "https://notify-bot.line.me/oauth/token";
        var response = await url
            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            .PostAsync(content);
        var result = await response.GetJsonAsync<LineNotifyOauthToken>();
        return result;
    }
}