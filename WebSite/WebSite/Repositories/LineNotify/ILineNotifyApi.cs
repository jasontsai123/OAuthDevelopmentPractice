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
}