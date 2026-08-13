using QimErp.Proxy.Mobile.Shared.Contracts;
using QimErp.Shared.Common.Extensions;

namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface IPeopleDownstreamClient
{
    Task<Result<JsonElement>> GetFeedAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetCurrentEmployeeAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> UpdatePersonalInfoAsync(Guid employeeId, object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> UpdateEssContactInfoAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> SubmitEssPersonalChangeRequestAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetMyEssPersonalChangeRequestsAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> CancelEssPersonalChangeRequestAsync(Guid id, object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> SubmitEssLearningChangeRequestAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetMyEssLearningChangeRequestsAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> CancelEssLearningChangeRequestAsync(Guid id, object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> UpdateProfilePictureAsync(Guid employeeId, Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetDirectoryAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetFiltersAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetQualificationsAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetDocumentsAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> MarkNewsReadAsync(Guid newsId, object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> ToggleNewsReactionAsync(Guid newsId, object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetNewsContributionsAsync(Guid newsId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> CreateNewsContributionAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<List<CertificationResponse>>> GetCertificationsAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Result<List<TrainingResponse>>> GetTrainingsAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Result<List<NextOfKinResponse>>> GetNextOfKinsAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetEssDependantsAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> AddEssDependantAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetEssNextOfKinsAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> AddEssNextOfKinAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<EmergencyContactResponse>>> GetEmergencyContactsAsync(Guid employeeId, object body, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<HrQueryResponse>>> GetMyQueriesAsync(Guid employeeId, object body, CancellationToken cancellationToken = default);
}

public sealed class PeopleDownstreamClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<PeopleDownstreamClient> logger)
    : DownstreamHttpClientBase(httpClientFactory, httpContextAccessor, logger), IPeopleDownstreamClient
{
    protected override string ClientName => DownstreamClientNames.People;

    public Task<Result<JsonElement>> GetFeedAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.PeopleNewsMy, cancellationToken);

    public Task<Result<JsonElement>> GetCurrentEmployeeAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.PeopleCurrentEmployee, cancellationToken);

    public Task<Result<JsonElement>> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PeopleEmployee, employeeId), cancellationToken);

    public Task<Result<JsonElement>> UpdatePersonalInfoAsync(Guid employeeId, object body, CancellationToken cancellationToken = default)
        => PutRawAsync(string.Format(MobileApiConstants.Downstream.PeoplePersonalInfo, employeeId), body, cancellationToken);

    public Task<Result<JsonElement>> UpdateEssContactInfoAsync(object body, CancellationToken cancellationToken = default)
        => PutRawAsync(MobileApiConstants.Downstream.PeopleEssContactInfo, body, cancellationToken);

    public Task<Result<JsonElement>> SubmitEssPersonalChangeRequestAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PeopleEssPersonalChangeRequests, body, cancellationToken);

    public Task<Result<JsonElement>> GetMyEssPersonalChangeRequestsAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.PeopleEssPersonalChangeRequestsMine, cancellationToken);

    public Task<Result<JsonElement>> CancelEssPersonalChangeRequestAsync(Guid id, object body, CancellationToken cancellationToken = default)
        => PostRawAsync(
            string.Format(MobileApiConstants.Downstream.PeopleEssPersonalChangeRequestCancel, id),
            body,
            cancellationToken);

    public Task<Result<JsonElement>> SubmitEssLearningChangeRequestAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PeopleEssLearningChangeRequests, body, cancellationToken);

    public Task<Result<JsonElement>> GetMyEssLearningChangeRequestsAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.PeopleEssLearningChangeRequestsMine, cancellationToken);

    public Task<Result<JsonElement>> CancelEssLearningChangeRequestAsync(Guid id, object body, CancellationToken cancellationToken = default)
        => PostRawAsync(
            string.Format(MobileApiConstants.Downstream.PeopleEssLearningChangeRequestCancel, id),
            body,
            cancellationToken);

    public Task<Result<JsonElement>> UpdateProfilePictureAsync(Guid employeeId, Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        => PutFileFormRawAsync(
            string.Format(MobileApiConstants.Downstream.PeopleProfilePicture, employeeId),
            "file",
            fileStream,
            fileName,
            contentType,
            cancellationToken);

    public Task<Result<JsonElement>> GetDirectoryAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PeopleDirectory, body, cancellationToken);

    public Task<Result<JsonElement>> GetFiltersAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.PeopleFilters, cancellationToken);

    public Task<Result<JsonElement>> GetQualificationsAsync(Guid employeeId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PeopleQualifications, employeeId), cancellationToken);

    public Task<Result<JsonElement>> GetDocumentsAsync(Guid employeeId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PeopleDocuments, employeeId), cancellationToken);

    public Task<Result<JsonElement>> MarkNewsReadAsync(Guid newsId, object body, CancellationToken cancellationToken = default)
        => PostRawAsync(string.Format(MobileApiConstants.Downstream.PeopleNewsRead, newsId), body, cancellationToken);

    public Task<Result<JsonElement>> ToggleNewsReactionAsync(Guid newsId, object body, CancellationToken cancellationToken = default)
        => PostRawAsync(string.Format(MobileApiConstants.Downstream.PeopleNewsReactionToggle, newsId), body, cancellationToken);

    public Task<Result<JsonElement>> GetNewsContributionsAsync(Guid newsId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PeopleNewsContributions, newsId), cancellationToken);

    public Task<Result<JsonElement>> CreateNewsContributionAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PeopleContributionsCreate, body, cancellationToken);

    public Task<Result<List<CertificationResponse>>> GetCertificationsAsync(Guid employeeId, CancellationToken cancellationToken = default)
        => GetAsync<List<CertificationResponse>>(string.Format(MobileApiConstants.Downstream.PeopleCertifications, employeeId), cancellationToken);

    public Task<Result<List<TrainingResponse>>> GetTrainingsAsync(Guid employeeId, CancellationToken cancellationToken = default)
        => GetAsync<List<TrainingResponse>>(string.Format(MobileApiConstants.Downstream.PeopleTrainings, employeeId), cancellationToken);

    public Task<Result<List<NextOfKinResponse>>> GetNextOfKinsAsync(Guid employeeId, CancellationToken cancellationToken = default)
        => GetAsync<List<NextOfKinResponse>>(string.Format(MobileApiConstants.Downstream.PeopleNextOfKins, employeeId), cancellationToken);

    public Task<Result<JsonElement>> GetEssDependantsAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.PeopleEssDependants, cancellationToken);

    public Task<Result<JsonElement>> AddEssDependantAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PeopleEssDependants, body, cancellationToken);

    public Task<Result<JsonElement>> GetEssNextOfKinsAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.PeopleEssNextOfKins, cancellationToken);

    public Task<Result<JsonElement>> AddEssNextOfKinAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PeopleEssNextOfKins, body, cancellationToken);

    public Task<Result<PaginatedList<EmergencyContactResponse>>> GetEmergencyContactsAsync(Guid employeeId, object body, CancellationToken cancellationToken = default)
        => PostAsync<PaginatedList<EmergencyContactResponse>>(string.Format(MobileApiConstants.Downstream.PeopleEmergencyContactsPage, employeeId), body, cancellationToken);

    public Task<Result<PaginatedList<HrQueryResponse>>> GetMyQueriesAsync(Guid employeeId, object body, CancellationToken cancellationToken = default)
        => PostAsync<PaginatedList<HrQueryResponse>>(string.Format(MobileApiConstants.Downstream.PeopleQueriesPage, employeeId), body, cancellationToken);
}
