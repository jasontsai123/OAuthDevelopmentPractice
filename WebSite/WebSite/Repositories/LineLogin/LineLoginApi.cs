using Flurl.Http;
using WebSite.Setting;

namespace WebSite.Repositories.LineLogin;

/// <summary>
/// The line login api class
/// </summary>
public class LineLoginApi : ILineLoginApi
{
    /// <summary>
    /// The line login setting
    /// </summary>
    private readonly LineLoginSetting _lineLoginSetting;
    /// <summary>
    /// Initializes a new instance of the <see cref="LineLoginApi"/> class
    /// </summary>
    /// <param name="lineLoginSetting">The line login setting</param>
    public LineLoginApi(LineLoginSetting lineLoginSetting)
    {
        _lineLoginSetting = lineLoginSetting;
    }

    /// <summary>
    /// Gets a user's ID, display name, profile image, and status message.
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <returns>A task containing the line profile result</returns>
    public async Task<LineProfileResult> GetProfileAsync(string accessToken)
    {
        return await @"https://api.line.me/v2/profile"
            .WithOAuthBearerToken(accessToken)
            .GetJsonAsync<LineProfileResult>();
    }
}