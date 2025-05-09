using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static void Shuffle<T>(this IEnumerable<T> source)
    {
        var array = source as T[] ?? source.ToArray();
        var n = array.Length;

        for (var i = 0; i < n - 1; i++)
        {
            var k = Random.Range(0, n - 1);
            (array[i], array[k]) = (array[k], array[i]);
        }
    }
}