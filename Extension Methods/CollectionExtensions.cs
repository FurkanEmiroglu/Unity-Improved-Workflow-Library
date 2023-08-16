using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IW.ExtensionMethods
{
    /// <summary>
    ///     Contains extension methods for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Removes the last item from the list and returns it.
        /// </summary>
        /// <param name="t">A generic list to pop an item</param>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <returns>Popped item</returns>
        public static T Pop<T>(this List<T> t)
        {
            T lastItem = t[^1];
            t.Remove(lastItem);
            return lastItem;
        }

        /// <summary>
        ///     Removes the first item from the list and returns it.
        /// </summary>
        /// <param name="t">A generic list to Dequeue an item</param>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <returns>Dequeued item</returns>
        public static T Dequeue<T>(this List<T> t)
        {
            T firstItem = t[0];
            t.Remove(firstItem);
            return firstItem;
        }

        /// <summary>
        ///     Picks a random item from the list and returns it. Item still remains in the list.
        ///     You can remove it by passing true to removeFromList parameter.
        /// </summary>
        /// <param name="t">A generic list to get a random item from</param>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <returns>Randomly picked item</returns>
        public static T GetRandom<T>(this IList<T> t)
        {
            return t[Random.Range(0, t.Count)];
        }

        /// <summary>
        ///     Picks a random item from the list and returns it. Item will be removed form the list based on the
        ///     removeFormList parameter.
        /// </summary>
        /// <param name="t">A generic list to get a random item from</param>
        /// <param name="removeFromList">should item stay in the list after returning</param>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <returns>Randomly picked item</returns>
        public static T GetRandom<T>(this IList<T> t, bool removeFromList)
        {
            T randomItem = GetRandom(t);
            if (removeFromList) t.Remove(randomItem);
            return randomItem;
        }

        /// <summary>
        ///     Picks a weighted randomized item from the list and returns it. The item will remain in the list.
        /// </summary>
        /// <param name="items">Collection to pick from</param>
        /// <param name="weights">Weights corresponding to items</param>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <returns>Picked item</returns>
        /// <exception cref="Exception"></exception>
        public static T GetWeightedRandom<T>(IList<T> items, IList<float> weights)
        {
            if (items == null || weights == null || items.Count == 0 || items.Count != weights.Count)
                Debug.Log("Invalid Arguments");

            double totalWeight = 0;
            foreach (double weight in weights)
            {
                if (weight < 0) Debug.Log("weight cant be negative");
                totalWeight += weight;
            }

            float randomValue = (float)(new System.Random().NextDouble() * totalWeight);

            for (int i = 0; i < items.Count; i++)
            {
                randomValue -= weights[i];
                if (randomValue <= 0) return items[i];
            }

            throw new Exception("Weights must sum to a positive value");
        }

        /// <summary>
        ///     Removes a random item from the list and returns it.
        /// </summary>
        /// <param name="list">Collection to remove randomized item</param>
        /// <typeparam name="T">Generic type</typeparam>
        /// <returns>Removed item</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new IndexOutOfRangeException("Cannot remove a random item from an empty list");
            int index = Random.Range(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        /// <summary>
        ///     Shuffles a list based on the Fisher-Yates algorithm.
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static IEnumerable<T> Examine<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T obj in source)
            {
                action(obj);
                yield return obj;
            }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T obj in source)
                action(obj);
            return source;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int num = 0;
            foreach (T obj in source)
                action(obj, num++);
            return source;
        }

        public static IEnumerable<T> Convert<T>(this IEnumerable source, Func<object, T> converter)
        {
            foreach (object obj in source)
                yield return converter(obj);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(source, comparer);
        }

        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> source, Func<T> prepend)
        {
            yield return prepend();
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> source, T prepend)
        {
            yield return prepend;
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> source, IEnumerable<T> prepend)
        {
            foreach (T obj in prepend)
                yield return obj;
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> source, bool condition, Func<T> prepend)
        {
            if (condition)
                yield return prepend();
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> source, bool condition, T prepend)
        {
            if (condition)
                yield return prepend;
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> source, bool condition, IEnumerable<T> prepend)
        {
            if (condition)
                foreach (T obj in prepend)
                    yield return obj;
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> source, Func<bool> condition, Func<T> prepend)
        {
            if (condition())
                yield return prepend();
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> source, Func<bool> condition, T prepend)
        {
            if (condition())
                yield return prepend;
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> source, Func<bool> condition, IEnumerable<T> prepend)
        {
            if (condition())
                foreach (T obj in prepend)
                    yield return obj;
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> source, Func<IEnumerable<T>, bool> condition, Func<T> prepend)
        {
            if (condition(source))
                yield return prepend();
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> source, Func<IEnumerable<T>, bool> condition, T prepend)
        {
            if (condition(source))
                yield return prepend;
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> source, Func<IEnumerable<T>, bool> condition, IEnumerable<T> prepend)
        {
            if (condition(source))
                foreach (T obj in prepend)
                    yield return obj;
            foreach (T obj in source)
                yield return obj;
        }

        public static IEnumerable<T> AppendWith<T>(this IEnumerable<T> source, Func<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            yield return append();
        }

        public static IEnumerable<T> AppendWith<T>(this IEnumerable<T> source, T append)
        {
            foreach (T obj in source)
                yield return obj;
            yield return append;
        }

        public static IEnumerable<T> AppendWith<T>(this IEnumerable<T> source, IEnumerable<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            foreach (T obj in append)
                yield return obj;
        }

        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> source, bool condition, Func<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition)
                yield return append();
        }

        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> source, bool condition, T append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition)
                yield return append;
        }

        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> source, bool condition, IEnumerable<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition)
                foreach (T obj in append)
                    yield return obj;
        }

        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> source, Func<bool> condition, Func<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition())
                yield return append();
        }

        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> source, Func<bool> condition, T append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition())
                yield return append;
        }

        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> source, Func<bool> condition, IEnumerable<T> append)
        {
            foreach (T obj in source)
                yield return obj;
            if (condition())
                foreach (T obj in append)
                    yield return obj;
        }

        /// <summary>
        ///     Returns and casts only the items of type <typeparamref name="T" />.
        /// </summary>
        /// <param name="source">The collection.</param>
        public static IEnumerable<T> FilterCast<T>(this IEnumerable source)
        {
            foreach (object obj1 in source)
                if (obj1 is T obj2)
                    yield return obj2;
        }

        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> range)
        {
            foreach (T obj in range)
                hashSet.Add(obj);
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static void Populate<T>(this IList<T> list, T item)
        {
            int count = list.Count;
            for (int index = 0; index < count; ++index)
                list[index] = item;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
        {
            if (list is List<T>)
                ((List<T>)list).AddRange(collection);
            else
                foreach (T obj in collection)
                    list.Add(obj);
        }

        public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
        {
            if (list is List<T>)
            {
                ((List<T>)list).Sort(comparison);
            }
            else
            {
                List<T> objList = new(list);
                objList.Sort(comparison);
                for (int index = 0; index < list.Count; ++index)
                    list[index] = objList[index];
            }
        }

        public static void Sort<T>(this IList<T> list)
        {
            if (list is List<T>)
            {
                ((List<T>)list).Sort();
            }
            else
            {
                List<T> objList = new(list);
                objList.Sort();
                for (int index = 0; index < list.Count; ++index)
                    list[index] = objList[index];
            }
        }
    }