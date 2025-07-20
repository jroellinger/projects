namespace ProjectAssignmentPortal.Application.Models.Projects.ProjectFilters;

/// <summary>
///     Filter option information for organisation with hierarchy details.
/// </summary>
public sealed record OrganisationFilterOption(string Id, string Name, string Type, string? Code = null, string? ParentId = null, string? ParentName = null);
/// <summary>
///     Date range information for filter options.
/// </summary>
public sealed record DateRangeOption(DateOnly? MinDate, DateOnly? MaxDate);
/// <summary>
///     Manager information for filter options.
/// </summary>
public sealed record ManagerFilterOption(string Id, string Name, string? FirstName = null, string? Email = null);
/// <summary>
///     Available filter options extracted from project data.
/// </summary>
public sealed record ProjectFilterOptions(
    IReadOnlyList<OrganisationFilterOption> BusinessUnits,
    IReadOnlyList<OrganisationFilterOption> Branches,
    IReadOnlyList<OrganisationFilterOption> Divisions,
    IReadOnlyList<OrganisationFilterOption> Departments,
    DateRangeOption StartDateRange,
    DateRangeOption EndDateRange,
    DateRangeOption CreatedDateRange,
    DateRangeOption UpdatedDateRange,
    IReadOnlyList<ManagerFilterOption> Managers,
    IReadOnlyList<ManagerFilterOption> SalesManagers,
    IReadOnlyList<string> Statuses,
    IReadOnlyList<string> Types)
{
    /// <summary>
    ///     Creates filter options from a collection of project responses.
    /// </summary>
    /// <param name="projects">The collection of project responses to extract filter options from.</param>
    /// <returns>A new instance of ProjectFilterOptions with extracted values.</returns>
    public static ProjectFilterOptions CreateFromProjects(IEnumerable<ProjectResponse> projects)
    {
        var projectList = projects.ToList();

        // Extract organisations with hierarchy
        var businessUnits = ExtractOrganisationOptions(projectList, x => x.BusinessUnit, "BusinessUnit");
        var branches = ExtractOrganisationOptionsWithParent(projectList, x => x.Branch, x => x.BusinessUnit, "Branch");
        var divisions = ExtractOrganisationOptionsWithParent(projectList, x => x.Division, x => x.Branch, "Division");
        var departments = ExtractOrganisationOptionsWithParent(projectList, x => x.Department, x => x.Division, "Department");

        // Extract date ranges
        var startDateRange = ExtractDateRange(projectList, x => x.StartDate);
        var endDateRange = ExtractDateRange(projectList, x => x.EndDate);
        var createdDateRange = ExtractDateRange(projectList, x => DateOnly.FromDateTime(x.CreatedAt));
        var updatedDateRange = ExtractDateRange(projectList, x => x.UpdatedAt.HasValue ? DateOnly.FromDateTime(x.UpdatedAt.Value) : null);

        // Extract managers
        var managers = ExtractManagerOptions(projectList, x => x.Manager);
        var salesManagers = ExtractManagerOptions(projectList, x => x.SalesManager);

        // Extract distinct statuses and types
        var statuses = projectList.Select(x => x.Status)
                                  .Distinct()
                                  .Where(x => !string.IsNullOrEmpty(x))
                                  .ToList();

        var types = projectList.Select(x => x.Type)
                               .Distinct()
                               .Where(x => !string.IsNullOrEmpty(x))
                               .ToList();

        return new ProjectFilterOptions(businessUnits,
                                        branches,
                                        divisions,
                                        departments,
                                        startDateRange,
                                        endDateRange,
                                        createdDateRange,
                                        updatedDateRange,
                                        managers,
                                        salesManagers,
                                        statuses,
                                        types);
    }

    private static DateRangeOption ExtractDateRange(IEnumerable<ProjectResponse> projects, Func<ProjectResponse, DateOnly?> selector)
    {
        var dates = projects.Select(selector)
                            .Where(date => date.HasValue)
                            .Select(date => date!.Value)
                            .ToList();

        if (!dates.Any())
        {
            return new DateRangeOption(null, null);
        }

        return new DateRangeOption(dates.Min(), dates.Max());
    }

    private static IReadOnlyList<ManagerFilterOption> ExtractManagerOptions(IEnumerable<ProjectResponse> projects, Func<ProjectResponse, WorkerInfo?> selector)
    {
        return projects.Select(selector)
                       .Where(manager => manager != null && !string.IsNullOrEmpty(manager.Name))
                       .Cast<WorkerInfo>()
                       .GroupBy(manager => manager.Id)
                       .Select(group => group.First())
                       .Select(manager => new ManagerFilterOption(manager.Id, manager.Name, manager.FirstName, manager.Email))
                       .ToList();
    }

    private static IReadOnlyList<OrganisationFilterOption> ExtractOrganisationOptions(IEnumerable<ProjectResponse> projects,
                                                                                      Func<ProjectResponse, OrganisationInfo?> selector,
                                                                                      string type)
    {
        return projects.Select(selector)
                       .Where(org => org != null)
                       .Cast<OrganisationInfo>()
                       .GroupBy(org => org.NameFr)
                       .Select(group => group.First())
                       .Select(org => new OrganisationFilterOption(org.Id, org.NameFr, type, org.Code))
                       .ToList();
    }

    private static IReadOnlyList<OrganisationFilterOption> ExtractOrganisationOptionsWithParent(IEnumerable<ProjectResponse> projects,
                                                                                                Func<ProjectResponse, OrganisationInfo?> selector,
                                                                                                Func<ProjectResponse, OrganisationInfo?> parentSelector,
                                                                                                string type)
    {
        var projectList = projects.ToList();

        // Create a lookup for parent relationships
        var parentLookup = projectList.Where(p => selector(p) != null && parentSelector(p) != null)
                                      .GroupBy(p => selector(p)!.NameFr)
                                      .ToDictionary(g => g.Key, g => parentSelector(g.First())!);

        return projectList.Select(selector)
                          .Where(org => org != null)
                          .Cast<OrganisationInfo>()
                          .GroupBy(org => org.NameFr)
                          .Select(group => group.First())
                          .Select(org => new OrganisationFilterOption(org.Id,
                                                                      org.NameFr,
                                                                      type,
                                                                      org.Code,
                                                                      parentLookup.ContainsKey(org.NameFr) ? parentLookup[org.NameFr].Id : null,
                                                                      parentLookup.ContainsKey(org.NameFr) ? parentLookup[org.NameFr].NameFr : null))
                          .ToList();
    }
}
