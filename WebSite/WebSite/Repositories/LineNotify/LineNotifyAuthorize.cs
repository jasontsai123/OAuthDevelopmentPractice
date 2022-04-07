using System.Text.Json.Serialization;

namespace WebSite.Repositories.LineNotify;

public class LineNotifyAuthorize
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("state")]
    public string? State { get; set; }
}