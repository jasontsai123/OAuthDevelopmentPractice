using System.Text.Json;
using Flurl.Util;
using WebSite.Setting;

namespace WebSite.Repositories.LineNotify;

using Flurl;
using Flurl.Http;

/// <summary>
/// The line notify api class
/// </summary>
public class LineNotifyApi : ILineNotifyApi
{
    /// <summary>
    /// LineNotify設定
    /// </summary>
    private readonly LineNotifySetting _lineNotifySetting;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineNotifyApi"/> class
    /// </summary>
    public LineNotifyApi(LineNotifySetting lineNotifySetting)
    {
        _lineNotifySetting = lineNotifySetting;
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
            { "client_id", parameter.ClientId ?? _lineNotifySetting.ClientId },
            { "client_secret", parameter.ClientSecret ?? _lineNotifySetting.ClientSecret }
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
        var response = await url
            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            .WithOAuthBearerToken(accessToken)
            .PostAsync();
        return await response.GetJsonAsync<RevokeResult>();
    }


    /// <summary>
    /// Sends the notify using the specified access token
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <param name="notifyParameter">The notify parameter</param>
    /// <returns>A task containing the notify result</returns>
    public async Task<NotifyResult> SendNotifyAsync(string accessToken, NotifyParameter notifyParameter)
    {
        var values = new Dictionary<string, string>
        {
            { "message", notifyParameter.Message }
        };

        var content = new FormUrlEncodedContent(values);
        const string url = "https://notify-api.line.me/api/notify";
        var flurlResponse = await url
            .WithOAuthBearerToken(accessToken)
            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            .PostAsync(content);
        
        return await flurlResponse.GetJsonAsync<NotifyResult>();
    }
}