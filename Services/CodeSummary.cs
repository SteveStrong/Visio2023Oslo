
using FoundryBlazor.Extensions;

namespace Visio2023Foundry.Model;

public class CodeSummary
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Notes { get; set; } = "";
    public string Filename { get; set; } = "";
    public string Language { get; set; } = "C#";
    public string ImageURL { get; set; } = "";
    public string DemoURL { get; set; } = "";
    public string MemeURL { get; set; } = "";

    public void ModifyImageUrl(string path)
    {
        if ( !string.IsNullOrEmpty(ImageURL) && ImageURL.Contains("{Path}"))
        {
            ImageURL = ImageURL.Replace("{Path}", path);
            $"Replace ImageURL {ImageURL}".WriteSuccess();
        } else if ( !string.IsNullOrEmpty(ImageURL) ) {
            $"ImageURL {ImageURL}".WriteInfo();
        }
    }

        public void ModifyMemeUrl(string path)
    {
        if ( !string.IsNullOrEmpty(MemeURL) && MemeURL.Contains("{Path}"))
        {
            MemeURL = MemeURL.Replace("{Path}", path);
            $"Replace ImageURL {MemeURL}".WriteSuccess();
        } else if ( !string.IsNullOrEmpty(MemeURL) ) {
            $"ImageURL {MemeURL}".WriteInfo();
        }
    }
}
