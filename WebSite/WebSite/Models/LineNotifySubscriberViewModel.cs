namespace WebSite.Models;

public class LineNotifySubscriberViewModel
{
    public LineNotifySubscriberViewModel()
    {
        this.HasSubscribed = false;
    }

    /// <summary>
    /// 已訂閱
    /// </summary>
    public bool HasSubscribed { get; set; }
}