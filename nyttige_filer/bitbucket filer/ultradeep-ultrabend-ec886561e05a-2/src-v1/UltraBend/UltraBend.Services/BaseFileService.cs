using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Dependencies;
using UltraBend.Common;
using UltraBend.Databases;

namespace UltraBend.Services
{
    public abstract class BaseFileService<T>
        where T : DbContext
    {

        protected string FileName { get; set; }



        public static event EventHandler RepositoryUpdated;

        public static void OnRepositoryUpdated(BaseFileService<T> source)
        {
            RepositoryUpdated?.Invoke(source, EventArgs.Empty);
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
                source.Open();
                destination.Open();
                source.BackupDatabase(destination, "main", "main", -1, null, -1);
            }
        }
    }
    public abstract class BaseFileService<T, TModel> : BaseFileService<T>
        where T: DbContext
        where TModel: ICacheDependency, IModelId, new()
    {


        public static void Invalidate(Guid id)
        {
            CachingServices.Invalidation.Invalidate(new TModel {Id = id});
        }



    }
}
