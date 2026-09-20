using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Cloudflare.OpenApiClient;
using Soenneker.Cloudflare.OpenApiClient.Models;
using Soenneker.Cloudflare.Utils.Client.Abstract;

namespace Soenneker.Cloudflare.D1.Tests;

public sealed class CloudflareD1UtilTests
{
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Query_preserves_parameters_and_uses_correct_endpoint(bool raw)
    {
        using var fixture = new Fixture();
        var body = new D1BatchQuery { D1SingleQuery = new D1SingleQuery
        {
            Sql = "SELECT * FROM users WHERE name = ?", Params = ["O'Reilly"]
        }};
        if (raw)
            await fixture.Util.QueryRaw("account", "token", "database", body);
        else
            await fixture.Util.Query("account", "token", "database", body);

        await Assert.That(fixture.Handler.Method).IsEqualTo("POST");
        await Assert.That(fixture.Handler.Uri).IsEqualTo("https://api.cloudflare.com/client/v4/accounts/account/d1/database/database/" + (raw ? "raw" : "query"));
        using var json = JsonDocument.Parse(fixture.Handler.Body!);
        await Assert.That(json.RootElement.GetProperty("sql").GetString()).IsEqualTo(body.D1SingleQuery.Sql);
        await Assert.That(json.RootElement.GetProperty("params")[0].GetString()).IsEqualTo("O'Reilly");
        await Assert.That(fixture.ClientUtil.Token).IsEqualTo("token");
    }

    [Test]
    public async Task Batch_serializes_as_batch_object()
    {
        using var fixture = new Fixture();
        await fixture.Util.Query("account", "token", "database", new D1BatchQuery
        {
            D1BatchQueryMember1 = new D1BatchQueryMember1 { Batch = [new D1SingleQuery { Sql = "SELECT 1" }, new D1SingleQuery { Sql = "SELECT 2" }] }
        });
        using var json = JsonDocument.Parse(fixture.Handler.Body!);
        await Assert.That(json.RootElement.GetProperty("batch").GetArrayLength()).IsEqualTo(2);
    }

    [Test]
    public async Task List_passes_pagination_and_encodes_name()
    {
        using var fixture = new Fixture();
        await fixture.Util.ListDatabases("account", "token", "test database", 2, 10);
        await Assert.That(fixture.Handler.Method).IsEqualTo("GET");
        await Assert.That(fixture.Handler.Uri!.Contains("page=2")).IsTrue();
        await Assert.That(fixture.Handler.Uri.Contains("per_page=10")).IsTrue();
        await Assert.That(fixture.Handler.Uri.Contains("name=test%20database")).IsTrue();
    }

    [Test]
    public async Task Rejects_ambiguous_query_before_acquiring_client()
    {
        using var fixture = new Fixture();
        try
        {
            await fixture.Util.Query("account", "token", "database", new D1BatchQuery
            {
                D1SingleQuery = new D1SingleQuery { Sql = "SELECT 1" },
                D1BatchQueryMember1 = new D1BatchQueryMember1 { Batch = [new D1SingleQuery { Sql = "SELECT 2" }] }
            });
            throw new InvalidOperationException("Expected validation failure.");
        }
        catch (ArgumentException) { }
        await Assert.That(fixture.ClientUtil.Token).IsNull();
    }

    [Test]
    public async Task Cancellation_stops_before_acquiring_client()
    {
        using var fixture = new Fixture();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        try
        {
            await fixture.Util.GetDatabase("account", "token", "database", cancellation.Token);
            throw new InvalidOperationException("Expected cancellation.");
        }
        catch (OperationCanceledException) { }
        await Assert.That(fixture.ClientUtil.Token).IsNull();
    }

    [Test]
    public async Task Api_errors_are_propagated()
    {
        using var fixture = new Fixture();
        fixture.Handler.Status = HttpStatusCode.Forbidden;
        fixture.Handler.Response = "{\"success\":false,\"errors\":[{\"code\":10000,\"message\":\"Authentication error\"}]}";
        try
        {
            await fixture.Util.GetDatabase("account", "token", "database");
            throw new InvalidOperationException("Expected Cloudflare API failure.");
        }
        catch (D1ApiResponseCommonFailure error)
        {
            await Assert.That(error.ResponseStatusCode).IsEqualTo(403);
        }
    }

    private sealed class Fixture : IDisposable
    {
        public RecordingHandler Handler { get; } = new();
        private readonly System.Net.Http.HttpClient _http;
        private readonly HttpClientRequestAdapter _adapter;
        public StubClientUtil ClientUtil { get; }
        public CloudflareD1Util Util { get; }
        public Fixture()
        {
            _http = new System.Net.Http.HttpClient(Handler);
            _adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: _http);
            ClientUtil = new StubClientUtil(new CloudflareOpenApiClient(_adapter));
            Util = new CloudflareD1Util(ClientUtil);
        }
        public void Dispose() { _adapter.Dispose(); _http.Dispose(); }
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public string? Uri { get; private set; }
        public string? Method { get; private set; }
        public string? Body { get; private set; }
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
        public string Response { get; set; } = "{\"success\":true}";
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Uri = request.RequestUri!.AbsoluteUri;
            Method = request.Method.Method;
            Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(Status) { Content = new StringContent(Response, Encoding.UTF8, "application/json") };
        }
    }

    private sealed class StubClientUtil(CloudflareOpenApiClient client) : ICloudflareClientUtil
    {
        public string? Token { get; private set; }
        public ValueTask<CloudflareOpenApiClient> Get(CancellationToken cancellationToken = default) => ValueTask.FromResult(client);
        public ValueTask<CloudflareOpenApiClient> Get(string apiKey, CancellationToken cancellationToken = default)
        {
            Token = apiKey;
            return ValueTask.FromResult(client);
        }
        public ValueTask<bool> Remove(CancellationToken cancellationToken = default) => ValueTask.FromResult(false);
        public ValueTask<bool> Remove(string apiKey, CancellationToken cancellationToken = default) => ValueTask.FromResult(false);
        public bool RemoveSync(CancellationToken cancellationToken = default) => false;
        public bool RemoveSync(string apiKey, CancellationToken cancellationToken = default) => false;
        public void Dispose() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

