using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Extensions;
using sdORM.Mapping.AttributeMapping;
using sdORM.MySql;
using sdORM.Session;
using sdORM.Tests.Entities;
using Xunit;

namespace sdORM.Tests
{
    public class MySqlTests
    {
        public AttributeEntityMappingProvider MappingProvider;

        public DatabaseSessionFactory Factory
        {
            get
            {
                this.MappingProvider = new AttributeEntityMappingProvider();

                this.MappingProvider.AddMappingsFromAssembly(Assembly.GetCallingAssembly());

                //var connectionString = "Server=b5-312-pc02; Database=mydb; UID=3r1k-uk; password=user2;";
                var connectionString = "Server=localhost; Database=develop; UID=root; password=;";

                var factory = new MySqlDatabaseSessionFactory(connectionString, this.MappingProvider);

                factory.Initialize();

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
            var sqlProvider = new MySqlSqlProvider();

            using (var session = this.Factory.CreateSession())
            {
                #region DateTime

                QueryTestInternal(session,
                    f => f.Birthday < new DateTime(2019, 02, 07, 23, 28, 00, 00),
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE Birthday < @Birthday",
                    1);

                var dateTime = new DateTime(2019, 02, 07, 23, 28, 00, 00);
                QueryTestInternal(session,
                    f => f.Birthday < dateTime,
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE Birthday < @Birthday",
                    1);

                QueryTestInternal(session,
                    f => f.Birthday < DateTime.Now,
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE Birthday < NOW()",
                    0);

                QueryTestInternal(session,
                    f => f.Birthday == DateTime.Today,
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE DATE(Birthday) = DATE(NOW())",
                    0);

                #endregion

                #region Lists

                var namesList = new List<string>
                {
                    "Jaina",
                    "Arthas",
                    "Garrosh"
                };
                QueryTestInternal(session,
                    f => namesList.Contains(f.FirstName),
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE FirstName IN (@FirstName, @FirstName1, @FirstName2)",
                    3);

                QueryTestInternal(session,
                    f => new List<string>
                    {
                        "Jaina",
                        "Arthas",
                        "Garrosh"
                    }.Contains(f.FirstName),
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE FirstName IN (@FirstName, @FirstName1, @FirstName2)",
                    3);

                #endregion

                QueryTestInternal(session,
                    f => f.FirstName == "Jaina",
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE FirstName = @FirstName",
                    1);

                QueryTestInternal(session,
                    f => f.FirstName == "Temp" && f.ID == 2 || f.LastName == "Proudmoore" && f.Salary != 0,
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE ((FirstName = @FirstName) AND (ID = @ID)) OR ((LastName = @LastName) AND (Salary <> @Salary))",
                    4);

                QueryTestInternal(session,
                    f => f.ID == 2,
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE ID = @ID",
                    1);

                var id = 3;
                QueryTestInternal(session,
                    f => f.ID == id,
                    "SELECT ID, FirstName, LastName, Gender, Birthday, Status, Salary FROM Employee WHERE ID = @ID",
                    1);
            }

            void QueryTestInternal(IDatabaseSession session, Expression<Func<Employee, bool>> expression, string expectedResultSql, int expectedResultParamaterCount)
            {
                var query = sqlProvider.GetSqlForPredicate(expression, this.MappingProvider.GetMapping<Employee>());
                var result1 = session.Query<Employee>(query);
                var replaced = query.GetWithReplacedParameters();

                Assert.True(query.Sql == expectedResultSql);
                Assert.True(query.Parameters.Count == expectedResultParamaterCount);
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
