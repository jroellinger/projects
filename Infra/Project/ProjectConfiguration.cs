using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectAssignmentPortal.Infrastructure.Organisation;
using ProjectAssignmentPortal.Infrastructure.Worker;

namespace ProjectAssignmentPortal.Infrastructure.Project;

/// <summary>
///     Entity Framework configuration for the ProjectDb entity.
///     Defines table structure, constraints, and indexes.
///     Maps to PostgreSQL table: project
/// </summary>
public class ProjectDbConfiguration : IEntityTypeConfiguration<ProjectDb>
{
    public void Configure(EntityTypeBuilder<ProjectDb> builder)
    {
        builder.ToTable("project");

        // Primary Key
        builder.HasKey(p => p.Id);

        // Configure properties
        builder.Property(p => p.Id)
               .IsRequired()
               .HasColumnName("id");

        builder.Property(p => p.Number)
               .IsRequired()
               .HasColumnName("numero");

        builder.Property(p => p.Name)
               .IsRequired()
               .HasColumnName("libelle");

        // Enum configurations - Use PostgreSQL native enum types
        builder.Property(p => p.Status)
               .IsRequired()
               .HasColumnName("statut");

        builder.Property(p => p.Type)
               .IsRequired()
               .HasColumnName("type");

        builder.Property(p => p.Site)
               .HasColumnName("site");

        builder.Property(p => p.StartDate)
               .HasColumnName("date_debut");

        builder.Property(p => p.EndDate)
               .HasColumnName("date_fin");

        builder.Property(p => p.Comment)
               .HasColumnName("commentaire");

        // Timestamps
        builder.Property(p => p.CreatedAt)
               .IsRequired()
               .HasColumnName("created_at")
               .HasDefaultValueSql("now()");

        builder.Property(p => p.UpdatedAt)
               .HasColumnName("updated_at")
               .HasDefaultValueSql("now()");

        // Organisation foreign keys
        builder.Property(p => p.BusinessUnitId)
               .HasColumnName("business_unit_id");

        builder.Property(p => p.BranchId)
               .HasColumnName("branch_id");

        builder.Property(p => p.DivisionId)
               .HasColumnName("division_id");

        builder.Property(p => p.DepartmentId)
               .HasColumnName("department_id");

        // Additional project properties
        builder.Property(p => p.Arborescence)
               .HasColumnName("arborescence");

        builder.Property(p => p.ReflexNumber)
               .HasColumnName("reflex_number");

        builder.Property(p => p.MissionNumber)
               .HasColumnName("mission_number");

        // Worker foreign keys
        builder.Property(p => p.CreatedById)
               .HasColumnName("created_by_id");

        builder.Property(p => p.ManagerId)
               .HasColumnName("manager_id");

        builder.Property(p => p.SalesManagerId)
               .HasColumnName("sales_manager_id");

        // Foreign key relationships to WorkerDb
        builder.HasOne<WorkerDb>()
               .WithMany()
               .HasForeignKey(p => p.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<WorkerDb>()
               .WithMany()
               .HasForeignKey(p => p.ManagerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<WorkerDb>()
               .WithMany()
               .HasForeignKey(p => p.SalesManagerId)
               .OnDelete(DeleteBehavior.Restrict);

        // Foreign key relationships to OrganisationDb
        builder.HasOne<OrganisationDb>()
               .WithMany()
               .HasForeignKey(p => p.BusinessUnitId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OrganisationDb>()
               .WithMany()
               .HasForeignKey(p => p.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OrganisationDb>()
               .WithMany()
               .HasForeignKey(p => p.DivisionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OrganisationDb>()
               .WithMany()
               .HasForeignKey(p => p.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);

        // Many-to-many relationship with Companies is handled by ProjectCompanyConfiguration

        builder.Property(p => p.DirectoryUrl)
               .HasColumnName("directory_url");

        builder.Property(p => p.TeamsUrl)
               .HasColumnName("teams_url");

        // Indexes for performance (matching PostgreSQL schema)
        builder.HasIndex(p => p.Status)
               .HasDatabaseName("idx_project_statut");

        builder.HasIndex(p => p.Type)
               .HasDatabaseName("idx_project_type");

        // Unique constraint for project number
        builder.HasIndex(p => p.Number)
               .IsUnique();

        // Indexes for organisation foreign keys
        builder.HasIndex(p => p.BusinessUnitId)
               .HasDatabaseName("idx_project_business_unit_id");

        builder.HasIndex(p => p.BranchId)
               .HasDatabaseName("idx_project_branch_id");

        builder.HasIndex(p => p.DivisionId)
               .HasDatabaseName("idx_project_division_id");

        builder.HasIndex(p => p.DepartmentId)
               .HasDatabaseName("idx_project_department_id");

        // Indexes for worker foreign keys
        builder.HasIndex(p => p.CreatedById)
               .HasDatabaseName("idx_project_created_by_id");

        builder.HasIndex(p => p.ManagerId)
               .HasDatabaseName("idx_project_manager_id");

        builder.HasIndex(p => p.SalesManagerId)
               .HasDatabaseName("idx_project_sales_manager_id");

        // Navigation property configuration
        builder.HasMany(p => p.ExternalLinks)
               .WithOne(l => l.Project)
               .HasForeignKey(l => l.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
