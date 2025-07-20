namespace ProjectAssignmentPortal.Application.Models.Projects.ProjectFilters;

/// <summary>
///     Filter criteria for projects
/// </summary>
public sealed record ProjectFilters
{
    /// <summary>
    ///     Filter by branch (case-insensitive contains search)
    /// </summary>
    public string? Branch { get; init; }
    /// <summary>
    ///     Filter by business unit (case-insensitive contains search)
    /// </summary>
    public string? BusinessUnit { get; init; }
    /// <summary>
    ///     Filter by creation date (greater than or equal to)
    /// </summary>
    public DateTime? CreatedAtFrom { get; init; }
    /// <summary>
    ///     Filter by creation date (less than or equal to)
    /// </summary>
    public DateTime? CreatedAtTo { get; init; }
    /// <summary>
    ///     Filter by created by (case-insensitive contains search)
    /// </summary>
    public string? CreatedBy { get; init; }
    /// <summary>
    ///     Filter by end date (greater than or equal to)
    /// </summary>
    public DateOnly? EndDateFrom { get; init; }
    /// <summary>
    ///     Filter by end date (less than or equal to)
    /// </summary>
    public DateOnly? EndDateTo { get; init; }
    /// <summary>
    ///     Filter by offer manager (case-insensitive contains search)
    /// </summary>
    public string? Manager { get; init; }
    /// <summary>
    ///     Filter by mission manager (case-insensitive contains search)
    /// </summary>
    public string? MissionManager { get; init; }
    /// <summary>
    ///     Filter by project name (case-insensitive contains search)
    /// </summary>
    public string? Name { get; init; }
    /// <summary>
    ///     Filter by project number (case-insensitive contains search)
    /// </summary>
    public string? Number { get; init; }
    /// <summary>
    ///     Filter by site (case-insensitive contains search)
    /// </summary>
    public string? Site { get; init; }
    /// <summary>
    ///     Filter by start date (greater than or equal to)
    /// </summary>
    public DateOnly? StartDateFrom { get; init; }
    /// <summary>
    ///     Filter by start date (less than or equal to)
    /// </summary>
    public DateOnly? StartDateTo { get; init; }
    /// <summary>
    ///     Filter by project status (exact match)
    /// </summary>
    public string? Status { get; init; }
    /// <summary>
    ///     Filter by project type (exact match)
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    ///     Gets whether any filters are applied
    /// </summary>
    public bool HasFilters() => !string.IsNullOrWhiteSpace(Name)
                                || !string.IsNullOrWhiteSpace(Number)
                                || !string.IsNullOrWhiteSpace(Status)
                                || !string.IsNullOrWhiteSpace(Type)
                                || !string.IsNullOrWhiteSpace(Site)
                                || !string.IsNullOrWhiteSpace(BusinessUnit)
                                || !string.IsNullOrWhiteSpace(Branch)
                                || !string.IsNullOrWhiteSpace(MissionManager)
                                || !string.IsNullOrWhiteSpace(Manager)
                                || !string.IsNullOrWhiteSpace(CreatedBy)
                                || StartDateFrom.HasValue
                                || StartDateTo.HasValue
                                || EndDateFrom.HasValue
                                || EndDateTo.HasValue
                                || CreatedAtFrom.HasValue
                                || CreatedAtTo.HasValue;
}
