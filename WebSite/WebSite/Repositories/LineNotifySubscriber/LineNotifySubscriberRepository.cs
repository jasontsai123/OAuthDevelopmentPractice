using Microsoft.EntityFrameworkCore;
using WebSite.Database;

namespace WebSite.Repositories.LineNotifySubscriber;

/// <summary>
/// The line notify subscriber repository class
/// </summary>
public class LineNotifySubscriberRepository : ILineNotifySubscriberRepository
{
    /// <summary>
    /// The member context
    /// </summary>
    private readonly MemberContext _memberContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineNotifySubscriberRepository"/> class
    /// </summary>
    /// <param name="memberContext">The member context</param>
    public LineNotifySubscriberRepository(MemberContext memberContext)
    {
        _memberContext = memberContext;
    }

    /// <summary>
    /// Gets the all
    /// </summary>
    /// <returns>A task containing an enumerable of line notify subscriber</returns>
    public async Task<IEnumerable<LineNotifySubscriber>> GetAllAsync()
    {
        return await _memberContext.LineNotifySubscribers.ToListAsync();
    }

    /// <summary>
    /// Inserts the line notify subscriber
    /// </summary>
    /// <param name="lineNotifySubscriber">The line notify subscriber</param>
    /// <returns>A task containing the int</returns>
    public async Task<int> InsertAsync(LineNotifySubscriber lineNotifySubscriber)
    {
        await _memberContext.LineNotifySubscribers.AddAsync(lineNotifySubscriber);
        return await _memberContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes the by access token using the specified access token
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <returns>A task containing the int</returns>
    public async Task<int> DeleteByAccessTokenAsync(string accessToken)
    {
        var lineNotifySubscriber = _memberContext.LineNotifySubscribers.Where(x => x.AccessToken == accessToken);
        _memberContext.LineNotifySubscribers.RemoveRange(lineNotifySubscriber);
        return await _memberContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Deletes the all
    /// </summary>
    /// <returns>A task containing the int</returns>
    public async Task<int> DeleteAllAsync()
    {
        _memberContext.LineNotifySubscribers.RemoveRange(_memberContext?.LineNotifySubscribers);
        return await _memberContext.SaveChangesAsync();
    }
}