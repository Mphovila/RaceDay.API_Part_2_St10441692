using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data;

public partial class RaceDayContext : DbContext
{
    public RaceDayContext(DbContextOptions<RaceDayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Enrolment> Enrolments { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<RaceDay.API.Models.Route> Routes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WeatherInformation> WeatherInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B8F55EB5D");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Distance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EntryFee).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Event).WithMany(p => p.Categories)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categories_Events");
        });

        modelBuilder.Entity<Enrolment>(entity =>
        {
            entity.HasKey(e => e.EnrolmentId).HasName("PK__Enrolmen__5C0E5FCF7718664B");

            entity.Property(e => e.EnrolmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Category).WithMany(p => p.Enrolments)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrolments_Categories");

            entity.HasOne(d => d.Event).WithMany(p => p.Enrolments)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrolments_Events");

            entity.HasOne(d => d.Participant).WithMany(p => p.Enrolments)
                .HasForeignKey(d => d.ParticipantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrolments_Users");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C810C3841358");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EntryFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.EventName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Organiser).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganiserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_Users");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__Results__976902081223D256");

            entity.HasIndex(e => e.EnrolmentId, "UQ__Results__5C0E5FCEB83D3B89").IsUnique();

            entity.Property(e => e.FinishTime)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ResultStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Enrolment).WithOne(p => p.Result)
                .HasForeignKey<Result>(d => d.EnrolmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Results_Enrolments");
        });

        modelBuilder.Entity<RaceDay.API.Models.Route>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__Routes__80979B4D36C19270");

            entity.Property(e => e.Distance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EndLocation)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.RouteDescription)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.RouteName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.StartLocation)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Event).WithMany(p => p.Routes)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Routes_Events");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C1AE5721D");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053486615978").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WeatherInformation>(entity =>
        {
            entity.HasKey(e => e.WeatherId).HasName("PK__WeatherI__0BF97BF532C796DC");

            entity.ToTable("WeatherInformation");

            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Temperature).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.WeatherDescription)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.WindSpeed).HasColumnType("decimal(5, 2)");

            
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
