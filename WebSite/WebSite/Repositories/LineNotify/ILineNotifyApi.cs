namespace WebSite.Repositories.LineNotify;

/// <summary>
/// The line notify api interface
/// </summary>
public interface ILineNotifyApi
{
    /// <summary>
    /// Gets the oauth token using the specified parameter
    /// </summary>
    /// <param name="parameter">The parameter</param>
    /// <returns>The result</returns>
    Task<LineNotifyOauthToken> GetOauthTokenAsync(OauthTokenParameter parameter);

    /// <summary>
    /// 撤銷 access token
    /// </summary>
    /// <param name="accessToken">要撤銷的 access token</param>
    /// <returns>A task containing the revoke result</returns>
    Task<RevokeResult> RevokeAsync(string accessToken);

    /// <summary>
    /// Sends the notify using the specified access token
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <param name="notifyParameter">The notify parameter</param>
    /// <returns>A task containing the notify result</returns>
    Task<NotifyResult> SendNotifyAsync(string accessToken, NotifyParameter notifyParameter);
}