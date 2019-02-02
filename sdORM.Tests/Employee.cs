using System;
using sdORM.Attributes;
using sdORM.Attributes.Entities;
using sdORM.Attributes.Properties;

namespace sdORM.Tests
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
}