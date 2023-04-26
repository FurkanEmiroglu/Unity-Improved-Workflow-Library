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

        public static WaitForSeconds GetWait(float time)
        {
            if (s_waitDictionary.TryGetValue(time, out WaitForSeconds wait)) return wait;

            s_waitDictionary[time] = new WaitForSeconds(time);
            return s_waitDictionary[time];
        }

        public static bool IsOverUI()
        {
            s_eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            s_results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(s_eventDataCurrentPosition, s_results);
            return s_results.Count > 0;
        }

        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element, Camera camera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, camera,
                out Vector3 result);
            return result;
        }

        public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
            float closestPoint = Vector3.Dot(point - lineStart, lineDirection) /
                                 Vector3.Dot(lineDirection, lineDirection);
            return lineStart + closestPoint * lineDirection;
        }

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