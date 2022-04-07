using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace WebSite.Repositories.LineNotify;

public class LineNotifyOauthToken
{
    [JsonProperty("status")]
    public long Status { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
}