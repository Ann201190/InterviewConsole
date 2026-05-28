using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EmployeeService
{
    public class Employee
    {
        public Employee()
        {
            Employees = new List<Employee>();
        }
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int? ManagerID { get; set; }

        [DataMember]
        public bool Enable { get; set; }

        [DataMember]
        public ICollection<Employee> Employees { get; set; }  
    }
}