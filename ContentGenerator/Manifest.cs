using System.Reflection;
using Umbraco.Cms.Core.Manifest;

namespace ContentGenerator;

public class ManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.1",
            PackageName = "Content Generator",
            Scripts = new [] {
                Defaults.PluginBasePath + "/script.iife.js"
            },
            BundleOptions = BundleOptions.None
        });
    }
}