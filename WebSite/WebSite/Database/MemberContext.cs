using Microsoft.EntityFrameworkCore;
using WebSite.Repositories.LineNotifySubscriber;

namespace WebSite.Database;

public class MemberContext: DbContext
{
    public MemberContext(DbContextOptions<MemberContext> options) : base(options)
    {
    }
    
    public DbSet<LineNotifySubscriber> LineNotifySubscribers { get; set; }
}