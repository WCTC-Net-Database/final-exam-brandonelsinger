using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Equipments;
using ConsoleRpgEntities.Models.Rooms;
using Microsoft.EntityFrameworkCore;

namespace ConsoleRpgEntities.Data
{
    /// <summary>
    /// Entity Framework Core database context for the RPG game.
    /// Configures all entity relationships, TPH inheritance, and cascade behaviors.
    /// </summary>
    public class GameContext : DbContext
    {
        // ===== DbSets - Database Tables =====
        // Player characters table
        public DbSet<Player> Players { get; set; }

        // Monsters table (TPH inheritance - all monster types in one table)
        public DbSet<Monster> Monsters { get; set; }

        // Abilities table (TPH inheritance - all ability types in one table)
        public DbSet<Ability> Abilities { get; set; }

        // Items table (weapons, armor, loot)
        public DbSet<Item> Items { get; set; }

        // Equipment slots linking players to their gear
        public DbSet<Equipment> Equipments { get; set; }

        // Rooms/locations in the game world
        public DbSet<Room> Rooms { get; set; }

        public GameContext(DbContextOptions<GameContext> options) : base(options)
        {
        }

        /// <summary>
        /// Configures entity relationships, inheritance mappings, and constraints.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== TPH Inheritance for Monsters =====
            // All monster types stored in single table with MonsterType discriminator
            modelBuilder.Entity<Monster>()
                .HasDiscriminator<string>(m => m.MonsterType)
                .HasValue<Goblin>("Goblin")
                .HasValue<Beast>("Beast")
                .HasValue<Undead>("Undead")   
                .HasValue<Bandit>("Bandit");

            // ===== TPH Inheritance for Abilities =====
            // All ability types stored in single table with AbilityType discriminator
            modelBuilder.Entity<Ability>()
                .HasDiscriminator<string>(pa => pa.AbilityType)
                .HasValue<ShoveAbility>("ShoveAbility")
                .HasValue<FireballAbility>("FireballAbility")
                .HasValue<HealAbility>("HealAbility")
                .HasValue<CombatAbility>("CombatAbility");

            // ===== Player-Ability Many-to-Many =====
            // Creates PlayerAbilities join table
            modelBuilder.Entity<Player>()
                .HasMany(p => p.Abilities)
                .WithMany(a => a.Players)
                .UsingEntity(j => j.ToTable("PlayerAbilities"));

            // ===== Room-Player Relationship =====
            // Players belong to rooms, restrict delete to prevent orphaned players
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Players)
                .WithOne(p => p.Room)
                .HasForeignKey(p => p.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // ===== Room-Monster Relationship =====
            // Monsters belong to rooms, restrict delete to prevent orphaned monsters
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Monsters)
                .WithOne(m => m.Room)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // ===== Room Self-Referencing Navigation =====
            // Rooms connect to other rooms in four directions
            // Each relationship needs explicit configuration due to multiple self-references
            modelBuilder.Entity<Room>()
                .HasOne(r => r.NorthRoom)
                .WithMany()
                .HasForeignKey(r => r.NorthRoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.SouthRoom)
                .WithMany()
                .HasForeignKey(r => r.SouthRoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.EastRoom)
                .WithMany()
                .HasForeignKey(r => r.EastRoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.WestRoom)
                .WithMany()
                .HasForeignKey(r => r.WestRoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // ===== Equipment Relationships =====
            ConfigureEquipmentRelationships(modelBuilder);

            // ===== Player-Inventory One-to-One =====
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Inventory)
                .WithOne(i => i.Player)
                .HasForeignKey<Inventory>(i => i.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Inventory-Items Many-to-Many =====
            modelBuilder.Entity<Inventory>()
                .HasMany(i => i.Items)
                .WithMany();

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Configures Equipment-Item relationships.
        /// Weapons and armor are optional (nullable foreign keys).
        /// </summary>
        private void ConfigureEquipmentRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Weapon)
                .WithMany()
                .HasForeignKey(e => e.WeaponId)
                .IsRequired(false);

            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Armor)
                .WithMany()
                .HasForeignKey(e => e.ArmorId)
                .IsRequired(false);
        }
    }
}