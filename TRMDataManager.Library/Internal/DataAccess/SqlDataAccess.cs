using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManager.Library.Internal.DataAccess
{
    // This class is only usable in this library project. This is accessible through TRMDataManager.Library.DataAccess layer
    internal class SqlDataAccess : IDisposable
    {
        // ConnectionString is in the TRMDataManager/Web.config
        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            { 
                List<T> rows = connection.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }
        
        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            { 
                connection.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);                
            }
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        // Creates a connection and then a transaction
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            _connection = new SqlConnection(connectionString);
            _connection.Open();

            _transaction = _connection.BeginTransaction();
        }

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            List<T> rows = _connection.Query<T>(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;            
        }

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure, transaction: _transaction);          
        }        

        // Called if the transaction succeeded. Can be called multiple times because of the ? operators.
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection.Close();
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection.Close();
        }

        public void Dispose()
        {
            CommitTransaction();
        }
        // Open connect/start transaction method
        // Load using the transaction
        // Save using the transaction
        // Close connection/stop transaction method
        // Dispose
    }
}
