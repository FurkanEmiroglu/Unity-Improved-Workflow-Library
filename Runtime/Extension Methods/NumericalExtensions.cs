using System.Collections.Generic;

namespace ImprovedWorkflow.Extensions
{
    public static class NumericalExtensions
    {
        public static void LinearRemap(this IList<float> valueList, float oldMin,float oldMax,float newMin,float newMax)
        {
            for (int index = 0; index < valueList.Count; index++)
            {
                valueList[index] = valueList[index].LinearRemap(oldMin,oldMax,newMin,newMax);
            }
        }
        
        public static float LinearRemap(this float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return value = (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
        }
        
        public static int WithRandomSign(this int value, float negativeProbability = 0.5f)
        {
            return UnityEngine.Random.value < negativeProbability ? -value : value;
        }
        
        public static float WithRandomSign(this float value, float negativeProbability = 0.5f)
        {
            return UnityEngine.Random.value < negativeProbability ? -value : value;
        }
    }
}