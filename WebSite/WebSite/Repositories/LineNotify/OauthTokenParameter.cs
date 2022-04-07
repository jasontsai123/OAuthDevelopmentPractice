using System.Text.Json.Serialization;

namespace WebSite.Repositories.LineNotify;

public class OauthTokenParameter
{
    [JsonPropertyName("grant_type")]
    public string? GrantType { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; set; }

    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; set; }
}