using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using WebSite.Database;
using WebSite.Repositories.LineNotifySubscriber;

namespace WebSiteTest.Repositories.LineNotifySubscriber;

[TestClass]
public class LineNotifySubscriberRepositoryTest
{
    private SqliteConnection _connection = null!;
    private static DbContextOptions<MemberContext> _contextOptions;
    MemberContext CreateContext() => new MemberContext(_contextOptions);

    private ILineNotifySubscriberRepository GetSystemUnderTest()
    {
        var sut = new LineNotifySubscriberRepository(CreateContext());
        return sut;
    }

    [TestInitialize]
    public void TestInitialize()
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    [ClassInitialize]
    public static void TestClassInitialize(TestContext testContext)
    {
        //DataSource
        PrepareData();
    }

    private static void PrepareData()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        _contextOptions = new DbContextOptionsBuilder<MemberContext>()
            .UseSqlite(connection)
            .Options;
        // Create the schema and seed some data
        using var context = new MemberContext(_contextOptions);
        context?.Database?.EnsureDeleted();
        context?.Database?.EnsureCreated();

        context?.AddRange(
            new WebSite.Repositories.LineNotifySubscriber.LineNotifySubscriber
            {
                Id = 1,
                LineUserId = "LineUserId1",
                AccessToken = "AccessToken1"
            },
            new WebSite.Repositories.LineNotifySubscriber.LineNotifySubscriber
            {
                Id = 2,
                LineUserId = "LineUserId2",
                AccessToken = "AccessToken2"
            });
        context?.SaveChanges();
    }

    [ClassCleanup]
    public static void TestClassCleanup()
    {
        using var context = new MemberContext(_contextOptions);
        context?.Database.EnsureDeleted();
        context?.SaveChanges();
    }

    [TestMethod]
    public async Task GetAllAsyncTest_取得全部資料_筆數應為2筆()
    {
        //arrange
        var sut = GetSystemUnderTest();

        //act
        var actual = await sut.GetAllAsync();

        //assert
        actual.Count().Should().Be(2);
    }

    [TestMethod]
    public async Task InsertAsyncTest_新增一筆資料_應回傳影響筆數1筆()
    {
        //arrange
        var sut = GetSystemUnderTest();

        //act
        var entity = new WebSite.Repositories.LineNotifySubscriber.LineNotifySubscriber
        {
            LineUserId = null,
            AccessToken = "ZpG6Tcn1X44Kfxxy5duLT7fertSz3UdveyicFLnSvJn"
        };
        var actual = await sut.InsertAsync(entity);

        //assert
        actual.Should().Be(1);
    }
    
    [TestMethod]
    public async Task DeleteByAccessTokenAsync_刪除一筆資料_應回傳影響筆數1筆()
    {
        //arrange
        var sut = GetSystemUnderTest();

        //act
        var accessToken = "AccessToken2";
        var actual = await sut.DeleteByAccessTokenAsync(accessToken);

        //assert
        actual.Should().Be(1);
    }
    
    [TestMethod]
    public async Task DeleteAllAsync_刪除全部資料_應回傳影響筆數2筆()
    {
        //arrange
        var sut = GetSystemUnderTest();

        //act
        var actual = await sut.DeleteAllAsync();

        //assert
        actual.Should().Be(2);
    }
}