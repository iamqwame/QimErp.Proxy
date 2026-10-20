using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Home;

public static class GetMobileHome
{
    public class Query : IRequest<Result<MobileHomeResponse>> { }

    public class Handler(
        IPerformanceDownstreamClient performanceClient,
        ILeaveDownstreamClient leaveClient,
        IWorkflowDownstreamClient workflowClient,
        IPayrollDownstreamClient payrollClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Query, Result<MobileHomeResponse>>
    {
        public async Task<Result<MobileHomeResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);

            // DATA-27: one parallel fan-out for home sections (no sequential downstream round trips).
            Task<(string Section, JsonElement? Data, bool Failed)> essHomeTask;
            if (employeeId is { } resolvedEmployeeId)
            {
                essHomeTask = TrySectionAsync(
                    performanceClient.GetEssHomeAsync(resolvedEmployeeId, cancellationToken),
                    "essHome",
                    cancellationToken);
            }
            else
            {
                essHomeTask = Task.FromResult<(string, JsonElement?, bool)>(("essHome", null, true));
            }

            var leaveTask = TrySectionAsync(
                leaveClient.GetBalanceAsync(cancellationToken), "leaveBalance", cancellationToken);
            var workflowTask = TrySectionAsync(
                workflowClient.GetPendingAsync(1, 5, cancellationToken), "pendingApprovals", cancellationToken);
            var payrollTask = TrySectionAsync(
                payrollClient.GetMySummaryAsync(cancellationToken), "payrollSummary", cancellationToken);

            var sections = await Task.WhenAll(essHomeTask, leaveTask, workflowTask, payrollTask);

            var degraded = sections.Where(s => s.Failed).Select(s => s.Section).ToList();
            var bySection = sections.ToDictionary(s => s.Section, s => s.Data);

            return Result.WithSuccess(new MobileHomeResponse
            {
                EssHome = bySection.GetValueOrDefault("essHome"),
                LeaveBalance = bySection.GetValueOrDefault("leaveBalance"),
                PendingApprovals = bySection.GetValueOrDefault("pendingApprovals"),
                PayrollSummary = bySection.GetValueOrDefault("payrollSummary"),
                DegradedSections = degraded
            });
        }

        private static async Task<(string Section, JsonElement? Data, bool Failed)> TrySectionAsync(
            Task<Result<JsonElement>> task,
            string section,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await task.WaitAsync(TimeSpan.FromSeconds(8), cancellationToken);
                if (result.IsSuccess)
                {
                    return (section, result.Data, false);
                }

                return (section, null, true);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (TimeoutException)
            {
                return (section, null, true);
            }
            catch (TaskCanceledException)
            {
                return (section, null, true);
            }
            catch (HttpRequestException)
            {
                return (section, null, true);
            }
        }
    }
}

public class GetMobileHomeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.Home,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileHome.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Home)
            .WithName("MobileHome")
            .WithSummary("Mobile ESS home composition");
    }
}
