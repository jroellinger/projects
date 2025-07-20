using ProjectAssignmentPortal.Application.Common;
using ProjectAssignmentPortal.Infrastructure.Company;
using ProjectAssignmentPortal.Infrastructure.Location;
using ProjectAssignmentPortal.Infrastructure.Organisation;

namespace ProjectAssignmentPortal.Infrastructure.Project;

/// <summary>
///     Maps between domain entities and database entities.
///     Following clean architecture principles by keeping infrastructure mapping logic separate.
/// </summary>
public static class ProjectMapper
{
    /// <summary>
    ///     Maps from database entity to domain entity with complete data.
    ///     Used when retrieving data from the database with joined organization, company, and worker information.
    ///     Uses the Builder pattern to reconstruct projects from database data.
    /// </summary>
    public static Result<Application.Models.Projects.Project> ToDomainEntity(this ProjectDb projectDb, ProjectMappingData? mappingData = null)
    {
        if (projectDb == null)
        {
            return Result<Application.Models.Projects.Project>.Failure("Project database entity cannot be null");
        }

        try
        {
            var project = Application.Models.Projects.Project.Builder()
                                     .WithId(projectDb.Id.ToString())
                                     .WithNumber(projectDb.Number)
                                     .WithName(projectDb.Name)
                                     .WithType(projectDb.Type)
                                     .WithStatus(projectDb.Status)
                                     .WithCreatedAt(projectDb.CreatedAt)
                                     .WithOptionalUpdatedAt(projectDb.UpdatedAt)
                                     .WithOptionalSite(projectDb.Site)
                                     .WithOptionalComment(projectDb.Comment)
                                     .WithOptionalStartDate(projectDb.StartDate)
                                     .WithOptionalEndDate(projectDb.EndDate)
                                     .WithOptionalDirectoryUrl(projectDb.DirectoryUrl)
                                     .WithOptionalBusinessUnit(mappingData?.BusinessUnit?.ToDomainEntity())
                                     .WithOptionalBranch(mappingData?.Branch?.ToDomainEntity())
                                     .WithOptionalDivision(mappingData?.Division?.ToDomainEntity())
                                     .WithOptionalDepartment(mappingData?.Department?.ToDomainEntity())
                                     .WithOptionalCompanies(mappingData?.Companies?.Where(pc => pc.Company != null)
                                                                       .Select(pc => new
                                                                                     {
                                                                                         Company = pc.Company!.ToDomainEntity(),
                                                                                     })
                                                                       .Where(x => x.Company.IsSuccess)
                                                                       .Select(x => x.Company.Value)
                                                                       .ToList() ?? [])
                                     // Set worker properties if provided
                                     .WithOptionalProjectCreator(mappingData?.Creator)
                                     .WithOptionalManager(mappingData?.Manager)
                                     .WithOptionalSalesManager(mappingData?.SalesManager)
                                     // Set locations if provided
                                     .WithOptionalLocations(mappingData?.Locations?.Select(l => (l.Id, l.Code, l.Label))
                                                                       .ToList())
                                     .Build();

            return project.IsSuccess
                ? Result<Application.Models.Projects.Project>.Success(project.Value)
                : Result<Application.Models.Projects.Project>.Failure($"Failed to reconstruct project from database: {project.Error}");
        }
        catch (Exception ex)
        {
            return Result<Application.Models.Projects.Project>.Failure($"Error mapping project from database: {ex.Message}");
        }
    }

    /// <summary>
    ///     Contains related data for project mapping
    /// </summary>
    public sealed record ProjectMappingData(
        ProjectDb Project,
        OrganisationDb? BusinessUnit = null,
        OrganisationDb? Branch = null,
        OrganisationDb? Division = null,
        OrganisationDb? Department = null,
        List<ProjectCompanyMappingData>? Companies = null,
        Application.Models.Workers.Worker? Creator = null,
        Application.Models.Workers.Worker? Manager = null,
        Application.Models.Workers.Worker? SalesManager = null,
        List<LocationDb>? Locations = null);
    /// <summary>
    ///     Contains company data with role for project mapping
    /// </summary>
    public sealed record ProjectCompanyMappingData(CompanyDb? Company);
}
