using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartPark.Models;

namespace SmartPark.Data;

public class SmartParkContext : IdentityDbContext<ApplicationUser>
{
    public SmartParkContext(DbContextOptions<SmartParkContext> options) : base(options) { }
    public DbSet<Parkirisce> Parkirisca { get; set; }
    public DbSet<ParkirnoMesto> ParkirnaMesta { get; set; }
    public DbSet<Rezervacija> Rezervacije { get; set; }
    public DbSet<Placilo> Placila { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Parkirisce>().ToTable("Parkirisce");
        modelBuilder.Entity<ParkirnoMesto>().ToTable("ParkirnoMesto");
        modelBuilder.Entity<Rezervacija>().ToTable("Rezervacija");
        modelBuilder.Entity<Placilo>().ToTable("Placilo");

        modelBuilder.Entity<ParkirnoMesto>()
            .HasOne(pm => pm.Parkirisce)
            .WithMany(p => p.ParkirnaMesta)
            .HasForeignKey(pm => pm.ParkirisceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Rezervacija>()
            .HasOne(r => r.Uporabnik)
            .WithMany()
            .HasForeignKey(r => r.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rezervacija>()
            .HasOne(r => r.Parkirisce)
            .WithMany()
            .HasForeignKey(r => r.ParkirisceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rezervacija>()
            .HasOne(r => r.ParkirnoMesto)
            .WithMany()
            .HasForeignKey(r => r.ParkirnoMestoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Placilo>()
            .HasOne(p => p.Uporabnik)
            .WithMany()
            .HasForeignKey(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Placilo>()
            .HasOne(p => p.Rezervacija)
            .WithMany()
            .HasForeignKey(p => p.RezervacijaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Parkirisce>()
            .Property(p => p.CenaNaUro)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Placilo>()
            .Property(p => p.Znesek)
            .HasPrecision(18, 2);
    }
}
