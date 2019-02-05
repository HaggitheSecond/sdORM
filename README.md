# sdORM - super duper object relational mapper

sdORM is a small, very basic ORM. It's still in developement and in its early stages.

**It only supports MySql at the moment!**

## Usage 

#### Settings up your factory
Creating the mapping provider (in this case a AttributeEntityMappingProvider):
```c#
var provider = new AttributeEntityMappingProvider();
```
Adding and validating all types you'll require to it:
```c#
provider.AddMappingsFromAssembly("yourAssembly");
```
or
```c#
provider.AddMappings(new Type[] { typeof("yourType"), .. });
```
Creating your factory (in this case a MySqlDatabaseSessionFactory):
```c#
var factory = new MySqlDatabaseSessionFactory("theMySqlConnectionString", provider);
```
and initializing the factory:
```c#
factory.Initalize();
```
or the async version:
```c#
await factory.InitalizeAsync();
```

After this your factory is set up, all entities are validated and you can start using CreateSession() and CreateAsyncSession().

### Examples

##### SaveOrUpdate
```c#
using (var session = this.Factory.CreateSession())
{
     var savedEntity = session.SaveOrUpdate("yourEntity"); 
}

using (var session = await this.Factory.CreateAsyncSession())
{
     var savedEntity = await session.SaveOrUpdateAsync("yourEntity"); 
}
```

##### A simple Query

```c#
using (var session = this.Factory.CreateSession())
{
     var result = session.Query<Employee>(f => f.FirstName=="Jaina");
}
 
using (var session = await this.Factory.CreateAsyncSession())
{
     var result = await session.QueryAsync<Employee>(f => f.LastName=="Proudmoore");
}
```

##### Entity with attribute mapping

```c#
[DBEntity]
public class Employee
{
     [DBPrimaryKey]
     public int ID { get; set; }

     [DBProperty]
     public string FirstName { get; set; }

     [DBProperty]
     public string LastName { get; set; }

     [DBProperty]
     public DateTime Birthday { get; set; }

     [DBProperty]
     public EmployeeStatus Status { get; set; }

     [DBIgnore]
     public int Salary { get; set; }
}
```
