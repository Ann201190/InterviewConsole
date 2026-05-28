using EmployeeService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace InterviewConsole
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            try
            {
                var employeeService = serviceProvider.GetRequiredService<IEmployeeService>();

                string tableResult = await employeeService.GetQueryResultAsTableAsync("SELECT ID, Name, ManagerID, Enable FROM Employee");
                Console.WriteLine(tableResult);
                Console.WriteLine();


                Console.Write("Enter employee ID: ");
                int id = int.Parse(Console.ReadLine());
                string treeResult = await employeeService.GetEmployeeTreeAsTextAsync(id);
                Console.WriteLine(treeResult);
                Console.WriteLine();


                Console.Write("Enter employee ID: ");
                int idEnable = int.Parse(Console.ReadLine());
                Console.Write("Enter Enable (1) or Disable (0): ");
                int enable = int.Parse(Console.ReadLine());
                await employeeService.EnableEmployeeAsync(idEnable, enable);
                Console.WriteLine();


                tableResult = await employeeService.GetQueryResultAsTableAsync("SELECT ID, Name, ManagerID, Enable FROM Employee");
                Console.WriteLine(tableResult);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while running the application: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IEmployeeService>(provider => new EmployeeService.EmployeeService());
        }
    }
}