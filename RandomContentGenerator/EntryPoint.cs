using Microsoft.Extensions.DependencyInjection;
using RandomContentGenerator.Generator.Enrichment;
using RandomContentGenerator.Generator.Enrichment.Implementations;
using RandomContentGenerator.Generator.Production;
using RandomContentGenerator.Request;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace RandomContentGenerator
{
    public class EntryPoint : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddManifestFilter<ManifestFilter>();
            builder.AddNotificationHandler<MenuRenderingNotification, ContentMenuHandler>();
            builder.Services.AddScoped<IGenerateContentRequestHandler, GenerateContentRequestHandler>();

            builder.Services.AddSingleton<IRandomContentFactory, RandomContentFactory>();
            builder.Services.AddSingleton<IRandomContentEnricherFactory, RandomContentEnricherFactory>();
            builder.Services.AddSingleton<IPropertyFillerFactory, TextStringPropertyFillerFactory>();

            builder.WithCollectionBuilder<FillerCollectionBuilder>()
                .Append<TextStringPropertyFillerFactory>()
                .Append<TextAreaPropertyFillerFactory>()
                .Append<SwitchPropertyFillerFactory>()
                .Append<BlocklistPropertyFillerFactory>()
                .Append<ContentPickerPropertyFillerFactory>()
                .Append<NumericPropertyFillerFactory>()
                .Append<MediaPicker3PropertyFillerFactory>()
                .Append<RTEPropertyFillerFactory>()
                .Append<DateTimePropertyFillerFactory>()
                .Append<MultiNodeTreePickerPropertyFillerFactory>()
                .Append<URLPickerPropertyFillerFactory>();
            builder.Services.AddSingleton<IFillerCollection>(sp => sp.GetRequiredService<FillerCollection>());
        }
    }
}
