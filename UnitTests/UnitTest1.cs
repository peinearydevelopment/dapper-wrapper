namespace UnitTests
{
    using Dapper;
    using DataAccess;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    public class Tests
    {
        [Test]
        public async Task Test1()
        {
            var dbContext = Setup.ServiceProvider.GetService<SqliteConnection>();
            var results = await dbContext.QueryAsync<BlogPostDto>("select * from blog_Posts").ConfigureAwait(false);
            Assert.IsFalse(results.Any());
        }

        [Test]
        public async Task Test2()
        {
            var dbContext = Setup.ServiceProvider.GetService<SqliteConnection>();
            var results = await dbContext.QueryAsync<BlogPostDto>("select * from blog_UnmoderatedComments").ConfigureAwait(false);
            Assert.IsFalse(results.Any());
        }

        [Test]
        public async Task Test3()
        {
            var dbContext = Setup.ServiceProvider.GetService<SqliteConnection>();
            var results = await dbContext.QueryAsync<BlogPostDto>("select * from blog_V_Comments").ConfigureAwait(false);
            Assert.IsFalse(results.Any());
        }
    }
}