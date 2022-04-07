using Newtonsoft.Json;

namespace WebSite.Repositories.LineLogin;

/// <summary>
/// The line auth token result class
/// </summary>
public class LineOAuth2TokenResult
{
    /// <summary>
    /// Access token. Valid for 30 days.
    /// </summary>
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Number of seconds until the access token expires.
    /// </summary>
    [JsonProperty("expires_in")]
    public long ExpiresIn { get; set; }

    /// <summary>
    /// JSON Web Token (JWT) (opens new window)with information about the user.
    /// This property is returned only if you requested the openid scope.
    /// For more information about ID tokens, see Get profile information from ID tokens.
    /// </summary>
    [JsonProperty("id_token")]
    public string IdToken { get; set; }

    /// <summary>
    /// Token used to get a new access token (refresh token).
    /// Valid for 90 days after the access token is issued.
    /// </summary>
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Permissions granted to the access token.
    /// Note that the <c>email</c> scope isn't returned as a value of the <c>scope</c> property even if access to it has been granted.
    /// </summary>
    [JsonProperty("scope")]
    public string Scope { get; set; }

    /// <summary>
    /// Bearer
    /// </summary>
    [JsonProperty("token_type")]
    public string TokenType { get; set; }
}