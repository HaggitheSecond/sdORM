using System;
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
        public DatabaseSessionFactory Factory
        {
            get
            {
                var attributeEntityMappingProvider = new AttributeEntityMappingProvider();

                attributeEntityMappingProvider.AddMappingsFromAssembly(Assembly.GetCallingAssembly());

                //var connectionString = "Server=b5-312-pc02; Database=mydb; UID=3r1k-uk; password=user2;";
                var connectionString = "Server=localhost; Database=develop; UID=root; password=;";
                
                var factory = new MySqlDatabaseSessionFactory(connectionString, attributeEntityMappingProvider);

                factory.Initalize();

                return factory;
            }
        }

        [Fact]
        public void Test()
        {
            using (var session = this.Factory.CreateSession())
            {

                var employee = new Employee
                {
                    Birthday = DateTime.MinValue,
                    FirstName = "dunno",
                    LastName = "House",
                    Status = EmployeeStatus.Retired,
                    Salary = 42
                };
                var temp = session.Save(employee);

                var temp2 = session.Query<Employee>();
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
                var result = session.Query<Employee>(f => f.FirstName=="Jaina");

                Assert.True(result != null);
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

        [Fact]
        public void SaveAndDeleteTest()
        {
            using (var session = this.Factory.CreateSession())
            {
                var result1 = session.Save(new Employee
                {
                    FirstName = "does not matter"
                });

                Assert.True(result1.ID != 0);

                session.Delete<Employee>(result1.ID);
            }
        }

        [Fact]
        public async Task SaveAndDeleteAsyncTest()
        {
            using (var session = await this.Factory.CreateAsyncSession())
            {
                var result1 = await session.SaveAsync(new Employee
                {
                    FirstName = "does not matter",
                    
                });

                Assert.True(result1.ID != 0);

                await session.Delete<Employee>(result1.ID);
            }
        }
    }
}
