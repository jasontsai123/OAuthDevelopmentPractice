namespace WebSite.Repositories.LineNotifySubscriber;

public interface ILineNotifySubscriberRepository
{
    /// <summary>
    /// Gets the all
    /// </summary>
    /// <returns>A task containing an enumerable of line notify subscriber</returns>
    Task<IEnumerable<LineNotifySubscriber>> GetAllAsync();

    /// <summary>
    /// Inserts the line notify subscriber
    /// </summary>
    /// <param name="lineNotifySubscriber">The line notify subscriber</param>
    /// <returns>A task containing the int</returns>
    Task<int> InsertAsync(LineNotifySubscriber lineNotifySubscriber);

    /// <summary>
    /// Deletes the by access token using the specified access token
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <returns>A task containing the int</returns>
    Task<int> DeleteByAccessTokenAsync(string accessToken);

    /// <summary>
    /// Deletes the all
    /// </summary>
    /// <returns>A task containing the int</returns>
    Task<int> DeleteAllAsync();
}