using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using SQLite.CodeFirst;
using UltraBend.Databases.Project;

namespace UltraBend.Databases
{
    public class BaseDbContext<T> : DbContext where T : DbContext
    {
        protected string FileName { get; set; }
        private readonly bool CreateDatabase;
        protected static Dictionary<string,string> SchemaSqls = new Dictionary<string, string>();

        //public BaseDbContext()
        //    : base(
        //        new SQLiteConnection
        //        {
        //            ConnectionString =
        //                new SQLiteConnectionStringBuilder
        //                {
        //                    DataSource = ":memory:",
        //                    ForeignKeys = true,
        //                    BinaryGUID = false
        //                }.ConnectionString
        //        }, true)
        //{
        //    CreateDatabase = true;
        //    Configure();

        //    if (CreateDatabase)
        //    {
        //        Database.CreateIfNotExists();
        //        Database.ExecuteSqlCommand(SchemaSqls[GetType().Name]);
        //    }
        //}

        //public BaseDbContext(bool createDatabase = true)
        //    : base(
        //        new SQLiteConnection
        //        {
        //            ConnectionString =
        //                new SQLiteConnectionStringBuilder
        //                {
        //                    DataSource = ":memory:",
        //                    ForeignKeys = true,
        //                    BinaryGUID = false
        //                }.ConnectionString
        //        }, true)
        //{
        //    CreateDatabase = createDatabase;
        //    Configure();

        //    if (CreateDatabase)
        //    {
        //        Database.CreateIfNotExists();
        //        Database.ExecuteSqlCommand(SchemaSqls[GetType().Name]);
        //    }
        //}

        public BaseDbContext(DbConnection connection, bool contextOwnsConnection, bool createDatabase = true)
            : base(connection, contextOwnsConnection)
        {
            CreateDatabase = createDatabase;
            Configure();

            //var sqlGenerator = new SqliteSqlGenerator();
            //var test = sqlGenerator.Generate(modelBuilder.Build(Database.Connection).StoreModel);
            ;

            if (CreateDatabase)
            {
                Database.CreateIfNotExists();
                Database.ExecuteSqlCommand(SchemaSqls[GetType().Name]);
            }
        }

        public BaseDbContext(string fileName, bool contextOwnsConnection = true, bool createDatabase = true) : base(
            new SQLiteConnection(new SQLiteConnectionStringBuilder
            {
                DataSource = fileName,
                ForeignKeys = true,
                BinaryGUID = false
            }.ConnectionString), contextOwnsConnection)
        {
            CreateDatabase = createDatabase;
            Configure();

            if (CreateDatabase)
            {
                Database.CreateIfNotExists();
                Database.ExecuteSqlCommand(SchemaSqls[GetType().Name]);
            }
        }

        public Action<T> Seed { get; set; } = null;

        private void Configure()
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ConfigureEntities(modelBuilder);
            var initializer = new BaseDbInitializer<ProjectDbContext>(modelBuilder, null);
            Database.SetInitializer(initializer);

            var type = GetType();
            if (!SchemaSqls.ContainsKey(GetType().Name))
            {
                // build the sql to create the schema and manually execute it
                var sqlGenerator = new SqliteSqlGenerator();
                SchemaSqls[GetType().Name] = sqlGenerator.Generate(modelBuilder.Build(Database.Connection).StoreModel);
            }
        }

        protected virtual void ConfigureEntities(DbModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string destinationFileName, bool overWrite = false)
        {
            if (File.Exists(destinationFileName) && !overWrite)
                throw new Exception("File exists");

            if (File.Exists(destinationFileName) && !overWrite)
                File.Delete(destinationFileName);

            var directory = Path.GetDirectoryName(destinationFileName);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var source = new SQLiteConnection()
            {
                ConnectionString =
                    new SQLiteConnectionStringBuilder()
                    {
                        DataSource = FileName,
                        ForeignKeys = true,
                        BinaryGUID = false
                    }.ConnectionString
            })
            using (var destination = new SQLiteConnection()
            {
                ConnectionString =
                    new SQLiteConnectionStringBuilder()
                    {
                        DataSource = destinationFileName,
                        ForeignKeys = true,
                        BinaryGUID = false
                    }.ConnectionString
            })
            {
                destination.Open();
                source.BackupDatabase(destination, "main", "main", -1, null, -1);
            }
        }
    }
}