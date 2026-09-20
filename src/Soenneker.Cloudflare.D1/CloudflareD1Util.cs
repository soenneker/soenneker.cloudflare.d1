using System;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cloudflare.D1.Abstract;
using Soenneker.Cloudflare.OpenApiClient.Models;
using Soenneker.Cloudflare.Utils.Client.Abstract;

namespace Soenneker.Cloudflare.D1;

public sealed class CloudflareD1Util(ICloudflareClientUtil clientUtil) : ICloudflareD1Util
{
    public async ValueTask<D1ListDatabases200?> ListDatabases(string accountId, string apiKey, string? name = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        if (page is < 1) throw new ArgumentOutOfRangeException(nameof(page));
        if (perPage is < 1) throw new ArgumentOutOfRangeException(nameof(perPage));
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database.GetAsync(config => { config.QueryParameters.Name = name; config.QueryParameters.Page = page; config.QueryParameters.PerPage = perPage; }, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1CreateDatabase200?> CreateDatabase(string accountId, string apiKey, D1CreateDatabase body, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentNullException.ThrowIfNull(body);
        ArgumentException.ThrowIfNullOrWhiteSpace(body.Name);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database.PostAsync(body, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1GetDatabase200?> GetDatabase(string accountId, string apiKey, string databaseId, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1DeleteDatabase200?> DeleteDatabase(string accountId, string apiKey, string databaseId, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].DeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1UpdateDatabase200?> UpdateDatabase(string accountId, string apiKey, string databaseId, D1DatabaseUpdateRequestBody body, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        ArgumentNullException.ThrowIfNull(body);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].PutAsync(body, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1UpdatePartialDatabase200?> UpdateDatabasePartial(string accountId, string apiKey, string databaseId, D1DatabaseUpdatePartialRequestBody body, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        ArgumentNullException.ThrowIfNull(body);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].PatchAsync(body, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1QueryDatabase200?> Query(string accountId, string apiKey, string databaseId, D1BatchQuery body, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        ValidateQuery(body);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].Query.PostAsync(body, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1RawDatabaseQuery200?> QueryRaw(string accountId, string apiKey, string databaseId, D1BatchQuery body, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        ValidateQuery(body);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].Raw.PostAsync(body, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1ExportDatabase200?> ExportDatabase(string accountId, string apiKey, string databaseId, D1ExportDatabase body, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        ArgumentNullException.ThrowIfNull(body);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].Export.PostAsync(body, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1ImportDatabase200?> ImportDatabase(string accountId, string apiKey, string databaseId, D1ImportDatabase body, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        ArgumentNullException.ThrowIfNull(body);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].Import.PostAsync(body, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1TimeTravelGetBookmark200?> GetTimeTravelBookmark(string accountId, string apiKey, string databaseId, string? timestamp = null, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].Time_travel.Bookmark.GetAsync(config => config.QueryParameters.Timestamp = timestamp, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<D1TimeTravelRestore200?> RestoreDatabase(string accountId, string apiKey, string databaseId, string bookmark, CancellationToken cancellationToken = default)
    {
        ValidateAccount(accountId, apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookmark);
        cancellationToken.ThrowIfCancellationRequested();
        var client = await clientUtil.Get(apiKey, cancellationToken).ConfigureAwait(false);
        return await client.Accounts[accountId].D1.Database[databaseId].Time_travel.Restore.PostAsync(config => config.QueryParameters.Bookmark = bookmark, cancellationToken).ConfigureAwait(false);
    }

    private static void ValidateAccount(string accountId, string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
    }

    private static void ValidateQuery(D1BatchQuery body)
    {
        ArgumentNullException.ThrowIfNull(body);
        if ((body.D1SingleQuery is null) == (body.D1BatchQueryMember1 is null))
            throw new ArgumentException("Specify exactly one single query or batch.", nameof(body));

        if (body.D1SingleQuery is { } query)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(query.Sql);
            return;
        }

        var batch = body.D1BatchQueryMember1!.Batch;
        if (batch is null || batch.Count == 0)
            throw new ArgumentException("A batch must contain at least one query.", nameof(body));
        foreach (var statement in batch)
        {
            ArgumentNullException.ThrowIfNull(statement);
            ArgumentException.ThrowIfNullOrWhiteSpace(statement.Sql);
        }
    }
}

