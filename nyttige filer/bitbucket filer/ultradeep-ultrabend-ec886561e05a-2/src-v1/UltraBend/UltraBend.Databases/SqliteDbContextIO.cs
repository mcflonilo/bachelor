using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Databases
{
    public class SqliteDbContextIO
    {
        public static void Save(DbContext context, string fileName, bool overWrite)
        {
            var connection = context.Database.Connection as SQLiteConnection;

            if (connection == null)
                throw new Exception("Must be a SQLite database");

            if (File.Exists(fileName) && !overWrite)
                throw new Exception("File exists");

            if (File.Exists(fileName) && !overWrite)
                File.Delete(fileName);

            var directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var destination = new SQLiteConnection()
            {
                ConnectionString =
                    new SQLiteConnectionStringBuilder()
                    {
                        DataSource = fileName,
                        ForeignKeys = true,
                        BinaryGUID = false
                    }.ConnectionString
            })
            {
                destination.Open();
                connection.BackupDatabase(destination, "main", "main", -1, null, -1);
            }
        }

        public static SQLiteConnection BuildFileConnection(string fileName)
        {
            return
                new SQLiteConnection(new SQLiteConnectionStringBuilder()
                {
                    DataSource = fileName,
                    ForeignKeys = true,
                    BinaryGUID = false
                }.ConnectionString);
        }
        public static string BuildFileConnectionString(string fileName)
        {
            return
                new SQLiteConnectionStringBuilder()
                {
                    DataSource = fileName,
                    ForeignKeys = true,
                    BinaryGUID = false
                }.ConnectionString;
        }

        public static SQLiteConnection Load(string fileName)
        {
            if (!File.Exists(fileName))
                throw new Exception("File not found");

            using (var source = new SQLiteConnection()
            {
                ConnectionString =
                    new SQLiteConnectionStringBuilder()
                    {
                        DataSource = fileName,
                        ForeignKeys = true,
                        BinaryGUID = false
                    }.ConnectionString
            })
            {
                source.Open();

                var connection = new SQLiteConnection() { ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = ":memory:", ForeignKeys = true, BinaryGUID = false }.ConnectionString };

                connection.Open();

                source.BackupDatabase(connection, "main", "main", -1, null, -1);

                return connection;
            }
        }
    }
}
