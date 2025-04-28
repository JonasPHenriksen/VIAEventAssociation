using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess;

public partial class VeadatabaseProductionContext : DbContext
{
    public VeadatabaseProductionContext()
    {
    }

    public VeadatabaseProductionContext(DbContextOptions<VeadatabaseProductionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<GuestId> GuestIds { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=/home/jonas/RiderProjects/VIAEventAssociation/Infrastructure/EfcDataAccess/VEADatabaseProduction.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<GuestId>(entity =>
        {
            entity.ToTable("GuestId");

            entity.HasIndex(e => e.VeaEventEventId, "IX_GuestId_VeaEventEventId");

            entity.HasOne(d => d.VeaEventEvent).WithMany(p => p.GuestIds).HasForeignKey(d => d.VeaEventEventId);
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => new { e.VeaEventId, e.GuestId });

            entity.ToTable("Invitation");

            entity.HasIndex(e => e.GuestId, "IX_Invitation_GuestId");

            entity.HasOne(d => d.Guest).WithMany(p => p.Invitations).HasForeignKey(d => d.GuestId);

            entity.HasOne(d => d.VeaEvent).WithMany(p => p.Invitations).HasForeignKey(d => d.VeaEventId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
