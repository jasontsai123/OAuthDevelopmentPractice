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
}