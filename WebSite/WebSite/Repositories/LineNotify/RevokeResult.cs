using Newtonsoft.Json;

namespace WebSite.Repositories.LineNotify;

/// <summary>
/// The revoke result class
/// </summary>
public class RevokeResult
{
    /// <summary>
    /// Value according to HTTP status code
    /// <value>
    /// <para>200: Success</para>
    /// <para>401: Invalid access token</para>
    /// <para>Other: Processed over time or stopped</para>
    /// </value>
    /// </summary>
    [JsonProperty("status")]
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the value of the message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }
}