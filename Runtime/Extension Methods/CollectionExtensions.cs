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
        
        public static T GetWeightedRandom<T>(IList<T> items, IList<float> weights)
        {
            if (items == null || weights == null || items.Count == 0 || items.Count != weights.Count)
                Debug.Log("Invalid Arguments");

            double totalWeight = 0;
            foreach (double weight in weights)
            {
                if (weight < 0)
                {
                    Debug.Log("weight cant be negative");
                }
                totalWeight += weight;
            }

            float randomValue = (float)(new System.Random().NextDouble() * totalWeight);

            for (int i = 0; i < items.Count; i++)
            {
                randomValue -= weights[i];
                if (randomValue <= 0)
                {
                    return items[i];
                }
            }
            
            throw new System.Exception("Weights must sum to a positive value");
        }
        
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
            int index = UnityEngine.Random.Range(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }
        
        public static void Shuffle<T>(IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}