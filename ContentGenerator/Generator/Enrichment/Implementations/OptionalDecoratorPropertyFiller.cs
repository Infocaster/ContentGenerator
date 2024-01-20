/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
namespace ContentGenerator.Generator.Enrichment.Implementations;

public class OptionalDecoratorPropertyFiller : IPropertyFiller
{
    private readonly IPropertyFiller decoratee;

    public OptionalDecoratorPropertyFiller(IPropertyFiller decoratee)
    {
        this.decoratee = decoratee;
    }

    public IPropertySink FillProperties(IPropertySink content, IGeneratorContext context)
    {
        var ratio = context.GetOptionalRatio();

        Random rnd = context.GetRandom();
        if (rnd.NextDouble() < ratio)
        {
            content = decoratee.FillProperties(content, context);
        }

        return content;
    }
}
