using System;
using Soenneker.Cloudflare.OpenApiClient.Models;
using System.Threading;
using System.Text.Json;
using System.Threading.Tasks;

namespace Soenneker.Cloudflare.D1.Abstract;

/// <summary>
/// Provides Cloudflare D1 operations using a per-call account identifier and API bearer token.
/// Returns the generated Cloudflare response envelopes without hiding API or statement errors.
/// </summary>
public interface ICloudflareD1Util
{
    /// <summary>
    /// Lists one page of databases, optionally filtered by name. Pagination metadata is preserved.
    /// </summary>
    ValueTask<D1ListDatabases200Response?> ListDatabases(string accountId, string apiKey, string? name = null, int? page = null, int? perPage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a database using the supplied name, location, and replication settings.
    /// </summary>
    ValueTask<D1CreateDatabase200Response?> CreateDatabase(string accountId, string apiKey, D1CreateDatabaseRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a database by its identifier.
    /// </summary>
    ValueTask<D1GetDatabase200Response?> GetDatabase(string accountId, string apiKey, string databaseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes the specified database.
    /// </summary>
    ValueTask<D1DeleteDatabase200Response?> DeleteDatabase(string accountId, string apiKey, string databaseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates database settings using the complete update request.
    /// </summary>
    ValueTask<D1UpdateDatabase200Response?> UpdateDatabase(string accountId, string apiKey, string databaseId, D1DatabaseUpdateRequestBody body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates only the supplied database settings.
    /// </summary>
    ValueTask<D1UpdatePartialDatabase200Response?> UpdateDatabasePartial(string accountId, string apiKey, string databaseId, D1DatabaseUpdatePartialRequestBody body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a parameterized query or batch and returns rows as objects. Inspect the response and each statement's success and errors.
    /// </summary>
    ValueTask<D1QueryDatabase200Response?> Query(string accountId, string apiKey, string databaseId, D1BatchQuery body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a parameterized query or batch and preserves the complete JSON response, including
    /// the array of statement results that the generated query model cannot represent.
    /// </summary>
    /// <remarks>The caller owns the returned document and must dispose it. HTTP failures and malformed
    /// JSON throw; inspect the envelope and each statement for application-level errors.</remarks>
    ValueTask<JsonDocument> QueryDocument(string accountId, string apiKey, string databaseId, D1BatchQuery body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a parameterized query or batch and returns rows as arrays with column metadata.
    /// </summary>
    ValueTask<D1RawDatabaseQuery200Response?> QueryRaw(string accountId, string apiKey, string databaseId, D1BatchQuery body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts or polls a SQL export using the supplied export request and bookmark.
    /// </summary>
    ValueTask<D1ExportDatabase200Response?> ExportDatabase(string accountId, string apiKey, string databaseId, D1ExportDatabaseRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs one step of the import API flow. Uploading SQL to the returned upload URL is the caller's responsibility.
    /// </summary>
    ValueTask<D1ImportDatabase200Response?> ImportDatabase(string accountId, string apiKey, string databaseId, D1ImportDatabaseRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a Time Travel bookmark for an optional RFC3339 timestamp.
    /// </summary>
    ValueTask<D1TimeTravelGetBookmark200Response?> GetTimeTravelBookmark(string accountId, string apiKey, string databaseId, DateTimeOffset? timestamp = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores the database to the supplied Time Travel bookmark, replacing its current contents.
    /// </summary>
    ValueTask<D1TimeTravelRestore200Response?> RestoreDatabase(string accountId, string apiKey, string databaseId, string bookmark, CancellationToken cancellationToken = default);
}

