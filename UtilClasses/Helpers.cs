using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ImprovedWorkflow.UtilClasses
{
    public static class Helpers
    {
        private static readonly Dictionary<float, WaitForSeconds> s_waitDictionary = new();

        private static PointerEventData s_eventDataCurrentPosition;
        private static List<RaycastResult> s_results;

        /// <summary>
        /// Gets a cached WaitForSeconds object, to avoid creating new ones every time.
        /// </summary>
        /// <param name="time">time amount</param>
        /// <returns>Cached WaitForSeconds object</returns>
        public static WaitForSeconds GetWait(float time)
        {
            if (s_waitDictionary.TryGetValue(time, out WaitForSeconds wait)) return wait;

            s_waitDictionary[time] = new WaitForSeconds(time);
            return s_waitDictionary[time];
        }

        /// <summary>
        /// Checks if the mouse or finger (for mobile) is over a UI element.
        /// </summary>
        /// <returns>True if pointer is over an ui element</returns>
        public static bool IsOverUI()
        {
            s_eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            s_results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(s_eventDataCurrentPosition, s_results);
            return s_results.Count > 0;
        }

        /// <summary>
        /// Calculates and returns the world position of a canvas element. 
        /// </summary>
        /// <param name="element">UI element to get world position</param>
        /// <param name="camera">Camera to calculate screne point</param>
        /// <returns>World position of an ui element</returns>
        public static Vector3 GetWorldPositionOfCanvasElement(RectTransform element, Camera camera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, camera,
                out Vector3 result);
            return result;
        }

        /// <summary>
        /// Finds the nearest point on a line to a given point.
        /// </summary>
        /// <param name="lineStart">Starting point of the line</param>
        /// <param name="lineEnd">Ending point of the line</param>
        /// <param name="point">Point to search for the nearest point on the line</param>
        /// <returns>The nearest point on the line</returns>
        public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
            float closestPoint = Vector3.Dot(point - lineStart, lineDirection) /
                                 Vector3.Dot(lineDirection, lineDirection);
            return lineStart + closestPoint * lineDirection;
        }

        /// <summary>
        /// Inverse evalution of an animation curve. Searchs for the time in the curve, corresponding to the given value.
        /// </summary>
        /// <param name="curve">Target animation curve</param>
        /// <param name="value">Given value to find the time for in animation curve</param>
        /// <returns>Time for the given value, in animation curve graph</returns>
        public static float InverseEvaluate(this AnimationCurve curve, float value)
        {
            AnimationCurve inverseSpeedCurve = new();
            for (int i = 0; i < curve.length; i++)
            {
                Keyframe inverseKey = new(curve.keys[i].value, curve.keys[i].time);
                inverseSpeedCurve.AddKey(inverseKey);
            }

            return inverseSpeedCurve.Evaluate(value);
        }

        /// <summary>
        /// Inverses a given animation curve. Time becomes value and value becomes time in all key points.
        /// </summary>
        /// <param name="curve">Target animation curve</param>
        /// <returns>Inversed curve</returns>
        public static AnimationCurve GetInverseCurve(this AnimationCurve curve)
        {
            AnimationCurve inverseSpeedCurve = new();
            for (int i = 0; i < curve.length; i++)
            {
                Keyframe inverseKey = new(curve.keys[i].value, curve.keys[i].time);
                inverseSpeedCurve.AddKey(inverseKey);
            }

            return inverseSpeedCurve;
        }
    }
}