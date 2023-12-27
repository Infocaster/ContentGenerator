export function ensureExists<T>(obj: T | undefined): asserts obj is T
{
    if (!obj) throw new Error("The object does not exist");
}