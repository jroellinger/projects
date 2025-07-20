using Microsoft.EntityFrameworkCore;
using ProjectAssignmentPortal.Application.Common;
using ProjectAssignmentPortal.Application.Interfaces;
using ProjectAssignmentPortal.Application.Models.Projects;
using ProjectAssignmentPortal.Application.Models.Projects.Enums;
using ProjectAssignmentPortal.Infrastructure.Organisation;
using ProjectAssignmentPortal.Infrastructure.Worker;

namespace ProjectAssignmentPortal.Infrastructure.Project;

/// <summary>
///     Implementation of IProjectRepository using Entity Framework Core.
///     Follows clean architecture by implementing interfaces defined in domain layer.
/// </summary>
public sealed class ProjectRepository : IProjectRepository
{
    private readonly ProjectAssignmentPortalDbContext _dbContext;

    public ProjectRepository(ProjectAssignmentPortalDbContext dbContext) => _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    /// <summary>
    ///     Gets a project by ID with its locations
    /// </summary>
    public async Task<Result<Application.Models.Projects.Project?>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate ID format
            var idValidation = Validate(id);

            if (!idValidation.IsSuccess)
            {
                return Result<Application.Models.Projects.Project?>.Failure(idValidation.Error);
            }

            // First get the project by ID (simple query)
            var project = await _dbContext.Projects.AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.Id == idValidation.Value, cancellationToken);

            if (project == null)
            {
                return Result<Application.Models.Projects.Project?>.Success(null);
            }

            // Then get the related organization data
            var businessUnit = project.BusinessUnitId != null
                ? await _dbContext.Organisations.AsNoTracking()
                                  .FirstOrDefaultAsync(o => o.Id == project.BusinessUnitId, cancellationToken)
                : null;

            var branch = project.BranchId != null
                ? await _dbContext.Organisations.AsNoTracking()
                                  .FirstOrDefaultAsync(o => o.Id == project.BranchId, cancellationToken)
                : null;

            var division = project.DivisionId != null
                ? await _dbContext.Organisations.AsNoTracking()
                                  .FirstOrDefaultAsync(o => o.Id == project.DivisionId, cancellationToken)
                : null;

            var department = project.DepartmentId != null
                ? await _dbContext.Organisations.AsNoTracking()
                                  .FirstOrDefaultAsync(o => o.Id == project.DepartmentId, cancellationToken)
                : null;

            // Get project companies
            var companies = await (from pc in _dbContext.ProjectCompanies.AsNoTracking()
                join c in _dbContext.Companies.AsNoTracking() on pc.CompanyId equals c.Id
                where pc.ProjectId == idValidation.Value
                select new ProjectMapper.ProjectCompanyMappingData(c)).ToListAsync(cancellationToken);

            // Get project locations
            var locations = await (from pl in _dbContext.ProjectLocations.AsNoTracking()
                join l in _dbContext.Locations.AsNoTracking() on pl.LocationId equals l.Id
                where pl.ProjectId == idValidation.Value
                select l).ToListAsync(cancellationToken);

            // Create the mapping data manually
            var projectData = new ProjectMapper.ProjectMappingData(project, businessUnit, branch, division, department, companies, null, null, null, locations);

            // Map to domain entity using shared mapping logic
            var mappingResult = await MapProjectToDomainAsync(projectData, cancellationToken);

            if (!mappingResult.IsSuccess)
            {
                return Result<Application.Models.Projects.Project?>.Failure(mappingResult.Error);
            }

            return Result<Application.Models.Projects.Project?>.Success(mappingResult.Value);
        }
        catch (Exception ex)
        {
            return Result<Application.Models.Projects.Project?>.Failure($"Failed to get project: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<Projects>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Use optimized query for list view - only fetch manager info
            var projectListMappingDatas = await GetProjects()
                .ToListAsync(cancellationToken);

            var projectsResult = Projects.Create();

            if (!projectsResult.IsSuccess)
            {
                return Result<Projects>.Failure(projectsResult.Error);
            }

            var projects = projectsResult.Value;

            foreach (var projectData in projectListMappingDatas)
            {
                // Use lightweight mapping for list view
                var mappingResult = MapProjectToDomainForList(projectData);

                if (!mappingResult.IsSuccess)
                {
                    continue;
                }

                var addResult = projects.Add(mappingResult.Value);

                if (!addResult.IsSuccess)
                {
                    return Result<Projects>.Failure(addResult.Error);
                }
            }

            return Result<Projects>.Success(projects);
        }
        catch (Exception ex)
        {
            return Result<Projects>.Failure($"Failed to get projects: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<Projects>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            // Use optimized query for list view with status filter
            var projectsData = await GetProjectsWithManagerQueryByStatus(status)
                .ToListAsync(cancellationToken);

            var projectsResult = Projects.Create();

            if (!projectsResult.IsSuccess)
            {
                return Result<Projects>.Failure(projectsResult.Error);
            }

            var projects = projectsResult.Value;

            foreach (var projectData in projectsData)
            {
                // Use lightweight mapping for list view
                var mappingResult = MapProjectToDomainForList(projectData);

                if (!mappingResult.IsSuccess)
                {
                    continue;
                }

                var addResult = projects.Add(mappingResult.Value);

                if (!addResult.IsSuccess)
                {
                    return Result<Projects>.Failure(addResult.Error);
                }
            }

            return Result<Projects>.Success(projects);
        }
        catch (Exception ex)
        {
            return Result<Projects>.Failure($"Failed to get projects by status: {ex.Message}");
        }
    }

    /// <summary>
    ///     Return
    /// </summary>
    /// <returns></returns>
    private IQueryable<ProjectListMappingData> GetProjects() => from project in _dbContext.Projects.AsNoTracking()
        join bu in _dbContext.Organisations on project.BusinessUnitId equals bu.Id into buGroup
        from businessUnit in buGroup.DefaultIfEmpty()
        join br in _dbContext.Organisations on project.BranchId equals br.Id into brGroup
        from branch in brGroup.DefaultIfEmpty()
        join div in _dbContext.Organisations on project.DivisionId equals div.Id into divGroup
        from division in divGroup.DefaultIfEmpty()
        join dept in _dbContext.Organisations on project.DepartmentId equals dept.Id into deptGroup
        from department in deptGroup.DefaultIfEmpty()
        join manager in _dbContext.Workers on project.ManagerId equals manager.Id into managerGroup
        from manager in managerGroup.DefaultIfEmpty()
        join salesManager in _dbContext.Workers on project.SalesManagerId equals salesManager.Id into salesManagerGroup
        from salesManager in salesManagerGroup.DefaultIfEmpty()
        select new ProjectListMappingData(project, businessUnit, branch, division, department, manager, salesManager);

    /// <summary>
    ///     Optimized query for list view with status filter - includes manager info but not all workers
    /// </summary>
    private IQueryable<ProjectListMappingData> GetProjectsWithManagerQueryByStatus(ProjectStatus status) => from project in _dbContext.Projects.AsNoTracking()
        where project.Status == status
        join bu in _dbContext.Organisations on project.BusinessUnitId equals bu.Id into buGroup
        from businessUnit in buGroup.DefaultIfEmpty()
        join br in _dbContext.Organisations on project.BranchId equals br.Id into brGroup
        from branch in brGroup.DefaultIfEmpty()
        join div in _dbContext.Organisations on project.DivisionId equals div.Id into divGroup
        from division in divGroup.DefaultIfEmpty()
        join dept in _dbContext.Organisations on project.DepartmentId equals dept.Id into deptGroup
        from department in deptGroup.DefaultIfEmpty()
        join manager in _dbContext.Workers on project.ManagerId equals manager.Id into managerGroup
        from manager in managerGroup.DefaultIfEmpty()
        join salesManager in _dbContext.Workers on project.SalesManagerId equals salesManager.Id into salesManagerGroup
        from salesManager in salesManagerGroup.DefaultIfEmpty()
        select new ProjectListMappingData(project, businessUnit, branch, division, department, manager, salesManager);

    /// <summary>
    ///     Common query structure for projects with organization data (for details view)
    /// </summary>
    private IQueryable<ProjectMapper.ProjectMappingData> GetProjectsWithOrganizationQuery() => from project in _dbContext.Projects.AsNoTracking()
        join bu in _dbContext.Organisations on project.BusinessUnitId equals bu.Id into buGroup
        from businessUnit in buGroup.DefaultIfEmpty()
        join br in _dbContext.Organisations on project.BranchId equals br.Id into brGroup
        from branch in brGroup.DefaultIfEmpty()
        join div in _dbContext.Organisations on project.DivisionId equals div.Id into divGroup
        from division in divGroup.DefaultIfEmpty()
        join dept in _dbContext.Organisations on project.DepartmentId equals dept.Id into deptGroup
        from department in deptGroup.DefaultIfEmpty()
        // Companies will be loaded separately for many-to-many relationship
        select new ProjectMapper.ProjectMappingData(project, businessUnit, branch, division, department, null, null, null, null, null);

    /// <summary>
    ///     Helper method to fetch a worker by ID and convert to domain entity
    /// </summary>
    private async Task<Application.Models.Workers.Worker?> GetWorkerAsync(string? workerId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(workerId))
        {
            return null;
        }

        var workerDb = await _dbContext.Workers.AsNoTracking()
                                       .FirstOrDefaultAsync(w => w.Id == workerId, cancellationToken);

        if (workerDb == null)
        {
            return null;
        }

        var workerResult = Application.Models.Workers.Worker.Create(workerDb.Id, workerDb.FirstName, workerDb.Name, workerDb.Email);

        return workerResult.IsSuccess ? workerResult.Value : null;
    }

    /// <summary>
    ///     Helper to get worker from pre-loaded dictionary
    /// </summary>
    private Application.Models.Workers.Worker? GetWorkerFromDictionary(string? workerId, Dictionary<string, WorkerDb> workers)
    {
        if (string.IsNullOrEmpty(workerId)
            || !workers.TryGetValue(workerId, out var workerDb))
        {
            return null;
        }

        var workerResult = Application.Models.Workers.Worker.Create(workerDb.Id, workerDb.FirstName, workerDb.Name, workerDb.Email);

        return workerResult.IsSuccess ? workerResult.Value : null;
    }

    /// <summary>
    ///     Maps project data to domain entity with all relationships (for details view)
    /// </summary>
    private async Task<Result<Application.Models.Projects.Project>> MapProjectToDomainAsync(ProjectMapper.ProjectMappingData projectData,
                                                                                            CancellationToken cancellationToken)
    {
        try
        {
            // Fetch workers concurrently
            var creator = await GetWorkerAsync(projectData.Project.CreatedById, cancellationToken);
            var manager = await GetWorkerAsync(projectData.Project.ManagerId, cancellationToken);
            var salesManager = await GetWorkerAsync(projectData.Project.SalesManagerId, cancellationToken);

            // Add workers to the existing mapping data
            var mappingDataWithWorkers = projectData with
                                         {
                                             Creator = creator,
                                             Manager = manager,
                                             SalesManager = salesManager,
                                         };

            return projectData.Project.ToDomainEntity(mappingDataWithWorkers);
        }
        catch (Exception ex)
        {
            return Result<Application.Models.Projects.Project>.Failure($"Failed to map project: {ex.Message}");
        }
    }

    /// <summary>
    ///     Lightweight mapping for list view - only includes manager (already fetched in query)
    /// </summary>
    private Result<Application.Models.Projects.Project> MapProjectToDomainForList(ProjectListMappingData projectData)
    {
        try
        {
            // Convert WorkerDb to domain Worker if manager exists
            var manager = projectData.ManagerDb != null
                ? Application.Models.Workers.Worker.Create(projectData.ManagerDb.Id,
                                                           projectData.ManagerDb.FirstName,
                                                           projectData.ManagerDb.Name,
                                                           projectData.ManagerDb.Email)
                             .Value
                : null;

            var salesManager = projectData.SalesManagerDb != null
                ? Application.Models.Workers.Worker.Create(projectData.SalesManagerDb.Id,
                                                           projectData.SalesManagerDb.FirstName,
                                                           projectData.SalesManagerDb.Name,
                                                           projectData.SalesManagerDb.Email)
                             .Value
                : null;

            // Create full mapping data for domain entity creation
            var mappingDataForDomain = new ProjectMapper.ProjectMappingData(projectData.Project,
                                                                            projectData.BusinessUnit,
                                                                            projectData.Branch,
                                                                            projectData.Division,
                                                                            projectData.Department,
                                                                            null,
                                                                            null,
                                                                            manager,
                                                                            salesManager);

            return projectData.Project.ToDomainEntity(mappingDataForDomain);
        }
        catch (Exception ex)
        {
            return Result<Application.Models.Projects.Project>.Failure($"Failed to map project for list: {ex.Message}");
        }
    }

    /// <summary>
    ///     Maps project using pre-loaded workers dictionary (no additional queries)
    /// </summary>
    private Result<Application.Models.Projects.Project> MapProjectToDomainWithBatchedWorkers(ProjectMapper.ProjectMappingData projectData,
                                                                                             Dictionary<string, WorkerDb> workers)
    {
        try
        {
            // Get workers from dictionary (no DB queries)
            var creator = GetWorkerFromDictionary(projectData.Project.CreatedById, workers);
            var manager = GetWorkerFromDictionary(projectData.Project.ManagerId, workers);
            var salesManager = GetWorkerFromDictionary(projectData.Project.SalesManagerId, workers);

            var mappingDataWithWorkers = projectData with
                                         {
                                             Creator = creator,
                                             Manager = manager,
                                             SalesManager = salesManager,
                                         };

            return projectData.Project.ToDomainEntity(mappingDataWithWorkers);
        }
        catch (Exception ex)
        {
            return Result<Application.Models.Projects.Project>.Failure($"Failed to map project with batched workers: {ex.Message}");
        }
    }

    /// <summary>
    ///     Validates and parses a project ID string to Guid
    /// </summary>
    private static Result<string> Validate(string id) => string.IsNullOrWhiteSpace(id) ? Result<string>.Failure("Project ID cannot be null or empty")
        : Guid.TryParse(id, out var guid) ? Result<string>.Success(guid.ToString()) : Result<string>.Failure("Invalid project ID format");

    /// <summary>
    ///     Mapping data for optimized list queries - includes WorkerDb for manager
    /// </summary>
    private sealed record ProjectListMappingData(
        ProjectDb Project,
        OrganisationDb? BusinessUnit,
        OrganisationDb? Branch,
        OrganisationDb? Division,
        OrganisationDb? Department,
        WorkerDb? ManagerDb,
        WorkerDb? SalesManagerDb);
}
