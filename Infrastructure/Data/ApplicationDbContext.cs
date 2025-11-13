using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.Entities;
using Application.Interfaces;

namespace Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IApplicationDbContext
{
    public DbSet<ApplicationPermission> Permissions { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // ¡IMPORTANTE mantener esto!

        // Especificar schema para todas las tablas
        builder.HasDefaultSchema("public");

        // Configurar filtro global para borrado lógico
        // builder.Entity<ApplicationUser>().HasQueryFilter(e => e.IsActive);
        // builder.Entity<ApplicationRole>().HasQueryFilter(e => e.IsActive);
        builder.Entity<ApplicationPermission>().HasQueryFilter(e => e.IsActive);

        // Configurar la tabla de Usuarios con UUID
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");

            // Configurar ID como UUID nativo de PostgreSQL
            entity.Property(e => e.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            // Configurar IsActive con valor por defecto
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

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

            // Configurar IsActive con valor por defecto
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

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

            // Configurar IsActive con valor por defecto
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

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
                
                // Agregar columna IsActive para borrado lógico
                j.Property<bool>("IsActive").HasDefaultValue(true);
            }
        );

        // Configurar tabla UserRoles con IsActive
        builder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.Property<bool>("IsActive").HasDefaultValue(true);
        });
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}