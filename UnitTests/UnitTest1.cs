namespace UnitTests
{
    using Dapper;
    //using Dapper.Contrib;
    using DataAccess;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using System;
    using System.Data;
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
            var results = await dbContext.QueryAsync<CommentDto>("select * from blog_UnmoderatedComments").ConfigureAwait(false);
            Assert.IsFalse(results.Any());
        }

        [Test]
        public async Task Test3()
        {
            var dbContext = Setup.ServiceProvider.GetService<SqliteConnection>();
            var results = await dbContext.QueryAsync<ModeratedCommentDto>("select * from blog_V_Comments").ConfigureAwait(false);
            Assert.IsFalse(results.Any());
        }

        [Test]
        public async Task Test4()
        {
            var dbContext = Setup.ServiceProvider.GetService<SqliteConnection>();
            var time = DateTimeOffset.UtcNow;
            var blogPost = new BlogPostDto
            {
                Title = "SqLite",
                DeprecatedDate = time
            };
            var (sql, parameters) = blogPost.CreateInsertSqlStatement(ConnectionType.SqLite);
            await dbContext.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            var results = await dbContext.QueryAsync<BlogPostDto>("select Id, Title, DeprecatedDate from blog_Posts").ConfigureAwait(false);

            var comment = new CommentDto
            {
                BlogPostId = 2,
                Comment = "Hello world!"
            };
            (sql, parameters) = comment.CreateInsertSqlStatement(ConnectionType.SqLite);
            await dbContext.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            Assert.AreEqual(1, results.First().Id);
            Assert.AreEqual("SqLite", results.First().Title);
            Assert.AreEqual(time, results.First().DeprecatedDate);
        }
    }
}