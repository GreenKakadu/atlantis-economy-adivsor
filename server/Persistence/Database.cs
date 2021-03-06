namespace atlantis.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public class Database : DbContext {
        public Database(DbContextOptions<Database> options)
            : base(options) {
        }

        public DbSet<DbGame> Games { get; set; }
        public DbSet<DbTurn> Turns { get; set; }
        public DbSet<DbFaction> Factions { get; set; }
        public DbSet<DbEvent> Events { get; set; }
        public DbSet<DbRegion> Regions { get; set; }
        public DbSet<DbStructure> Structures { get; set; }
        public DbSet<DbUnit> Units { get; set; }

        protected override void OnModelCreating(ModelBuilder model) {
            model.Entity<DbGame>(t => {
            });

            model.Entity<DbTurn>(t => {
                t.HasOne(x => x.Game)
                    .WithMany(x => x.Turns)
                    .HasForeignKey(x => x.GameId)
                    .IsRequired();
            });

            model.Entity<DbFaction>(t => {
                t.HasOne(x => x.Game)
                    .WithMany()
                    .HasForeignKey(x => x.GameId)
                    .IsRequired();

                t.HasOne(x => x.Turn)
                    .WithMany(x => x.Factions)
                    .HasForeignKey(x => x.TurnId)
                    .IsRequired();
            });

            model.Entity<DbEvent>(t => {
                t.HasOne(x => x.Game)
                    .WithMany()
                    .HasForeignKey(x => x.GameId)
                    .IsRequired();

                t.HasOne(x => x.Turn)
                    .WithMany(x => x.Events)
                    .HasForeignKey(x => x.TurnId)
                    .IsRequired();

                t.HasOne(x => x.Faction)
                    .WithMany(x => x.Events)
                    .HasForeignKey(x => x.FactionId)
                    .IsRequired();
            });

            model.Entity<DbRegion>(t => {
                t.HasOne(x => x.Game)
                    .WithMany()
                    .HasForeignKey(x => x.GameId)
                    .IsRequired();

                t.HasOne(x => x.Turn)
                    .WithMany(x => x.Regions)
                    .HasForeignKey(x => x.TurnId)
                    .IsRequired();
            });

            model.Entity<DbStructure>(t => {
                t.HasOne(x => x.Game)
                    .WithMany()
                    .HasForeignKey(x => x.GameId)
                    .IsRequired();

                t.HasOne(x => x.Turn)
                    .WithMany(x => x.Structures)
                    .HasForeignKey(x => x.TurnId)
                    .IsRequired();

                t.HasOne(x => x.Region)
                    .WithMany(x => x.Structures)
                    .HasForeignKey(x => x.RegionId)
                    .IsRequired();
            });

            model.Entity<DbUnit>(t => {
                t.HasOne(x => x.Game)
                    .WithMany()
                    .HasForeignKey(x => x.GameId)
                    .IsRequired();

                t.HasOne(x => x.Turn)
                    .WithMany(x => x.Units)
                    .HasForeignKey(x => x.TurnId)
                    .IsRequired();

                t.HasOne(x => x.Region)
                    .WithMany(x => x.Units)
                    .HasForeignKey(x => x.RegionId)
                    .IsRequired();

                t.HasOne(x => x.Structure)
                    .WithMany(x => x.Units)
                    .HasForeignKey(x => x.StrcutureId);
            });
        }
    }
}
