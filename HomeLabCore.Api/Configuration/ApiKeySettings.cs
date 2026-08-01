using System.ComponentModel.DataAnnotations;

namespace HomeLabCore.Api.Configuration;

public sealed record ApiKeyAuthenticationSettings
{
    public const string SectionName = "ApiKeyAuthentication";

    [Required]
    public required string[] ValidKeys { get; set; }
}
