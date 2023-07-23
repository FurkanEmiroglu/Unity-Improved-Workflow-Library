using UnityEditor;
using UnityEngine;

namespace IW.EditorExtensions.ContextMenus
{
    public static class TransformContextExtensions
    {
        [MenuItem ("CONTEXT/Transform/Snap to Ground", default, 100)]
        public static void SnapToGround(MenuCommand command)
        {
            foreach (Transform t in Selection.transforms)
            {
                RaycastHit hit;
                Ray ray = new Ray(t.position, Vector3.down);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector3 position = hit.point;
                    t.position = position;
                }
            }
        }

        [MenuItem("CONTEXT/Transform/Random Rotation", default, 100)]
        public static void RandomRotationCustom(MenuCommand command) 
        {
            RandomRotationPopup.Open();
        }
    }

    public class RandomRotationPopup : EditorWindow
    {
        private const int size_x = 300;
        private const int size_y = 115;

        private Vector3 m_minRotation;
        private Vector3 m_maxRotation;
        
        public static void Open()
        {
            RandomRotationPopup window = GetWindow<RandomRotationPopup>();
            
            window.titleContent = new GUIContent("Random Rotation");
            window.minSize = new Vector2(size_x, size_y);
            window.maxSize = new Vector2(size_x, size_y);
            window.Show();
        }

        private void OnGUI()
        {
            m_minRotation = EditorGUILayout.Vector3Field("Min Rotation", m_minRotation);
            m_maxRotation = EditorGUILayout.Vector3Field("Max Rotation", m_maxRotation);
            
            if (GUILayout.Button("Give Random Rotation"))
            {
                foreach (Transform t in Selection.transforms)
                {
                    if (t != null)
                    {
                        t.localRotation = Quaternion.Euler(GetRandomRotationBetween());
                    }
                    else
                    {
                        Debug.LogWarning("Object is not a transform");
                    }
                }
            }
        }
        
        private Vector3 GetRandomRotationBetween()
        {
            Vector3 rotation = new Vector3
            {
                x = Random.Range(m_minRotation.x, m_maxRotation.x),
                y = Random.Range(m_minRotation.y, m_maxRotation.y),
                z = Random.Range(m_maxRotation.z, m_maxRotation.z)
            };

            return rotation;
        }
    }
}