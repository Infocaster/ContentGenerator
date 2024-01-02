using ContentGenerator.Generator.Enrichment.Implementations;

namespace ContentGenerator.Generator.Enrichment;

public static class PropertyFillerExtensions
{
    public static IPropertyFiller MakeOptional(this IPropertyFiller source)
        => new OptionalDecoratorPropertyFiller(source);

    public static List<int> GetRandomInRange(this Random random, int amount, Range range)
        => random.GetRandomInRange(amount, range.Start.Value, range.End.Value);

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

    public static IEnumerable<T> SelectByRandomIndexesFromRange<T>(this Random rnd, Range indexRange, Range amountRange, Func<int, T> transformFunc)
    {
        var amount = rnd.Next(amountRange.Start.Value, Math.Min(amountRange.End.Value, indexRange.End.Value - indexRange.Start.Value));
        if (amount <= 0)
        {
            return Enumerable.Empty<T>();
        }

        var indexes = rnd.GetRandomInRange(amount, indexRange);

        return indexes.Select(transformFunc);
    }
}