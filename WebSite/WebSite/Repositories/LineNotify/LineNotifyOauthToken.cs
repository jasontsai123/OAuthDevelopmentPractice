using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace WebSite.Repositories.LineNotify;

public class LineNotifyOauthToken
{
    [JsonPropertyName("status")]
    [JsonProperty("status")]
    public long Status { get; set; }

    [JsonPropertyName("message")]
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonPropertyName("access_token")]
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
}