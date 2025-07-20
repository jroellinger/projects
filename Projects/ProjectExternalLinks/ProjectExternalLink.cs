using ProjectAssignmentPortal.Application.Common;

namespace ProjectAssignmentPortal.Application.Models.Projects.ProjectExternalLinks;

/// <summary>
///     External Link entity representing a link associated with a project.
/// </summary>
public class ProjectExternalLink
{
    public string Id { get; private set; }
    public string Label { get; private set; }
    public string ProjectId { get; private set; }
    public ExternalLinkType Type { get; private set; }
    public string Url { get; private set; }
    // Private constructor for EF Core

    private ProjectExternalLink(string id, string projectId, ExternalLinkType type, string label, string url)
    {
        Id = id;
        ProjectId = projectId;
        Type = type;
        Label = label;
        Url = url;
    }

    /// <summary>
    ///     Factory method to create a new ProjectExternalLink.
    ///     Returns Result pattern to handle validation failures.
    /// </summary>
    public static Result<ProjectExternalLink> Create(string projectId, ExternalLinkType type, string label, string url)
    {
        var validationResult = ValidateInputs(label, url);

        if (!validationResult.IsSuccess)
        {
            return Result<ProjectExternalLink>.Failure(validationResult.Error);
        }

        var id = Guid.NewGuid()
                     .ToString();

        var link = new ProjectExternalLink(id, projectId, type, label.Trim(), url.Trim());

        return Result<ProjectExternalLink>.Success(link);
    }

    private static Result ValidateInputs(string label, string url)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return Result.Failure("Label cannot be empty");
        }

        if (label.Length > 255)
        {
            return Result.Failure("Label cannot exceed 255 characters");
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            return Result.Failure("URL cannot be empty");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return Result.Failure("URL must be a valid HTTP or HTTPS URL");
        }

        return Result.Success();
    }
}
