namespace RandomContentGenerator.Generator;

public static class GeneratorContextExtensions
{
    private const string seedKey = "seed";
    private const string optionalRatioKey = "optionalRatio";
    private const string recursionLevelKey = "recursionLevel";

    public static int GetSeed(this IGeneratorContext context)
        => context.Get<int>(seedKey);

    public static void SetSeed(this IGeneratorContext context, int seed)
        => context.Set(seedKey, seed);

    public static Random GetRandom(this IGeneratorContext context)
    {
        var rnd = new Random(context.GetSeed());
        context.SetSeed(rnd.Next());

        return rnd;
    }

    public static double GetOptionalRatio(this IGeneratorContext context)
        => context.Get<double>(optionalRatioKey);

    public static void SetOptionalRatio(this IGeneratorContext context, double ratio)
        => context.Set(optionalRatioKey, ratio);

    private static void EnsureRecursionLevel(this IGeneratorContext context)
    {
        if (context[recursionLevelKey] is null) context.SetRecursionLevel(0);
    }

    public static int GetRecursionLevel(this IGeneratorContext context)
    {
        context.EnsureRecursionLevel();
        return context.Get<int>(recursionLevelKey);
    }

    public static void SetRecursionLevel(this IGeneratorContext context, int level)
        => context.Set(recursionLevelKey, level);

    public static void IncreaseRecursionLevel(this IGeneratorContext context)
    {
        context.EnsureRecursionLevel();
        context.SetRecursionLevel(context.GetRecursionLevel() + 1);
    }

    public static void DecreaseRecursionLevel(this IGeneratorContext context)
    {
        context.EnsureRecursionLevel();
        context.SetRecursionLevel(context.GetRecursionLevel() - 1);
    }

    public static bool IsMaxRecursionReached(this IGeneratorContext context, int maxLevel)
    {
        context.EnsureRecursionLevel();
        return context.GetRecursionLevel() >= maxLevel;
    }
}