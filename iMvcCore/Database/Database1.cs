using System;
using System.Data;
using System.Data.SqlClient;
using iMvcCore.Extensions;
using Microsoft.Extensions.Options;

namespace iMvcCore.Database
{
    public class Database1 : IDisposable
    {
        public Database1(IOptions<DatabaseOptions> databaseOptions)
        {
            var options = databaseOptions.Value;
            if (options == null || string.IsNullOrEmpty(options.Connection1)) throw new ArgumentNullException(nameof(options.Connection1));

            Current = new SqlConnection(options.Connection1.Decrypt(options.Key));
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