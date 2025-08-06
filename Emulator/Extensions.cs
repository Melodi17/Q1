namespace Q1.Emulator;

using System.Collections.Generic;

public static class Extensions
{
    public static IEnumerable<int> AllIndexesOf<T>(this IEnumerable<T> arr, T q)
    {
        var indexes = new List<int>();
        int index = 0;

        foreach (var item in arr)
        {
            if (EqualityComparer<T>.Default.Equals(item, q))
            {
                indexes.Add(index);
            }
            index++;
        }

        return indexes;
    }
}