/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
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