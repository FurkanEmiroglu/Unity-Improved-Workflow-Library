using System.Collections.Generic;
using UnityEngine;

namespace ImprovedWorkflow.Extensions
{
    public static class CollectionExtensions
    {
        public static T Pop<T>(this List<T> t)
        {
            T lastItem = t[^1];
            t.Remove(lastItem);
            return lastItem;
        }

        public static T Dequeue<T>(this List<T> t)
        {
            T firstItem = t[0];
            t.Remove(firstItem);
            return firstItem;
        }

        public static T GetRandom<T>(this IList<T> t)
        {
            return t[Random.Range(0, t.Count)];
        }

        public static T GetRandom<T>(this IList<T> t, bool removeFromList)
        {
            T randomItem = GetRandom(t);
            if (removeFromList) t.Remove(randomItem);
            return randomItem;
        }
    }
}