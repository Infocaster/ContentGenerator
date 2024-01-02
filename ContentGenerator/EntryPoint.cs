using Microsoft.Extensions.DependencyInjection;
using ContentGenerator.Generator.Enrichment;
using ContentGenerator.Generator.Enrichment.Implementations;
using ContentGenerator.Generator.Enrichment.Implementations.URLPicker;
using ContentGenerator.Generator.Production;
using ContentGenerator.Menu;
using ContentGenerator.Request;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace ContentGenerator
{
    public class EntryPoint : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddOptions<ContentGeneratorOptions>()
                .BindConfiguration(ContentGeneratorOptions.Section)
                .ValidateDataAnnotations();

            builder.AddManifestFilter<ManifestFilter>();
            builder.AddNotificationHandler<MenuRenderingNotification, ContentMenuHandler>();
            builder.Services.AddScoped<IGenerateContentRequestHandler, GenerateContentRequestHandler>();

            builder.Services.AddSingleton<IRandomContentFactory, RandomContentFactory>();
            builder.Services.AddSingleton<IRandomContentEnricherFactory, RandomContentEnricherFactory>();
            builder.Services.AddSingleton<IPropertyFillerFactory, TextStringPropertyFillerFactory>();

            builder.Services.AddSingleton<IURLPickerLinkGenerator, URLPickerByContentLinkGenerator>();
            builder.Services.AddSingleton<IURLPickerLinkGenerator, URLPickerByMediaLinkGenerator>();
            builder.Services.AddSingleton<IURLPickerLinkGenerator, URLPickerByTextLinkGenerator>();

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
