namespace WebSite.Models;

/// <summary>
/// The send notify message class
/// </summary>
public class SendNotifyMessage
{
    /// <summary>
    /// 標題
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 內容
    /// </summary>
    public string? Content { get; set; }
}