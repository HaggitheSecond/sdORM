using System;
using sdORM.Mapping.AttributeMapping.Attributes.Entities;
using sdORM.Mapping.AttributeMapping.Attributes.Properties;

namespace sdORM.Tests.Entities
{
    // non gdpr conform

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
        public string Gender { get; set; }

        [DBProperty]
        public DateTime Birthday { get; set; }
    }

    [DBEntity]
    public class EmployeeNoDBPrimaryKeyMapping
    {
        [DBProperty]
        public string FirstName { get; set; }

        [DBProperty]
        public string LastName { get; set; }
    }

    [DBEntity]
    public class EmployeeWrongPrimaryKeyMapping
    {
        [DBPrimaryKey]
        public object ID { get; set; }

        [DBProperty]
        public string FirstName { get; set; }

        [DBProperty]
        public string LastName { get; set; }
    }

    [DBEntity]
    public class EmployeeNoMappingAttribute
    {
        [DBPrimaryKey]
        public int ID { get; set; }

        public string FirstName { get; set; }

        [DBProperty]
        public string LastName { get; set; }
    }
}