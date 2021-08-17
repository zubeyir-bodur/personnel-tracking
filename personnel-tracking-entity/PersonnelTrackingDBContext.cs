using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace personnel_tracking_entity
{
    public partial class PersonnelTrackingDBContext : DbContext
    {
        public PersonnelTrackingDBContext()
        {
        }

        public PersonnelTrackingDBContext(DbContextOptions<PersonnelTrackingDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Leave> Leaves { get; set; }
        public virtual DbSet<Personnel> Personnel { get; set; }
        public virtual DbSet<PersonnelType> PersonnelTypes { get; set; }
        public virtual DbSet<Tracking> Trackings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // optionsBuilder.UseSqlServer("Server=.;Database=PersonnelTrackingDB;User Id=test;Password=test123");
                optionsBuilder.UseSqlServer("Data Source =.; Initial Catalog = PersonnelTrackingDB; Integrated Security = True; MultipleActiveResultSets=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Area");

                entity.Property(e => e.AreaId).HasColumnName("area_id");

                entity.Property(e => e.AreaName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("area_name");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.Latitude)
                    .HasColumnType("decimal(6, 3)")
                    .HasColumnName("latitude");

                entity.Property(e => e.Longitude)
                    .HasColumnType("decimal(6, 3)")
                    .HasColumnName("longitude");

                entity.Property(e => e.QrCode)
                    .HasColumnType("image")
                    .HasColumnName("qr_code");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Areas)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Company");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("company_name");
            });

            modelBuilder.Entity<Leave>(entity =>
            {
                entity.ToTable("Leave");

                entity.Property(e => e.LeaveId).HasColumnName("leave_id");

                entity.Property(e => e.LeaveEnd)
                    .HasColumnType("date")
                    .HasColumnName("leave_end");

                entity.Property(e => e.LeaveStart)
                    .HasColumnType("date")
                    .HasColumnName("leave_start");

                entity.Property(e => e.PersonnelId).HasColumnName("personnel_id");

                entity.HasOne(d => d.Personnel)
                    .WithMany(p => p.Leaves)
                    .HasForeignKey(d => d.PersonnelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Personnel>(entity =>
            {
                entity.HasIndex(e => e.UserName, "IX_Personel")
                    .IsUnique();

                entity.Property(e => e.PersonnelId).HasColumnName("personnel_id");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.IdentityNumber).HasColumnName("identity_number");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.PersonnelName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("personnel_name");

                entity.Property(e => e.PersonnelSurname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("personnel_surname");

                entity.Property(e => e.PersonnelTypeId).HasColumnName("personnel_type_id");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("user_name");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Personnel)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PersonnelType)
                    .WithMany(p => p.Personnel)
                    .HasForeignKey(d => d.PersonnelTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PersonnelType>(entity =>
            {
                entity.ToTable("PersonnelType");

                entity.Property(e => e.PersonnelTypeId).HasColumnName("personnel_type_id");

                entity.Property(e => e.PersonnelTypeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("personnel_type_name");
            });

            modelBuilder.Entity<Tracking>(entity =>
            {
                entity.ToTable("Tracking");

                entity.Property(e => e.TrackingId).HasColumnName("tracking_id");

                entity.Property(e => e.AreaId).HasColumnName("area_id");

                entity.Property(e => e.AutoExit).HasColumnName("auto_exit");

                entity.Property(e => e.EntranceDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("entrance_date");

                entity.Property(e => e.ExitDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("exit_date");

                entity.Property(e => e.PersonnelId).HasColumnName("personnel_id");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Trackings)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Personnel)
                    .WithMany(p => p.Trackings)
                    .HasForeignKey(d => d.PersonnelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
