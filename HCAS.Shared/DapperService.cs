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
            _connectionString = configuration.GetConnectionString("DbConnection");
        }

        public List<T> Query<T>(string query, object? param = null)
        {
            //Console.WriteLine("Query is: " + query);
            using IDbConnection db = new SqlConnection(_connectionString);
            var lst = db.Query<T>(query, param).ToList();
            return lst;
        }              

        public T QueryFirstOrDefault<T>(string query, object? param = null)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var item = db.QueryFirstOrDefault<T>(query, param);
            return item;
        }

        public int Execute(string query, object? param = null)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var result = db.Execute(query, param);
            return result;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? param = null)
        {         
            using IDbConnection db = new SqlConnection(_connectionString);
            return await db.QueryAsync<T>(query, param);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string query, object? param = null)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            return await db.QueryFirstOrDefaultAsync<T>(query, param);
        }

        public async Task<int> ExecuteAsync(string query, object? param = null)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            return await db.ExecuteAsync(query, param);        
        }
    }
}
