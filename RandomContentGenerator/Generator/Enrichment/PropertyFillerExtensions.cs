using RandomContentGenerator.Generator.Enrichment.Implementations;

namespace RandomContentGenerator.Generator.Enrichment;

public static class PropertyFillerExtensions
{
    public static IPropertyFiller MakeOptional(this IPropertyFiller source)
        => new OptionalDecoratorPropertyFiller(source);

    public static List<int> GetRandomInRange(this Random random, int amount, int min, int max)
    {
        // If the odds for duplicate picks is too big, then we just enumerate over the possibilities
        if (amount * 2 > max - min)
        {
            var range = Enumerable.Range(min, max - min).ToArray();
            random.Shuffle(range);

            return range.Take(amount).ToList();
        }

        var set = new HashSet<int>(amount);
        while(set.Count < amount)
        {
            set.Add(random.Next(min, max));
        }

        return [.. set];
    }
}