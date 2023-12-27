using Umbraco.Cms.Core.Manifest;

namespace RandomContentGenerator;

public class ManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            Version = "1.0.0",
            PackageName = "Random Content Generator",
            Scripts = new []{
                Defaults.PluginBasePath + "/script.iife.js"
            }
        });
    }
}