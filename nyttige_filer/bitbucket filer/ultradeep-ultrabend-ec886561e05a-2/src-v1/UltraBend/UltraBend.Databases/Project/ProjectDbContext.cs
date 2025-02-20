using System.Data.Entity;
using System.Data.SQLite;

namespace UltraBend.Databases.Project
{
    public class ProjectDbContext : BaseDbContext<ProjectDbContext>
    {
        //public ProjectDbContext()
        //{
        //}

        public ProjectDbContext(string fileName, bool createDatabase = false)
            : base(new SQLiteConnection(new SQLiteConnectionStringBuilder
            {
                DataSource = fileName,
                ForeignKeys = true,
                BinaryGUID = false
            }.ConnectionString), true, createDatabase)
        {
        }

        public virtual DbSet<ProjectDetails> ProjectDetails { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<MaterialData> MaterialDatas { get; set; }
        public virtual DbSet<Design> Designs { get; set; }
        public virtual DbSet<DesignSection> DesignSections { get; set; }
        public virtual DbSet<LoadContour> LoadContours { get; set; }
        public virtual DbSet<Case> Cases {get; set; }
        public virtual DbSet<Study> Studies { get; set; }
        public virtual DbSet<Umbilical> Umbilicals { get; set; }
        public virtual DbSet<Result> Results { get; set; }

        protected override void ConfigureEntities(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectDetails>();

            modelBuilder.Entity<MaterialData>()
                .HasRequired(i => i.Material)
                .WithMany(ds => ds.MaterialData)
                .WillCascadeOnDelete(true);
            
            modelBuilder.Entity<Material>();
            
            modelBuilder.Entity<Design>();

            modelBuilder.Entity<DesignSection>()
                .HasOptional(i => i.Material);

            modelBuilder.Entity<Case>();

            modelBuilder.Entity<Study>()
                .HasMany(m => m.Cases);

            modelBuilder.Entity<Umbilical>()
                .HasOptional(i => i.Design)
                .WithOptionalPrincipal(ds => ds.Umbilical);

            modelBuilder.Entity<Result>();

            Seed = context => { };
        }

        public override int SaveChanges()
        {
            // whenever a service saves a change (to a temp file), has changes is true
            ApplicationState.ProjectHasChanges = true;

            return base.SaveChanges();
        }
    }
}