using System;
using System.Data;
using System.Data.SqlClient;
using iMvcCore.Extensions;
using Microsoft.Extensions.Options;

namespace iMvcCore.Database
{
    public class Database3 : IDisposable
    {
        public Database3(IOptions<DatabaseOptions> databaseOptions)
        {
            var options = databaseOptions.Value;
            if (options == null || string.IsNullOrEmpty(options.Connection3)) throw new ArgumentNullException(nameof(options.Connection3));

            Current = new SqlConnection(options.Connection3.Decrypt(options.Key));
            if (Current.State != ConnectionState.Open)
            {
                Current.Open();
            }
        }

        public IDbConnection Current { get; }
        public Guid GuidFlag { get; } = Guid.NewGuid();

        public void Dispose()
        {
            if (Current.State != ConnectionState.Closed)
            {
                Current.Close();
            }
        }
    }
}