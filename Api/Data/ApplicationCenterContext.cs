using System;
using System.Collections.Generic;
using System.Text.Json;
using ApplicationCenter.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCenter.Api.Data;

public partial class ApplicationCenterContext : DbContext
{
    public ApplicationCenterContext()
    {
    }

    public ApplicationCenterContext(DbContextOptions<ApplicationCenterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=ApplicationCenter;Username=postgres;Password=admin");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("applications_pkey");

            entity.ToTable("applications");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(a => a.Files).HasColumnName("files").HasColumnType("text[]");
            entity.Property(e => e.Fullname).HasColumnName("fullname");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Servicetype).HasColumnName("servicetype");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Новая'::text")
                .HasColumnName("status")
                .HasConversion<string>();
            entity.Property(e => e.Userid)
                .HasColumnName("userid")
                .HasColumnType("uuid");
            entity.Property(e => e.Xmlpath).HasColumnName("xmlpath");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("applications_userid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "users_login_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Fullname).HasColumnName("fullname");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Placeofwork).HasColumnName("placeofwork");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
