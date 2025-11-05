using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<ApplicationPermission> Permissions { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // ¡IMPORTANTE mantener esto!

        // Especificar schema para todas las tablas
        builder.HasDefaultSchema("public");

        // Configurar la tabla de Usuarios con UUID
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");

            // Configurar ID como UUID nativo de PostgreSQL
            entity.Property(e => e.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            // Personalizar longitudes
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);

            // Agregar índices
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
        });

        // Configurar la tabla de Roles con UUID
        builder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("Roles");

            // Configurar ID como UUID nativo de PostgreSQL
            entity.Property(e => e.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
        });

        // Configurar la tabla de Permisos con UUID
        builder.Entity<ApplicationPermission>(entity =>
        {
            entity.ToTable("Permissions");

            // Configurar ID como UUID nativo de PostgreSQL
            entity.Property(e => e.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            // Configurar propiedades
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsRequired();

            // Índice único en Name para evitar permisos duplicados
            entity.HasIndex(e => e.Name).IsUnique();
        });

        builder.Entity<ApplicationRole>()
        .HasMany(r => r.ApplicationPermissions)
        .WithMany()
        .UsingEntity<Dictionary<string, object>>(
            "RolePermissions", // Nombre de la tabla intermedia
            j => j
                .HasOne<ApplicationPermission>()
                .WithMany()
                .HasForeignKey("PermissionId")
                .HasConstraintName("FK_RolePermissions_Permissions"),
            j => j
                .HasOne<ApplicationRole>()
                .WithMany()
                .HasForeignKey("RoleId")
                .HasConstraintName("FK_RolePermissions_Roles"),
            j =>
            {
                j.ToTable("RolePermissions");
                j.HasKey("RoleId", "PermissionId");

                // Configurar IDs como UUID
                j.Property("RoleId").HasColumnType("uuid");
                j.Property("PermissionId").HasColumnType("uuid");
            }
        );

        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}