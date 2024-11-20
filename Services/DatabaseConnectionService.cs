using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod7Exer1.Services
{
    public class DatabaseConnectionService
    {
            private readonly string _connectionString;

            public DatabaseConnectionService()
            {
                _connectionString = "Server=localhost;Database=companydb;User ID=mod7;Password=mod7";
            }

            public string GetConnectionString()
            {
                return
                        _connectionString;
            }

        
    }
}
