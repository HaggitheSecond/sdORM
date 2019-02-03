using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Mapping;
using sdORM.Mapping.AttributeMapping;
using sdORM.MySql.Session;
using sdORM.Tests.Entities;
using Xunit;

namespace sdORM.Tests
{
    public class MySqlTests
    {
        private readonly DataBaseSessionFactory _factory = new MySqlDataBaseSessionFactory(new SdOrmConfig
        {
            //Server = "b5-312-pc02",
            //DataBase = "mydb",
            //Password = "3r1k-uk",
            //UserName = "user2"
            ConnectionString = "Server=localhost; database=develop; UID=root; password=;",
            Mappings = new EntityMappingProvider(typeof(AttributeEntityMapping<>))
        });

        [Fact]
        public void Test()
        {

        }

        [Fact]
        public void ConnectionTest()
        {
            using (var session = this._factory.CreateSession())
            {

            }
        }

        [Fact]
        public void QueryTest()
        {
            using (var session = this._factory.CreateSession())
            {
                var result = session.Query<Employee>(f => f.ID == 1);

                Assert.True(result != null);
                Assert.True(result.Count == 1);
            }
        }

        [Fact]
        public async Task QueryTestAsync()
        {
            using (var session = await this._factory.CreateAsyncSession())
            {
                var result = await session.QueryAsync<Employee>(f => f.ID == 1);

                Assert.True(result != null);
                Assert.True(result.Count == 1);
            }
        }
    }
}
