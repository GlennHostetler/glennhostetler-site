using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GlennSite.Tests;

/// <summary>
/// Starts the real site in memory and requests pages from it, the same way a browser would.
/// </summary>
public class SiteTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SiteTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/index.html")]
    [InlineData("/how-this-site-works.html")]
    public async Task Pages_return_200_html(string path)
    {
        var response = await _client.GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData("/files/Glenn_Hostetler_Resume_Designed.pdf", "application/pdf")]
    [InlineData("/files/Glenn_Hostetler_Resume_ATS.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    public async Task Resume_files_are_served(string path, string contentType)
    {
        var response = await _client.GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(contentType, response.Content.Headers.ContentType?.MediaType);
        Assert.True(response.Content.Headers.ContentLength > 1000, "The resume file is empty or truncated.");
    }

    [Fact]
    public async Task Architecture_diagram_is_served_as_svg()
    {
        var response = await _client.GetAsync("/img/architecture.svg");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/svg+xml", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Unknown_path_returns_404()
    {
        var response = await _client.GetAsync("/this-page-does-not-exist.html");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("/index.html")]
    [InlineData("/how-this-site-works.html")]
    public async Task Pages_have_title_language_and_alt_text(string path)
    {
        var html = await _client.GetStringAsync(path);

        Assert.Matches(@"<html[^>]*\blang=""[a-z-]+""", html);
        Assert.Matches(@"<title>[^<]+</title>", html);

        // Every image needs an alt attribute (it may be empty for purely decorative images).
        foreach (Match img in Regex.Matches(html, @"<img\b[^>]*>", RegexOptions.IgnoreCase))
        {
            Assert.Matches(@"\balt=""", img.Value);
        }
    }

    [Theory]
    [InlineData("/index.html")]
    [InlineData("/how-this-site-works.html")]
    public async Task Local_links_and_assets_all_exist(string page)
    {
        var html = await _client.GetStringAsync(page);
        var pageUri = new Uri("http://localhost" + page);

        var targets = Regex.Matches(html, @"\b(?:href|src)=""([^""]+)""", RegexOptions.IgnoreCase)
            .Select(m => m.Groups[1].Value)
            .Where(IsLocal)
            .Select(v => new Uri(pageUri, v).AbsolutePath)
            .Distinct()
            .ToList();

        Assert.NotEmpty(targets);

        var missing = new List<string>();
        foreach (var target in targets)
        {
            var response = await _client.GetAsync(target);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                missing.Add($"{target} -> {(int)response.StatusCode}");
            }
        }

        Assert.True(missing.Count == 0, $"Broken links on {page}: " + string.Join(", ", missing));
    }

    // External links, mail links, and in-page anchors are not checked here.
    private static bool IsLocal(string value) =>
        !value.StartsWith("#")
        && !value.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
        && !value.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
        && !value.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
        && !value.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)
        && !value.StartsWith("//");
}
