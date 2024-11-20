using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using Mod7Exer1.Model;
using System.Security.Cryptography;
using Org.BouncyCastle.Cms;

namespace Mod7Exer1.Services
{
    public class EmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }



        public async Task<List<Employee>> GetAllPersonalsAsync()
        {
            var employeeService = new List<Employee>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT * FROM tblemployee", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employeeService.Add(new Employee
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"),
                            Name = reader.GetString("Name"),
                            Address = reader.GetString("Address"),
                            email = reader.GetString("email"),
                            ContactNo = reader.GetString("ContactNo"),
                        });
                    }
                }
                return employeeService;
            }
        }
        public async Task<bool> AddEmployeeAsync(Employee newEmployee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("INSERT INTO tblemployee (Name, Address, email, ContactNo) VALUES (@Name, @Address, @email, @ContactNo)", conn);
                    cmd.Parameters.AddWithValue("@Name", newEmployee.Name);
                    cmd.Parameters.AddWithValue("@Address", newEmployee.Address);
                    cmd.Parameters.AddWithValue("@Email", newEmployee.email);
                    cmd.Parameters.AddWithValue("@ContactNo", newEmployee.ContactNo);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0; // Returns true if an employee was added
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding employee record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int EmployeeID)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("Delete FROM tblemployee WHERE EmployeeID = @EmployeeID", conn);
                    cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }catch (Exception ex)
            {
                Console.WriteLine($"Error deleting employee record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Employee updateEmployee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand(
                        "UPDATE tblemployee SET Name = @Name, Address = @Address, email = @Email, ContactNo = @ContactNo WHERE EmployeeID = @EmployeeID", conn);
                    cmd.Parameters.AddWithValue("@Name", updateEmployee.Name);
                    cmd.Parameters.AddWithValue("@Address", updateEmployee.Address);
                    cmd.Parameters.AddWithValue("@Email", updateEmployee.email);
                    cmd.Parameters.AddWithValue("@ContactNo", updateEmployee.ContactNo);
                    cmd.Parameters.AddWithValue("@EmployeeID", updateEmployee.EmployeeID);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error updating employee record: {ex.Message}");
                return false;
            }
        }
    }
}
