namespace UnitTests
{
    using DataAccess;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var dbContext = Setup.ServiceProvider.GetService<SqLiteDatastoreContext>();
            var query = dbContext.Uses<BlogPostDto>("select * from BlogPostDto");
            Assert.AreEqual("select * from blog_Posts", query);
        }

        [Test]
        public void Test2()
        {
            var dbContext = Setup.ServiceProvider.GetService<MsSqlDatastoreContext>();
            var query = dbContext.Uses<BlogPostDto>("select * from BlogPostDto");
            Assert.AreEqual("select * from [blog].[Posts]", query);
        }
    }
}