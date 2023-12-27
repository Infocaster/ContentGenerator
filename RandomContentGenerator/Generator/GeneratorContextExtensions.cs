namespace RandomContentGenerator.Generator;

public static class GeneratorContextExtensions
{
    private const string seedKey = "seed";
    private const string optionalRatioKey = "optionalRatio";

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
}