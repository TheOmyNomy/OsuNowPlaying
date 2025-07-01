using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.MapFallback(context =>
{
    string url = "https://github.com/TheOmyNomy/OsuNowPlaying";
    context.Response.Redirect(url);

    return Task.CompletedTask;
});

string? baseUrl = app.Configuration.GetValue<string>("BaseUrl");

if (string.IsNullOrWhiteSpace(baseUrl))
    throw new Exception("\"BaseUrl\" must be set.");

if (baseUrl.EndsWith('/'))
    baseUrl = baseUrl.Remove(baseUrl.Length - 1);

app.MapGet("/request-token", context =>
{
    string? clientId = app.Configuration.GetValue<string>("ClientId");

    if (string.IsNullOrWhiteSpace(clientId))
        throw new Exception("\"ClientId\" must be set.");

    if (baseUrl.EndsWith('/'))
        baseUrl = baseUrl.Remove(baseUrl.Length - 1);

    const string scope = "user:bot";
    Guid state = Guid.NewGuid();

    StringBuilder uriBuilder = new StringBuilder("https://id.twitch.tv/oauth2/authorize");

    uriBuilder.Append("?client_id=").Append(clientId);
    uriBuilder.Append("&redirect_uri=").Append(baseUrl).Append("/token");
    uriBuilder.Append("&response_type=token");
    uriBuilder.Append("&scope=").Append(scope);
    uriBuilder.Append("&state=").Append(state);

    string url = uriBuilder.ToString();
    context.Response.Redirect(url);

    return Task.CompletedTask;
});

app.MapGet("/token", async context =>
{
    string html;

    string? error = context.Request.Query["error"];

    if (!string.IsNullOrWhiteSpace(error))
    {
        string? errorDescription = context.Request.Query["error_description"];

        html = File.ReadAllText("wwwroot/templates/error.html")
            .Replace("{{ERROR}}", error)
            .Replace("{{ERROR_DESCRIPTION}}", errorDescription)
            .Replace("{{REQUEST_TOKEN_URL}}", baseUrl + "/request-token");
    }
    else
        html = File.ReadAllText("wwwroot/templates/token.html");

    context.Response.ContentType = "text/html";

    await using StreamWriter writer = new StreamWriter(context.Response.Body);
    await writer.WriteAsync(html);
    await writer.FlushAsync();
});

app.Run();