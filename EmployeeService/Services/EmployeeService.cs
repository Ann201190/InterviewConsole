using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class EmployeeService : IEmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"]?.ConnectionString;

            if (string.IsNullOrEmpty(_connectionString))
            {
                //TODO: logging service
                throw new Exception("Connection string is empty");
            }
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            DataTable allEmployees = await GetQueryResultAsync("SELECT ID, Name, ManagerID, Enable FROM Employee");

            var dict = new Dictionary<int, Employee>();

            foreach (DataRow row in allEmployees.Rows)
            {
                var emp = new Employee
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Name = row["Name"].ToString(),
                    ManagerID = row["ManagerID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ManagerID"]),
                    Enable = Convert.ToBoolean(row["Enable"])
                };
                dict[emp.ID] = emp;
            }

            foreach (var emp in dict.Values)
            {
                if (emp.ManagerID.HasValue && dict.ContainsKey(emp.ManagerID.Value))
                {
                    dict[emp.ManagerID.Value].Employees.Add(emp);
                }
            }

            return dict.ContainsKey(id) ? dict[id] : null;
        }

        public async Task<string> GetEmployeeTreeAsTextAsync(int id)
        {
            Employee rootEmployee = await GetEmployeeByIdAsync(id);

            if (rootEmployee == null)
            {
                return $"Employee with ID = {id} not found.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"--- Hierarchy for an employee: {rootEmployee.Name} (ID: {rootEmployee.ID}) ---");

            BuildTreeText(rootEmployee, sb, "");

            return sb.ToString();
        }

        private void BuildTreeText(Employee employee, StringBuilder sb, string indent)
        {

            string status = employee.Enable ? "Enable" : "Disable";

            sb.AppendLine($"{indent}└─ [{employee.ID}] {employee.Name} ({status})");

            foreach (var subEmployee in employee.Employees)
            {
                BuildTreeText(subEmployee, sb, indent + "    ");
            }
        }

        public async Task EnableEmployeeAsync(int id, int enable)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE Employee SET Enable = @Enable WHERE ID = @ID";
                        command.Parameters.Add("@Enable", SqlDbType.Bit).Value = enable != 0;
                        command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: logging service
            }

        }

        public async Task <string> GetQueryResultAsTableAsync(string query)
        {
            DataTable dt = await GetQueryResultAsync(query);
            var hierarchy = GetQueryResultHierarchy(dt);

            return hierarchy;
        }

        private string GetQueryResultHierarchy(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return "The query did not return any data or the table is empty.";
            }

            var sb = new StringBuilder();

            foreach (DataColumn column in dt.Columns)
            {
                sb.Append($"{column.ColumnName}\t");
            }
            sb.AppendLine();
            sb.AppendLine(new string('-', 60));

            foreach (DataRow row in dt.Rows)
            {
                foreach (var cell in row.ItemArray)
                {
                    string value = cell == DBNull.Value ? "NULL" : cell.ToString();
                    sb.Append($"{value}\t");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private async Task <DataTable> GetQueryResultAsync(string query)
        {
            var dt = new DataTable();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: logging service
            }
            
            return dt;
        }
    }
}
