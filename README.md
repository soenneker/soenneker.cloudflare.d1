[![](https://img.shields.io/nuget/v/soenneker.cloudflare.d1.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.d1/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.d1/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.d1/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.cloudflare.d1.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.d1/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Cloudflare.D1
### A utility library for Cloudflare D1 database operations.

## Installation

```
dotnet add package Soenneker.Cloudflare.D1
```

## Usage

Targets .NET 10. Register the utility and its Cloudflare client dependency:

```csharp
using Soenneker.Cloudflare.D1.Registrars;

services.AddCloudflareD1UtilAsSingleton();
// AddCloudflareD1UtilAsScoped() is also available.
```

Inject `ICloudflareD1Util` from `Soenneker.Cloudflare.D1.Abstract`. Supply the account
ID and Cloudflare API bearer token for each call. Store the token in your secret
store; do not embed it in source code. Database reads require D1 Read permission;
mutating operations require D1 Write permission.

```csharp
using Soenneker.Cloudflare.OpenApiClient.Models;

var databases = await d1.ListDatabases(accountId, apiToken,
    page: 1, perPage: 20, cancellationToken: cancellationToken);

var result = await d1.Query(accountId, apiToken, databaseId,
    new D1BatchQuery
    {
        D1SingleQuery = new D1SingleQuery
        {
            Sql = "SELECT * FROM users WHERE name = ?",
            Params = ["Alice"]
        }
    }, cancellationToken);
```

Use parameters for SQL values. To submit a batch, populate
`D1BatchQuery.D1BatchQueryMember1` with a `D1BatchQueryMember1` whose `Batch` contains
`D1SingleQuery` objects. Exactly one single-query or batch member must be set.
The generated SDK represents query parameters as strings.

## Operations

- `ListDatabases`: one page of databases with optional name filtering.
- `CreateDatabase`, `GetDatabase`, `UpdateDatabase`, `UpdateDatabasePartial`, and `DeleteDatabase`.
- `Query` and `QueryRaw`: parameterized SQL or batches, returning object rows or raw arrays.
- `ExportDatabase` and `ImportDatabase`: typed requests for the API's export/import stages.
- `GetTimeTravelBookmark` and `RestoreDatabase`: bookmark lookup and database restoration.

Methods return generated Cloudflare response envelopes, including errors, statement
results, and pagination metadata. Inspect envelope and per-statement success fields;
HTTP/API exceptions and cancellation propagate to the caller. The utility does not
automatically paginate, retry mutations, upload import files, poll export/import jobs,
or download exports. Delete and restore calls modify the database immediately.

API reference: https://developers.cloudflare.com/api/resources/d1/subresources/database/

## Build and test

```powershell
dotnet build -c Release
dotnet test --project test/Soenneker.Cloudflare.D1.Tests -c Release -- --treenode-filter '/*/*/CloudflareD1UtilTests/*'
dotnet pack src/Soenneker.Cloudflare.D1 -c Release -o artifacts
```

Tests use an in-memory HTTP handler and do not require Cloudflare credentials or
access live databases.
