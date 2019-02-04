using System.Reflection;
using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Mapping.AttributeMapping;
using sdORM.MySql;
using sdORM.Tests.Entities;
using Xunit;

namespace sdORM.Tests
{
    public class MySqlTests
    {
        public DataBaseSessionFactory Factory
        {
            get
            {
                var attributeEntityMappingProvider = new AttributeEntityMappingProvider();

                attributeEntityMappingProvider.AddMappingsFromAssembly(Assembly.GetCallingAssembly());

                //var connectionString = "Server=b5-312-pc02; database=mydb; UID=3r1k-uk; password=user2;";
                var connectionString = "Server=localhost; database=develop; UID=root; password=;";
                
                var factory = new MySqlDataBaseSessionFactory(connectionString, attributeEntityMappingProvider);

                factory.Initalize();

                return factory;
            }
        }

        [Fact]
        public void ConnectionTest()
        {
            using (var session = this.Factory.CreateSession())
            {

            }
        }

        [Fact]
        public async Task ConnectionAsyncTest()
        {
            using (var session = await this.Factory.CreateAsyncSession())
            {

            }
        }

        [Fact]
        public void QueryTest()
        {
            using (var session = this.Factory.CreateSession())
            {
                var result = session.Query<Employee>(f => f.ID == 1);

                Assert.True(result != null);
                Assert.True(result.Count == 1);
            }
        }

        [Fact]
        public async Task QueryTestAsync()
        {
            using (var session = await this.Factory.CreateAsyncSession())
            {
                var result = await session.QueryAsync<Employee>(f => f.ID == 1);

                Assert.True(result != null);
                Assert.True(result.Count == 1);
            }
        }

        [Fact]
        public void GetByIdTest()
        {
            using (var session = this.Factory.CreateSession())
            {
                var result1 = session.GetByID<Employee>(1);

                Assert.True(result1 != null);

                var result2 = session.GetByID<Employee>(int.MaxValue);
                Assert.True(result2 == null);
            }
        }

        [Fact]
        public async Task GetByIdAsyncTest()
        {
            using (var session = await this.Factory.CreateAsyncSession())
            {
                var result1 = await session.GetByIDAsync<Employee>(1);
                Assert.True(result1 != null);

                var result2 = await session.GetByIDAsync<Employee>(int.MaxValue);
                Assert.True(result2 == null);
            }
        }
    }
}
