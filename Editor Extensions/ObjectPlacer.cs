using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

using static UnityEngine.Mathf;

namespace IW.EditorExtensions
{
    public class ObjectPlacer : EditorWindow
    {
        #region Opener

        [MenuItem("Tools/Workflow/Scene Tools/Object Placer #z")]
        private static void OpenWindow()
        {
            var window = GetWindow<ObjectPlacer>();
            window.titleContent = new GUIContent("Object Placing Tool");
            window.Show();
        }

        #endregion

        // generic varyings
        private GameObject m_prefab;
        private Editor m_prefabEditor;
        private Texture2D m_previewBackgroundTexture;
        private LevelDesignMode m_levelDesignMode;
        
        private string m_parentName = "Parent";
        private int m_itemIndex;

        private GameObject m_lastParent;

        private List<GameObject> m_allItems;
        private List<GameObject> m_lastItems;
        
        // circular varyings
        private int m_circleCount;
        private List<int> m_circleIntensity;
        private List<float> m_circleRadiuses;
        private Vector2 m_scrollPos;

        // rectangular varyings
        private int m_rowCount;
        private int m_columnCount;
        private float m_rowDistance;
        private float m_columnDistance;
        
        // triangular varyings
        private int m_baseCount;
        private float m_distanceBetween;

        private void OnEnable()
        {
            m_allItems = new List<GameObject>();
            m_lastItems = new List<GameObject>();
            m_levelDesignMode = LevelDesignMode.Circular;

            m_circleIntensity = new List<int>();
            m_circleRadiuses = new List<float>();
            m_scrollPos = new Vector2();
        }

        private void OnGUI()
        {
            DrawDesignSelection();
            
            DrawVariables();
            
            DrawButtons();

            DrawInteractivePreview();
        }

        private void DrawDesignSelection()
        {
            EditorGUI.BeginChangeCheck();
   
            m_prefab = (GameObject) EditorGUILayout.ObjectField(m_prefab, typeof(GameObject), true, GUILayout.MinHeight(50));
   
            if(EditorGUI.EndChangeCheck())
            {
                if(m_prefabEditor != null) DestroyImmediate(m_prefabEditor);
            }

            EditorGUILayout.BeginHorizontal();

            // change editor color
            GUI.backgroundColor = m_levelDesignMode == LevelDesignMode.Circular ? Color.green : new Color(0.6f, 0.6f, 0.6f);
            
            if (GUILayout.Button("Circular"))
                m_levelDesignMode = LevelDesignMode.Circular;

            GUI.backgroundColor = m_levelDesignMode == LevelDesignMode.Rectangular ? Color.green : new Color(0.6f, 0.6f, 0.6f);
            if (GUILayout.Button("Rectangular"))
                m_levelDesignMode = LevelDesignMode.Rectangular;

            GUI.backgroundColor = m_levelDesignMode == LevelDesignMode.Triangle ? Color.green : new Color(0.6f, 0.6f, 0.6f);
            if (GUILayout.Button("Triangular"))
                m_levelDesignMode = LevelDesignMode.Triangle;
            
            GUI.backgroundColor = Color.gray;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawVariables()
        {
            m_parentName = EditorGUILayout.TextField("Parent Name", m_parentName);

            switch (m_levelDesignMode)
            {
                case LevelDesignMode.Circular:
                    m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Height(250));
                    EditorGUI.BeginChangeCheck();
                    
                    m_circleCount = EditorGUILayout.IntField("Circle Count", m_circleCount);
                    m_circleCount = Max(0, m_circleCount);
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        List<int> newIntensityList = new (m_circleCount);
                        List<float> newRadiusList = new (m_circleCount);
                        
                        for (int i = 0; i < m_circleCount; i++)
                        {
                            newIntensityList.Add(0);
                            newRadiusList.Add(0);
                        }

                        for (int index = 0; index < m_circleIntensity.Count; index++)
                        {
                            int intensity = m_circleIntensity[index];
                            float radius = m_circleRadiuses[index];

                            if (newIntensityList.Count > index)
                            {
                                newIntensityList[index] = intensity;
                                newRadiusList[index] = radius;
                            }
                        }

                        m_circleIntensity = newIntensityList;
                        m_circleRadiuses = newRadiusList;
                    }
                    
                    DrawLine();
                    for (int i = 0; i < m_circleCount; i++)
                    {
                        m_circleRadiuses[i] = EditorGUILayout.FloatField($"Circle {i+1} Radius", m_circleRadiuses[i]);
                        m_circleIntensity[i] = EditorGUILayout.IntField($"Circle {i+1} Item Count", m_circleIntensity[i]);
                        DrawLine();
                    }
                    
                    EditorGUILayout.EndScrollView();
                    break;
                case LevelDesignMode.Rectangular:
                    DrawLine();
                    m_rowCount = EditorGUILayout.IntField("Row Count", m_rowCount);
                    m_columnCount = EditorGUILayout.IntField("Column Count", m_columnCount);
                    DrawLine();
                    m_rowDistance = EditorGUILayout.FloatField("Row Distance", m_rowDistance);
                    m_columnDistance = EditorGUILayout.FloatField("Column Distance", m_columnDistance);
                    break; 
                
                case LevelDesignMode.Triangle:
                    DrawLine();
                    m_baseCount = EditorGUILayout.IntField("Base Length", m_baseCount);
                    m_distanceBetween = EditorGUILayout.FloatField("Distance Between", m_distanceBetween);
                    break;
            }
        }

        private void DrawButtons()
        {
            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("Generate", GUILayout.Height(50)))
                Generate();

            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Last", GUILayout.Height(50)))
                ClearLast();

            if (GUILayout.Button("Clear All", GUILayout.Height(50)))
                ClearAll();
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawInteractivePreview()
        {
            GUIStyle bgColor = new GUIStyle();
   
            bgColor.normal.background = m_previewBackgroundTexture;
   
            if (m_prefab != null)
            {
                if (m_prefabEditor == null)
                    m_prefabEditor = Editor.CreateEditor(m_prefab);
                
                m_prefabEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect (200,200),bgColor);
            }
        }

        private void OnValidate()
        {
            m_circleCount = Max(0, m_circleCount);
        }

        private void ClearLast()
        {
            if (m_lastParent != null) DestroyImmediate(m_lastParent);
            m_lastItems.Clear();
        }

        private void ClearAll()
        {
            m_allItems.Clear();
            m_lastItems.Clear();
            if (m_lastParent != null) DestroyImmediate(m_lastParent);
            
            GameObject[] parents = GameObject.FindObjectsOfType<Transform>().Where(x => x.name.Contains(m_parentName)).Select(x => x
                                                 .gameObject)
                                             .ToArray();

            for (int i = 0; i < parents.Length; i++)
            {
                DestroyImmediate(parents[i].gameObject);
            }
        }

        private void Generate()
        {
            m_lastItems.Clear();
            m_lastParent = new GameObject(m_parentName);
            
            switch (m_levelDesignMode)
            {
                case LevelDesignMode.Rectangular:
                    GenerateRectangular();
                    break;
                case LevelDesignMode.Circular:
                    GenerateCircular();
                    break;
                case LevelDesignMode.Triangle:
                    GenerateTriangular();
                    break;
            }
        }
        
        private async void GenerateRectangular()
        {
            for (int i = 0; i < m_rowCount; i++)
            {
                for (int j = 0; j < m_columnCount; j++)
                {
                    await Task.Yield();
                    float xPosition = m_rowDistance * i;
                    float zPosition = m_columnDistance * j;
                    
                    InstantiateObject(new Vector3(xPosition,0,zPosition), m_lastParent.transform);
                }
            }
        }

        private async void GenerateTriangular()
        {
            int horizontalStepCount = m_baseCount;
            int verticalStepCount = m_baseCount;

            for (int i = 0; i < horizontalStepCount; i++)
            {
                for (int j = 0; j < verticalStepCount; j++)
                {
                    await Task.Yield();
                    float xPosition = m_distanceBetween * j;
                    float zPosition = m_distanceBetween * i;
                    Vector3 position = new Vector3(xPosition, 0, zPosition);
                    position.z += j * m_distanceBetween / 2;
                    InstantiateObject(position, m_lastParent.transform);
                }
                verticalStepCount--;
            }
        }
        
        private async void GenerateCircular()
        {
            for (int i = 0; i < m_circleCount; i++)
            {
                int intensity = m_circleIntensity[i];
                float radius = m_circleRadiuses[i];
                
                for (int j = 0; j < intensity; j++)
                {
                    await Task.Yield();
                    float anglePerItem = 360f / intensity;
                    anglePerItem *= Deg2Rad;

                    float currentAngle = anglePerItem * j;
                    float currentRadius = radius;
                
                    float xPosition = currentRadius * Cos(currentAngle);
                    float zPosition = currentRadius * Sin(currentAngle);
                    
                    InstantiateObject(new Vector3(xPosition, 0, zPosition), m_lastParent.transform);            
                }
            }
        }
        
        private void InstantiateObject(Vector3 position, Transform parent = null)
        {
            GameObject obj = PrefabUtility.InstantiatePrefab(m_prefab) as GameObject;
            obj.transform.position = position;
            obj.transform.SetParent(parent.transform);

            m_allItems.Add(obj);
            m_lastItems.Add(obj);
        }
        
        private static void DrawLine()
        {
            EditorGUILayout.Space(5);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false,1), Color.gray);
            EditorGUILayout.Space(5);
        }
    }

    public enum LevelDesignMode
    {
        Circular,
        Rectangular,
        Triangle
    }
}
