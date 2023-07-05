using System.Collections.Generic;
using UnityEngine;

namespace ImprovedWorkflow.UtilClasses
{
    public static class NumericalExtensions
    {
        /// <summary>
        ///     Remaps the values of the collection from old range to new range.
        /// </summary>
        /// <param name="valueList">List of values to remap</param>
        /// <param name="oldMin">old minimum limit of range</param>
        /// <param name="oldMax">old maximum limit of range</param>
        /// <param name="newMin">new maximum limit of range</param>
        /// <param name="newMax">new maximum limit of range</param>
        public static void LinearRemap(this IList<float> valueList, float oldMin, float oldMax, float newMin, float newMax)
        {
            for (int index = 0; index < valueList.Count; index++)
                valueList[index] = valueList[index].LinearRemap(oldMin, oldMax, newMin, newMax);
        }

        /// <summary>
        ///     Remaps a value from old range to new range.
        /// </summary>
        /// <param name="value">Value to remap</param>
        /// <param name="oldMin">old minimum limit of range</param>
        /// <param name="oldMax">old maximum limit of range</param>
        /// <param name="newMin">new maximum limit of range</param>
        /// <param name="newMax">new maximum limit of range</param>
        /// <returns>Remapped value</returns>
        public static float LinearRemap(this float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return value = (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
        }

        /// <summary>
        ///     Assigns a random sign to the value based on the negativeProbability.
        /// </summary>
        /// <param name="value">Value to manipulate</param>
        /// <param name="negativeProbability">Probability of having negative sign, must be in range (0,1)</param>
        /// <returns>Value with random sign</returns>
        public static int WithRandomSign(this int value, float negativeProbability = 0.5f)
        {
            return Random.value < negativeProbability ? -value : value;
        }

        /// <summary>
        ///     Assigns a random sign to the value based on the negativeProbability.
        /// </summary>
        /// <param name="value">Value to manipulate</param>
        /// <param name="negativeProbability">Probability of having negative sign, must be in range (0,1)</param>
        /// <returns>Value with random sign</returns>
        public static float WithRandomSign(this float value, float negativeProbability = 0.5f)
        {
            return Random.value < negativeProbability ? -value : value;
        }
    }
}