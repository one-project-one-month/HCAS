using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Shared
{
    public class DapperService
    {
        private readonly string _connectionString;

        public DapperService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConnection")!;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? param = null)
        {         
            using IDbConnection db = new SqlConnection(_connectionString);
            return await db.QueryAsync<T>(query, param);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string query, object? param = null)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var item = await db.QueryFirstOrDefaultAsync<T>(query, param);
            return item!;
        }

        public async Task<int> ExecuteAsync(string query, object? param = null)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            return await db.ExecuteAsync(query, param);        
        }
    }
}
