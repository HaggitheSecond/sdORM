using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Interfaces;
using sdORM.MySql;
using Xunit;

namespace sdORM.Tests
{
    public class UnitTest1
    {
        private DataBaseConfig _config => new DataBaseConfig
        {
            //Server = "b5-312-pc02",
            //DataBase = "mydb",
            //Password = "3r1k-uk",
            //UserName = "user2"
            Server = "localhost",
            DataBase = "develop",
            Password = "",
            UserName = "root"
        };

        [Fact]
        public void Test()
        {

        }

        [Fact]
        public void ConnectionTest()
        {
            using (var session = new MySqlDataBaseSession(this._config))
            {
                session.Connect();

                Assert.True(session.IsConnected);
            }
        }

        [Fact]
        public void QueryTest()
        {
            using (IDataBaseSession session = new MySqlDataBaseSession(this._config))
            {
                session.Connect();

                var result = session.Query<Employee>(f => f.ID == 1);

                Assert.True(result != null);
                Assert.True(result.Count == 1);
            }
        }

        [Fact]
        public async Task QueryTestAsync()
        {
            using (IDataBaseSession session = new MySqlDataBaseSession(this._config))
            {
                await session.ConnectAsync();

                var result = await session.QueryAsync<Employee>(f => f.ID == 1);

                Assert.True(result != null);
                Assert.True(result.Count == 1);
            }
        }

        [Fact]
        public void PreRegisterTest()
        {
            using (IDataBaseSession session = new MySqlDataBaseSession(this._config))
            {
                session.Connect();

                session.EntityMappingProvider.PreGenerateEntityMappings(new List<Type>()
                {
                    typeof(Employee)
                });
            }
        }

        [Fact]
        public async Task PreRegisterTestAsync()
        {
            using (IDataBaseSession session = new MySqlDataBaseSession(this._config))
            {
                await session.ConnectAsync();

                await session.EntityMappingProvider.PreGenerateEntityMappingsAsync(new List<Type>()
                {
                    typeof(Employee)
                });
            }
        }
    }
}
