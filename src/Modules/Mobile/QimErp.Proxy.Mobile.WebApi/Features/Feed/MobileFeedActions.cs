using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Feed;

/// <summary>
/// Mobile passthroughs for company-news engagement: mark a post read, toggle a
/// reaction (like), and list/create text comments (contributions) — all
/// forwarded to the CoreHr People news module with the caller's JWT.
/// </summary>
public static class MobileFeedActions
{
    public record MarkReadQuery(Guid NewsId) : IRequest<Result<JsonElement>>;

    public class MarkReadHandler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<MarkReadQuery, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(MarkReadQuery request, CancellationToken cancellationToken)
            => peopleClient.MarkNewsReadAsync(request.NewsId, new { readFrom = "mobile-ess" }, cancellationToken);
    }

    public record ToggleReactionQuery(Guid NewsId, string Key) : IRequest<Result<JsonElement>>;

    public class ToggleReactionHandler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<ToggleReactionQuery, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(ToggleReactionQuery request, CancellationToken cancellationToken)
            => peopleClient.ToggleNewsReactionAsync(
                request.NewsId,
                new { key = request.Key },
                cancellationToken);
    }

    public record ListCommentsQuery(Guid NewsId) : IRequest<Result<JsonElement>>;

    public class ListCommentsHandler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<ListCommentsQuery, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(ListCommentsQuery request, CancellationToken cancellationToken)
            => peopleClient.GetNewsContributionsAsync(request.NewsId, cancellationToken);
    }

    public record CreateCommentQuery(Guid NewsId, string Body) : IRequest<Result<JsonElement>>;

    public class CreateCommentHandler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<CreateCommentQuery, Result<JsonElement>>
    {
        public async Task<Result<JsonElement>> Handle(CreateCommentQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Body))
            {
                return Result.WithFailure<JsonElement>(
                    new Error("Feed.CommentEmpty", "Comment cannot be empty."));
            }

            // The news module models comments as "contributions" of type Other
            // with a free-text description — the mobile app renders that as a
            // comment thread. ContributorName is resolved downstream from the
            // caller's JWT (the request still requires a non-empty value, so we
            // pass a placeholder that the backend overwrites with the employee).
            var body = new
            {
                newsId = request.NewsId,
                contributionType = "Other",
                amount = (decimal?)null,
                description = request.Body.Trim(),
                contributorName = "Me",
                metadata = (object?)null,
                notes = (string?)null,
            };
            return await peopleClient.CreateNewsContributionAsync(body, cancellationToken);
        }
    }
}

public class MobileFeedActionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.FeedRead,
                [Authorize] async (Guid newsId, ISender sender) =>
                    (await sender.Send(new MobileFeedActions.MarkReadQuery(newsId))).ToIResult())
            .WithTags(MobileApiConstants.Tags.Me)
            .WithName("MobileFeedMarkRead")
            .WithSummary("Mark a company news item as read for the current employee");

        app.MapPost(MobileApiConstants.Url.FeedReactions,
                [Authorize] async (Guid newsId, ToggleReactionBody body, ISender sender) =>
                    (await sender.Send(new MobileFeedActions.ToggleReactionQuery(newsId, body.Key))).ToIResult())
            .WithTags(MobileApiConstants.Tags.Me)
            .WithName("MobileFeedToggleReaction")
            .WithSummary("Toggle a reaction (e.g. Like) on a company news item");

        app.MapGet(MobileApiConstants.Url.FeedComments,
                [Authorize] async (Guid newsId, ISender sender) =>
                    (await sender.Send(new MobileFeedActions.ListCommentsQuery(newsId))).ToIResult())
            .WithTags(MobileApiConstants.Tags.Me)
            .WithName("MobileFeedListComments")
            .WithSummary("List comments on a company news item");

        app.MapPost(MobileApiConstants.Url.FeedCommentCreate,
                [Authorize] async (Guid newsId, CreateCommentBody body, ISender sender) =>
                    (await sender.Send(new MobileFeedActions.CreateCommentQuery(newsId, body.Body))).ToIResult())
            .WithTags(MobileApiConstants.Tags.Me)
            .WithName("MobileFeedCreateComment")
            .WithSummary("Add a comment to a company news item");
    }

    public sealed record ToggleReactionBody(string Key);

    public sealed record CreateCommentBody(string Body);
}
