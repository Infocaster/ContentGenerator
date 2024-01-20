/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
using ContentGenerator.Generator.Enrichment.Implementations;

namespace ContentGenerator.Generator.Enrichment;

public static class PropertyFillerExtensions
{
    public static IPropertyFiller MakeOptional(this IPropertyFiller source)
        => new OptionalDecoratorPropertyFiller(source);

    public static void Shuffle<T> (this Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            (array[k], array[n]) = (array[n], array[k]);
        }
    }

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

        return set.ToList();
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