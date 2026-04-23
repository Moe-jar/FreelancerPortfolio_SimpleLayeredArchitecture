using System.Text.RegularExpressions;

namespace FreelancerPortfolio.Utilities;

public static class SlugHelper
{
    public static string GenerateSlug(string text)
    {
        var slug = text.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }
}
