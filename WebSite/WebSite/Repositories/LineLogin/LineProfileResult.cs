using Newtonsoft.Json;

namespace WebSite.Repositories.LineLogin;

/// <summary>
/// The line profile result class
/// </summary>
public class LineProfileResult
{
    /// <summary>
    /// User ID
    /// </summary>
    [JsonProperty("userId")]
    public string UserId { get; set; }

    /// <summary>
    /// User's display name
    /// </summary>
    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    /// <summary>
    /// Profile image URL.
    /// This is an HTTPS URL.
    /// It's only included in the response if the user has set a profile image.
    /// </summary>
    [JsonProperty("pictureUrl")]
    public Uri PictureUrl { get; set; }

    /// <summary>
    /// User's status message.
    /// Not included in the response if the user doesn't have a status message.
    /// </summary>
    [JsonProperty("statusMessage")]
    public string StatusMessage { get; set; }
}