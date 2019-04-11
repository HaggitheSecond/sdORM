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

        [DBProperty]
        public EmployeeStatus Status { get; set; }

        [DBProperty]
        public int Salary { get; set; }
    }

    public class WAT
    {
        public string ActualThingy { get; set; }
    }

    public enum EmployeeStatus
    {
        Active,
        Retired,
        Fired
    }
}