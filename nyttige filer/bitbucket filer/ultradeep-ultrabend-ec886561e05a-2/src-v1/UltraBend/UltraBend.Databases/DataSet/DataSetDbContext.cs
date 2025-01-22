using System.Data.Entity;
using System.Data.SQLite;

namespace UltraBend.Databases.DataSet
{
    public class DataSetDbContext : BaseDbContext<DataSetDbContext>
    {
        //public DataSetDbContext()
        //{
        //}

        public DataSetDbContext(string fileName, bool createDatabase = false)
            : base(new SQLiteConnection(new SQLiteConnectionStringBuilder
            {
                DataSource = fileName,
                ForeignKeys = true,
                BinaryGUID = false
            }.ConnectionString), true, createDatabase)
        {
        }

        public virtual DbSet<DataSet> DataSets { get; set; }
        public virtual DbSet<DataItem> DataItems { get; set; }

        protected override void ConfigureEntities(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataItem>()
                .HasRequired(i => i.DataSet)
                .WithMany(ds => ds.DataItems)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DataSet>();

            Seed = context => { };
        }
    }
}