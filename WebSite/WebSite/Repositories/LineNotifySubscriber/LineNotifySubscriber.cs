using System.ComponentModel.DataAnnotations;

namespace WebSite.Repositories.LineNotifySubscriber;

public class LineNotifySubscriber
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Line使用者id
    /// </summary>
    public string? LineUserId { get; set; }
    
    /// <summary>
    /// An access token for authentication.
    /// </summary>
    public string AccessToken{ get; set; }
}