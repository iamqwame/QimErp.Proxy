using System.Globalization;
using System.Net.Http.Headers;

namespace QimErp.Proxy.Mobile.Shared.Clients;

public abstract class DownstreamHttpClientBase(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    protected abstract string ClientName { get; }

    protected async Task<Result<T>> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        return await SendAsync<T>(HttpMethod.Get, relativeUrl, content: null, cancellationToken);
    }

    protected async Task<Result<T>> PostAsync<T>(string relativeUrl, object? body, CancellationToken cancellationToken)
    {
        return await SendAsync<T>(HttpMethod.Post, relativeUrl, body, cancellationToken);
    }

    protected async Task<Result<T>> PutAsync<T>(string relativeUrl, object? body, CancellationToken cancellationToken)
    {
        return await SendAsync<T>(HttpMethod.Put, relativeUrl, body, cancellationToken);
    }

    protected async Task<Result<JsonElement>> GetRawAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        return await SendAsync<JsonElement>(HttpMethod.Get, relativeUrl, content: null, cancellationToken);
    }

    /// Fetches a binary response (e.g. a generated PDF) instead of a JSON
    /// `Result&lt;T&gt;` envelope — the downstream endpoint returns the file
    /// directly via `Results.File(...)`, not wrapped JSON.
    protected async Task<Result<DownstreamFile>> GetFileAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(ClientName);
            using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl.TrimStart('/'));
            ApplyCallerHeaders(request);

            using var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.WithFailure<DownstreamFile>(
                    new Error($"{ClientName}.HttpError", payload),
                    code: ((int)response.StatusCode).ToString());
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                ?? response.Content.Headers.ContentDisposition?.FileName
                ?? "download";

            return Result.WithSuccess(new DownstreamFile(bytes, contentType, fileName));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "{Client} file download failed for {Url}", ClientName, relativeUrl);
            return Result.WithFailure<DownstreamFile>(
                new Error($"{ClientName}.Unavailable", "Downstream service is unavailable."),
                code: "503");
        }
    }

    protected async Task<Result<JsonElement>> PostRawAsync(string relativeUrl, object? body, CancellationToken cancellationToken)
    {
        return await SendAsync<JsonElement>(HttpMethod.Post, relativeUrl, body, cancellationToken);
    }

    protected async Task<Result<JsonElement>> PutRawAsync(string relativeUrl, object? body, CancellationToken cancellationToken)
    {
        return await SendAsync<JsonElement>(HttpMethod.Put, relativeUrl, body, cancellationToken);
    }

    /// Posts a body as multipart/form-data string fields. Used when the downstream
    /// endpoint binds from form (e.g. leave requests that accept file attachments).
    protected async Task<Result<JsonElement>> PostFormRawAsync(
        string relativeUrl,
        IReadOnlyDictionary<string, object?> fields,
        CancellationToken cancellationToken)
    {
        var form = new MultipartFormDataContent();
        foreach (var (key, value) in fields)
        {
            if (value is null)
            {
                continue;
            }

            form.Add(
                new StringContent(
                    Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty),
                key);
        }

        return await SendAsync<JsonElement>(
            HttpMethod.Post,
            relativeUrl,
            content: null,
            formContent: form,
            cancellationToken);
    }

    /// Puts a single file as multipart/form-data. Used for profile-picture uploads.
    protected async Task<Result<JsonElement>> PutFileFormRawAsync(
        string relativeUrl,
        string fieldName,
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        var form = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(fileContent, fieldName, fileName);

        return await SendAsync<JsonElement>(
            HttpMethod.Put,
            relativeUrl,
            content: null,
            formContent: form,
            cancellationToken);
    }

    private async Task<Result<T>> SendAsync<T>(
        HttpMethod method,
        string relativeUrl,
        object? content,
        CancellationToken cancellationToken)
        => await SendAsync<T>(method, relativeUrl, content, formContent: null, cancellationToken);

    private async Task<Result<T>> SendAsync<T>(
        HttpMethod method,
        string relativeUrl,
        object? content,
        HttpContent? formContent,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(ClientName);
            using var request = new HttpRequestMessage(method, relativeUrl.TrimStart('/'));
            ApplyCallerHeaders(request);

            if (formContent is not null)
            {
                request.Content = formContent;
            }
            else if (content is not null)
            {
                request.Content = JsonContent.Create(content, options: JsonOptions);
            }

            using var response = await client.SendAsync(request, cancellationToken);
            var payload = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(payload))
            {
                if (response.IsSuccessStatusCode)
                {
                    return Result.WithSuccess(default(T)!);
                }

                return Result.WithFailure<T>(
                    new Error($"{ClientName}.EmptyResponse", $"Downstream returned {(int)response.StatusCode} with empty body."),
                    code: ((int)response.StatusCode).ToString());
            }

            DownstreamApiEnvelope<T>? envelope;
            try
            {
                envelope = JsonSerializer.Deserialize<DownstreamApiEnvelope<T>>(payload, JsonOptions);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "{Client} returned non-Result JSON for {Url}", ClientName, relativeUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return Result.WithFailure<T>(
                        new Error($"{ClientName}.HttpError", payload),
                        code: ((int)response.StatusCode).ToString());
                }

                var raw = JsonSerializer.Deserialize<T>(payload, JsonOptions);
                return Result.WithSuccess(raw!);
            }

            if (envelope is null)
            {
                return Result.WithFailure<T>(
                    new Error($"{ClientName}.DeserializeFailed", "Unable to parse downstream response."),
                    code: "500");
            }

            if (!response.IsSuccessStatusCode || envelope.IsFailure || !envelope.IsSuccess)
            {
                var errorCode = envelope.Error?.Code ?? $"{ClientName}.DownstreamFailure";
                var errorMessage = envelope.Error?.Message ?? envelope.Message ?? "Downstream request failed.";
                return Result.WithFailure<T>(
                    new Error(errorCode, errorMessage),
                    envelope.Message ?? errorMessage,
                    envelope.Code ?? ((int)response.StatusCode).ToString());
            }

            return Result.WithSuccess(envelope.Data!, envelope.Message ?? "Request Sent Successfully", envelope.Code ?? "200");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "{Client} call failed for {Url}", ClientName, relativeUrl);
            return Result.WithFailure<T>(
                new Error($"{ClientName}.Unavailable", "Downstream service is unavailable."),
                code: "503");
        }
    }

    private void ApplyCallerHeaders(HttpRequestMessage request)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }

        var auth = httpContext.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(auth))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(auth);
        }

        if (httpContext.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationId)
            && !string.IsNullOrWhiteSpace(correlationId))
        {
            request.Headers.TryAddWithoutValidation("X-Correlation-Id", correlationId.ToString());
        }
    }
}
