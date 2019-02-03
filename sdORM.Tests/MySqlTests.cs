using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Mapping;
using sdORM.Mapping.AttributeMapping;
using sdORM.Mapping.Exceptions;
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
        public void ConnectionTest()
        {
            using (var session = this._factory.CreateSession())
            {

            }
        }

        [Fact]
        public async Task ConnectionAsyncTest()
        {
            using (var session = await this._factory.CreateAsyncSession())
            {

            }
        }

        [Fact]
        public void MappingTest()
        {
            using (var session = this._factory.CreateSession())
            {
                var result = session.GetByID<Employee>(1);

                Assert.Throws<DBEntityNonValidTypeException>(() => session.GetByID<int>(2));
                Assert.Throws<NoDBEntityMappingException>(() => session.GetByID<object>(2));
                Assert.Throws<NoPrimaryKeyMappingForDBEntityException>(() => session.GetByID<EmployeeNoDBPrimaryKeyMapping>(2));
                Assert.Throws<DBPrimaryKeyNonSupportedTypeException>(() => session.GetByID<EmployeeWrongPrimaryKeyMapping>(2));
                Assert.Throws<NoDBPropertyMappingException>(() => session.GetByID<EmployeeNoMappingAttribute>(2));
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

        [Fact]
        public void GetByIdTest()
        {
            using (var session = this._factory.CreateSession())
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
            using (var session = await this._factory.CreateAsyncSession())
            {
                var result1 = await session.GetByIDAsync<Employee>(1);
                Assert.True(result1 != null);

                var result2 = await session.GetByIDAsync<Employee>(int.MaxValue);
                Assert.True(result2 == null);
            }
        }
    }
}
