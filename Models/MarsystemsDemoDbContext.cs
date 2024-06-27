using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Compras.Models;

public partial class MarsystemsDemoDbContext : DbContext
{
    public MarsystemsDemoDbContext()
    {
    }

    public MarsystemsDemoDbContext(DbContextOptions<MarsystemsDemoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<Carritoscompra> Carritoscompras { get; set; }

    public virtual DbSet<Ordene> Ordenes { get; set; }

    public virtual DbSet<OrdenesArticulo> OrdenesArticulos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=marsystems_demo;Username=postgres;Password=admin;Port=5432");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.Idart).HasName("articulos_pkey");

            entity.ToTable("articulos");

            entity.Property(e => e.Idart).HasColumnName("idart");
            entity.Property(e => e.Nameart)
                .HasMaxLength(100)
                .HasColumnName("nameart");
            entity.Property(e => e.Priceart)
                .HasPrecision(10, 2)
                .HasColumnName("priceart");
            entity.Property(e => e.Quantityart).HasColumnName("quantityart");
        });

        modelBuilder.Entity<Carritoscompra>(entity =>
        {
            entity.HasKey(e => new { e.Idcarrito, e.Idarticulo }).HasName("carritoscompra_pkey");

            entity.ToTable("carritoscompra");

            entity.Property(e => e.Idcarrito).HasColumnName("idcarrito");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue(1)
                .HasColumnName("cantidad");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.Carritoscompras)
                .HasForeignKey(d => d.Idarticulo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("carritoscompra_idarticulo_fkey");

            entity.HasOne(d => d.IdcarritoNavigation).WithMany(p => p.Carritoscompras)
                .HasForeignKey(d => d.Idcarrito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("carritoscompra_idcarrito_fkey");
        });

        modelBuilder.Entity<Ordene>(entity =>
        {
            entity.HasKey(e => e.Idorder).HasName("ordenes_pkey");

            entity.ToTable("ordenes");

            entity.Property(e => e.Idorder).HasColumnName("idorder");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.Iduser).HasColumnName("iduser");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.Ordenes)
                .HasForeignKey(d => d.Iduser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordenes_iduser_fkey");
        });

        modelBuilder.Entity<OrdenesArticulo>(entity =>
        {
            entity.HasKey(e => new { e.Idorder, e.Idarticulo }).HasName("ordenes_articulos_pkey");

            entity.ToTable("ordenes_articulos");

            entity.Property(e => e.Idorder).HasColumnName("idorder");
            entity.Property(e => e.Idarticulo).HasColumnName("idarticulo");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");

            entity.HasOne(d => d.IdarticuloNavigation).WithMany(p => p.OrdenesArticulos)
                .HasForeignKey(d => d.Idarticulo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordenes_articulos_idarticulo_fkey");

            entity.HasOne(d => d.IdorderNavigation).WithMany(p => p.OrdenesArticulos)
                .HasForeignKey(d => d.Idorder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordenes_articulos_idorder_fkey");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Iduser).HasName("usuarios_pkey");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Email, "usuarios_email_key").IsUnique();

            entity.Property(e => e.Iduser).HasColumnName("iduser");
            entity.Property(e => e.Edad).HasColumnName("edad");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nameuser)
                .HasMaxLength(50)
                .HasColumnName("nameuser");
            entity.Property(e => e.Passworduser)
                .HasMaxLength(100)
                .HasColumnName("passworduser");
            entity.Property(e => e.Secondname)
                .HasMaxLength(50)
                .HasColumnName("secondname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
