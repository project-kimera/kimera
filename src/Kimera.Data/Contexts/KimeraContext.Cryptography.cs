using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Data.Contexts
{
    /*
     * Original Code
     * https://github.com/nitor-infotech-oss/sqlite-encryption-using-efcore/blob/master/BloggingContext.cs
     * modified by Kimera Contributors
     */
    public partial class KimeraContext
    {
        private SqliteConnection _connection;

        public SqliteConnection Connection
        {
            get => _connection;
        }

        private string _databaseFilePath = string.Empty;

        private string _password = null;

        public KimeraContext(string databaseFilePath, string password = null)
        {
            _databaseFilePath = databaseFilePath;

            _password = password;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _connection = InitializeConnection();

            optionsBuilder.UseSqlite(_connection);

            base.OnConfiguring(optionsBuilder);
        }

        // SQLCipher Encryption is applied to database using DBBrowser for SQLite.
        // DBBrowser for SQLite is free and open source tool to edit the SQLite files. 
        private SqliteConnection InitializeConnection()
        {
            try
            {
                SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder();
                builder.DataSource = _databaseFilePath;

                SqliteConnection connection = new SqliteConnection(builder.ConnectionString);
                connection.Open();

                if (_password != null)
                {
                    // Get a quoted password.
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT quote($password);";
                    command.Parameters.AddWithValue("$password", _password);

                    var quotedPassword = (string)command.ExecuteScalar();

                    // Input the quoted password.
                    command.CommandText = "PRAGMA key = " + quotedPassword;
                    command.Parameters.Clear();
                    command.ExecuteNonQuery();

                    // Reset variables.
                    quotedPassword = null;
                }

                return connection;
            }
            finally
            {
                _password = null;
            }
        }

        public bool EncryptDatabase(ref string password,ref string path)
        {
            try
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"KimeraBackup {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fff")}.tmp");

                if (File.Exists(_databaseFilePath))
                {
                    SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder();
                    builder.DataSource = _databaseFilePath;

                    using (SqliteConnection connection = new SqliteConnection(builder.ConnectionString))
                    {
                        connection.Open();

                        var query = @$"ATTACH DATABASE '{tempFilePath}' AS encrypted KEY '{password}'; SELECT sqlcipher_export('encrypted'); DETACH DATABASE encrypted;";
                        using var cmd = new SqliteCommand(query, connection);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                password = null;
            }
        }

        public void DecryptDatabase(ref string password)
        {
            try
            {
                using var keyCommand = _connection.CreateCommand();
                keyCommand.CommandText = $"PRAGMA key = {password};";
                keyCommand.ExecuteNonQuery();

                using var rekeyCommand = _connection.CreateCommand();
                rekeyCommand.CommandText = "PRAGMA rekey = '';";
                rekeyCommand.ExecuteNonQuery();
            }
            finally
            {
                password = null;
            }
        }

        public void ChangePassword(ref string password)
        {
            try
            {
                // Get a quoted password.
                var command = _connection.CreateCommand();
                command.CommandText = "SELECT quote($password);";
                command.Parameters.AddWithValue("$password", password);

                var quotedPassword = (string)command.ExecuteScalar();

                // Input the quoted password.
                command.CommandText = "PRAGMA rekey = " + quotedPassword;
                command.Parameters.Clear();
                command.ExecuteNonQuery();

                // Reset variables.
                quotedPassword = null;
            }
            finally
            {
                password = null;
            }
        }
    }
}
