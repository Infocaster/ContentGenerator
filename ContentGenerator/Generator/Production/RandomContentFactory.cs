/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace ContentGenerator.Generator.Production;

public interface IRandomContentFactory
{
    IContent CreateContent(ContentFactoryContext context);
}

public class RandomContentFactory : IRandomContentFactory
{
    private readonly IContentService contentService;

    public RandomContentFactory(IContentService contentService)
    {
        this.contentService = contentService;
    }

    public IContent CreateContent(ContentFactoryContext context)
    {
        FieldOptionsTextWords fieldOptions = new FieldOptionsTextWords() { Seed = context.GeneratorContext.GetSeed() };
        var loremipsumrandomizer = RandomizerFactory.GetRandomizer(fieldOptions);
        var name = loremipsumrandomizer.Generate()!;

        var result = contentService.Create(name, context.Parent.Id, context.TargetContentType, context.UserId);

        context.GeneratorContext.SetSeed(fieldOptions.Seed.Value);
        return result;
    }
}
