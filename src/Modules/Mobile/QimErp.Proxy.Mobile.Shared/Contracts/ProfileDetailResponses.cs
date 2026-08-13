namespace QimErp.Proxy.Mobile.Shared.Contracts;

public sealed class CertificationResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string IssuingOrganization { get; set; } = "";
    public DateOnly IssueDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public string? CertificateNumber { get; set; }
    public string? Status { get; set; }
}

public sealed class TrainingResponse
{
    public Guid Id { get; set; }
    public string TrainingProgram { get; set; } = "";
    public string TrainingProvider { get; set; } = "";
    public DateOnly CompletionDate { get; set; }
    public int? DurationHours { get; set; }
    public string? Certificate { get; set; }
    public string? Status { get; set; }
}

public sealed class NextOfKinResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Relationship { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public sealed class EmergencyContactResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Relationship { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsPrimaryContact { get; set; }
    public bool IsEmergencyContact { get; set; }
}

public sealed class HrQueryResponse
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = "";
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public string Status { get; set; } = "";
    public string? Resolution { get; set; }
    public DateTime? ResolvedDate { get; set; }
}

public sealed class AuditActivityResponse
{
    public Guid Id { get; set; }
    public string Module { get; set; } = "";
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public string? SubjectLabel { get; set; }
}
