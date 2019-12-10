using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace iMvcCore.Database
{
    public class Database : IDisposable
    {
        public Database(IOptions<DatabaseOptions> databaseOptions)
        {
            var options = databaseOptions.Value;
            if(options == null || string.IsNullOrEmpty(options.DefaultConnection)) throw new ArgumentNullException(nameof(options.DefaultConnection));

            Current = new SqlConnection(options.DefaultConnection);
            if(Current.State != ConnectionState.Open)
            {
                Current.Open();
            }
        }

        public IDbConnection Current { get; }
        public Guid GuidFlag { get; } = Guid.NewGuid();

        public void Dispose()
        {
            if(Current.State != ConnectionState.Closed)
            {
                Current.Close();
            }
        }
    }
}