namespace WebSite.Repositories.LineNotify;

/// <summary>
/// The notify parameter class
/// </summary>

public class NotifyParameter
{
    /// <summary>
    /// Gets or sets the value of the message
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Gets or sets the value of the image thumbnail
    /// </summary>
    public string? ImageThumbnail { get; set; }
    /// <summary>
    /// Gets or sets the value of the image fullsize
    /// </summary>
    public string? ImageFullsize { get; set; }
    /// <summary>
    /// Gets or sets the value of the image file
    /// </summary>
    public string? ImageFile { get; set; }
    /// <summary>
    /// Gets or sets the value of the sticker package id
    /// </summary>
    public int? StickerPackageId { get; set; }
    /// <summary>
    /// Gets or sets the value of the sticker id
    /// </summary>
    public int? StickerId { get; set; }
    /// <summary>
    /// Gets or sets the value of the notification disabled
    /// </summary>
    public bool? NotificationDisabled { get; set; }
}