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
            var degraded = new List<string>();
            JsonElement? essHome = null;
            JsonElement? leaveBalance = null;
            JsonElement? pending = null;
            JsonElement? payrollSummary = null;

            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            Task<Result<JsonElement>>? essHomeTask = null;
            if (employeeId is { } resolvedEmployeeId)
            {
                essHomeTask = performanceClient.GetEssHomeAsync(resolvedEmployeeId, cancellationToken);
            }
            else
            {
                degraded.Add("essHome");
            }

            var leaveTask = leaveClient.GetBalanceAsync(cancellationToken);
            var workflowTask = workflowClient.GetPendingAsync(1, 5, cancellationToken);
            var payrollTask = payrollClient.GetMySummaryAsync(cancellationToken);

            if (essHomeTask is not null)
            {
                var (data, failed) = await TrySectionAsync(essHomeTask, "essHome", degraded, cancellationToken);
                essHome = data;
                _ = failed;
            }

            var (leave, leaveFailed) = await TrySectionAsync(leaveTask, "leaveBalance", degraded, cancellationToken);
            leaveBalance = leave;
            _ = leaveFailed;

            var (pendingData, pendingFailed) = await TrySectionAsync(workflowTask, "pendingApprovals", degraded, cancellationToken);
            pending = pendingData;
            _ = pendingFailed;

            var (payroll, payrollFailed) = await TrySectionAsync(payrollTask, "payrollSummary", degraded, cancellationToken);
            payrollSummary = payroll;
            _ = payrollFailed;

            return Result.WithSuccess(new MobileHomeResponse
            {
                EssHome = essHome,
                LeaveBalance = leaveBalance,
                PendingApprovals = pending,
                PayrollSummary = payrollSummary,
                DegradedSections = degraded
            });
        }

        /// <summary>
        /// Awaits one downstream section with a short per-section timeout and
        /// records it as degraded on failure or timeout, so a slow/hung module
        /// degrades the section instead of 500ing (or stalling) the whole home
        /// composition.
        /// </summary>
        private static async Task<(JsonElement? Data, bool Failed)> TrySectionAsync(
            Task<Result<JsonElement>> task,
            string section,
            List<string> degraded,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await task.WaitAsync(TimeSpan.FromSeconds(8));
                if (result.IsSuccess)
                {
                    return (result.Data, false);
                }
                degraded.Add(section);
                return (null, true);
            }
            catch (TimeoutException)
            {
                degraded.Add(section);
                return (null, true);
            }
            catch (TaskCanceledException)
            {
                // HttpClient timeout — degrade rather than fail the whole request.
                degraded.Add(section);
                return (null, true);
            }
            catch (HttpRequestException)
            {
                degraded.Add(section);
                return (null, true);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
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
