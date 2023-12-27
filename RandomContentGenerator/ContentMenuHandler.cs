using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Notifications;

namespace RandomContentGenerator;

public class ContentMenuHandler
: INotificationHandler<MenuRenderingNotification>
{
    public void Handle(MenuRenderingNotification notification)
    {
        if (!string.Equals(notification.TreeAlias, Constants.Trees.Content, StringComparison.Ordinal))
            return;

        MenuItem pluginMenuItem = new ("randomContentGenerator", "Generate random content");
        pluginMenuItem.LaunchDialogView(Defaults.ContextMenuViewPath, "Generate random content pages");
        notification.Menu.Items.Add(pluginMenuItem);
    }
}
