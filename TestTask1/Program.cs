using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Data;
using Npgsql;

namespace TestTask1
{
    public class DatabaseConnectionParams
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string DatabaseName { get; set; }
        public string Port { get; set; }
        public string Password { get; set; }

        public override string ToString() => $"Server={HostName};Username={UserName};Database={DatabaseName};Port={Port};Password={Password};SSLMode=Prefer";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding(1251);

            //Show program help
            if (args.Contains("--help"))
            {
                Console.WriteLine(getProgramHelp());
                return;
            }

            //Check config file path in args
            int dbccfpArgIndex = Array.IndexOf(args, "--dbccfp");
            if (dbccfpArgIndex < 0 || dbccfpArgIndex == args.Length - 1)
            {
                Console.WriteLine("No action");
                return;
            }

            //Check config file exists
            string configFileName = args[dbccfpArgIndex + 1];
            if (!File.Exists(configFileName))
            {
                Console.WriteLine(String.Format("Config file \"{0}\" not exists!\nNo action.", configFileName));
                return;
            }

            //Read config file into databaseConnectionParams
            string configText = File.ReadAllText(configFileName);
            DatabaseConnectionParams databaseConnectionParams;
            try
            {
                databaseConnectionParams = JsonSerializer.Deserialize<DatabaseConnectionParams>(configText);
            }
            catch (System.Text.Json.JsonException)
            {
                Console.WriteLine(String.Format("Config file \"{0}\" is invalid!\nNo action.", configFileName));
                return;
            }

            //Connect to database
            NpgsqlConnection databaseConnection = new NpgsqlConnection(databaseConnectionParams.ToString());
            try
            {
                databaseConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Connect to database with config file \"{0}\" finished with errors:\n{1}\n\nNo action.",
                    configFileName,
                    e.ToString()));
                return;
            }
            if (databaseConnection == null || databaseConnection.State != ConnectionState.Open)
            {
                Console.WriteLine(String.Format("Can not connect to database with config file \"{0}\"!\nNo action.", configFileName));
                return;
            }
            else
            {
                Console.WriteLine(String.Format("Connection to \"{0}\" database established.\n", databaseConnectionParams.DatabaseName));
            }

            //Test task implementation
            Dictionary<string, string> commandsInfo = new Dictionary<string, string>() {
                {
                    "1", @"SELECT employee_salary_sum_view.employee_salary_sum FROM test_task_schema.department AS department 
                           RIGHT JOIN (SELECT department_id, SUM(salary) AS employee_salary_sum FROM test_task_schema.employee
                           GROUP BY employee.department_id) AS employee_salary_sum_view 
                           ON (employee_salary_sum_view.department_id = department.id)"
                },
                {
                    "2", @"SELECT employee_salary_sum_view.employee_salary_sum 
                           FROM test_task_schema.department AS department
                           RIGHT JOIN (SELECT employee.department_id, SUM(salary) AS employee_salary_sum FROM test_task_schema.employee AS employee
                           WHERE NOT EXISTS(SELECT * FROM test_task_schema.employee AS chiefs_view WHERE chiefs_view.chief_id = employee.id)
                           GROUP BY employee.department_id) AS employee_salary_sum_view 
                           ON (employee_salary_sum_view.department_id = department.id)"
                },
                {
                    "3", @"SELECT department.id FROM test_task_schema.department
                           LEFT JOIN test_task_schema.employee ON (employee.department_id = department.id)
                           ORDER BY employee.salary DESC LIMIT 1"
                },
                {
                    "4", @"SELECT employee.salary
                           FROM test_task_schema.employee AS employee
                           WHERE employee.id IN (SELECT DISTINCT chiefs_view.chief_id FROM test_task_schema.employee AS chiefs_view)
                           ORDER BY employee.salary DESC"
                }
            };
            while (true)
            {
                Console.WriteLine(getCommandHelp() + "\n");
                Console.Write("Command: ");
                string command = Convert.ToString(Console.ReadLine());

                //Exit
                if (command == "0")
                    break;

                //Wrong command
                int commandInfoIndex = commandsInfo.Keys.ToList().IndexOf(command);
                if (commandInfoIndex < 0)
                {
                    Console.Out.WriteLine("\nWrong command! Try again.\n\n");
                    continue;
                }

                Console.WriteLine("\nResult:");

                //Execute sql query
                var sqlCommand = new NpgsqlCommand(commandsInfo.ElementAt(commandInfoIndex).Value, databaseConnection);
                NpgsqlDataReader reader;
                try
                {
                    reader = sqlCommand.ExecuteReader();
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Command finished with errors:\n{0}\n", e.ToString()));
                    continue;
                }
                if (!reader.HasRows)
                {
                    Console.WriteLine("Rows count = 0.\n");
                    continue;
                }

                //Output sql query result
                switch (command)
                {
                    case "1":
                        Console.WriteLine(String.Format("Total salary by department with chiefs (salary):"));
                        break;
                    case "2":
                        Console.WriteLine(String.Format("Total salary by department without chiefs (salary):"));
                        break;
                    case "3":
                        Console.WriteLine(String.Format("Department with max employee's salary (department id):"));
                        break;
                    case "4":
                        Console.WriteLine(String.Format("Department chiefs salaries in descending order (salary):"));
                        break;
                }
                while (reader.Read())
                    Console.WriteLine(String.Format("({0})", reader.GetInt32(0).ToString()));

                Console.WriteLine("\n");
                reader.Close();
            }

            //Close connection to database
            databaseConnection.Close();
        }

        static string getProgramHelp()
        {
            //Example
            DatabaseConnectionParams databaseConnectionParams = new DatabaseConnectionParams();
            databaseConnectionParams.HostName = "127.0.0.1";
            databaseConnectionParams.UserName = "postgres";
            databaseConnectionParams.DatabaseName = "directum_test_task_db";
            databaseConnectionParams.Port = "5432";
            databaseConnectionParams.Password = "12345678";

            string[] helpParts = { "It's test task implementation.\n",
                "--dbccfp \"file_path\"\t JSON file with config for connect to PostgreSQL database\n",
                String.Format("JSON file example:\n{0}", JsonSerializer.Serialize<DatabaseConnectionParams>(databaseConnectionParams,  new JsonSerializerOptions () { WriteIndented = true }))
        };
            return string.Join("\n",
                               helpParts);
        }

        static string getCommandHelp()
        {
            string[] helpParts = { "Enter command number:",
                "1. Total salary by department with chiefs",
                "2. Total salary by department without chiefs",
                "3. Department with max employee's salary",
                "4. Department chiefs salaries in descending order",
                "0. Exit"
            };
            return string.Join("\n",
                               helpParts);
        }
    }
}
