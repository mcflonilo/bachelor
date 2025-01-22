using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.CodeFirst;
using UltraBend.Databases.Project;

namespace UltraBend.Databases
{
    public class BaseDbInitializer<T> : SqliteCreateDatabaseIfNotExists<T> where T:DbContext
    {
        public BaseDbInitializer(DbModelBuilder modelBuilder, Action<T> seedAction)
            : base(modelBuilder)
        {
            
        }

        protected override void Seed(T context)
        {
            // Here you can seed your core data if you have any.
        }
    }
}
