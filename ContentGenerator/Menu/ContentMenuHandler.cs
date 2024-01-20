/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Notifications;

namespace ContentGenerator.Menu;

public class ContentMenuHandler(IOptionsMonitor<ContentGeneratorOptions> contentGeneratorOptions)
: INotificationHandler<MenuRenderingNotification>
{
    public void Handle(MenuRenderingNotification notification)
    {
        if (!string.Equals(notification.TreeAlias, Constants.Trees.Content, StringComparison.Ordinal))
            return;

        if (!contentGeneratorOptions.CurrentValue.Enabled)
            return;

        if (!NodeIsElligible(notification))
            return;

        MenuItem pluginMenuItem = new("ContentGenerator", "Generate random content")
        {
            Icon = "fire color-deep-orange",
        };
        pluginMenuItem.LaunchDialogView(Defaults.ContextMenuViewPath, "Generate random content pages");
        notification.Menu.Items.Add(pluginMenuItem);
    }

    private static bool NodeIsElligible(MenuRenderingNotification notification)
    {
        return int.TryParse(notification.NodeId, out var nodeIdInt)
            && (nodeIdInt > 0 || nodeIdInt == Constants.System.Root);
    }
}
