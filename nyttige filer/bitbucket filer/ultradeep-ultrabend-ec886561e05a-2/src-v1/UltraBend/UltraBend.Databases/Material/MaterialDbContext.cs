using System.Data.Entity;
using System.Data.SQLite;

namespace UltraBend.Databases.Material
{
    public class MaterialDbContext : BaseDbContext<MaterialDbContext>
    {
        //public MaterialDbContext()
        //{
        //}

        public MaterialDbContext(string fileName, bool createDatabase = false)
            : base(new SQLiteConnection(new SQLiteConnectionStringBuilder
            {
                DataSource = fileName,
                ForeignKeys = true,
                BinaryGUID = false
            }.ConnectionString), true, createDatabase)
        {
        }

        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<MaterialData> MaterialDatas { get; set; }

        protected override void ConfigureEntities(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaterialData>()
                .HasRequired(i => i.Material)
                .WithMany(ds => ds.MaterialData)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Material>();

            Seed = context => { };
        }
    }
}