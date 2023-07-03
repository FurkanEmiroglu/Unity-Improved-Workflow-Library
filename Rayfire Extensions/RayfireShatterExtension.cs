//#define RAYFIRE

#if RAYFIRE
using UnityEditor;
using UnityEngine;

namespace ImprovedWorkflow.EditorTools.RayfireExtensions
{
    public class RayfireShatterExtension : EditorWindow
    {
        public Transform saveTarget;
        public Material material;

        public Transform loadTarget;
        public bool addMeshColliders;
        public Mesh[] meshes;

        // editor window opener
        [MenuItem("Workflow/Rayfire Shatter Helper")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(RayfireShatterExtension));
            window.Show();
            window.titleContent = new GUIContent("Rayfire Shatter Helper");
        }
        
        private void OnGUI()
        {
            SerializedObject target = new SerializedObject(this);
            target.Update();
            
            EditorGUILayout.LabelField("Save Prefractured mesh", EditorStyles.boldLabel, GUILayout.Height(25));
            saveTarget = EditorGUILayout.ObjectField(saveTarget, typeof(Transform), true) as Transform;

            if (GUILayout.Button("Save Prefracture"))
            {
                Save();
            }
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Load Prefractured mesh", EditorStyles.boldLabel, GUILayout.Height(25));
            
            loadTarget = EditorGUILayout.ObjectField(loadTarget, typeof(Transform), true) as Transform;
            
            material = EditorGUILayout.ObjectField(material, typeof(Material), true) as Material;
            
            EditorGUILayout.BeginHorizontal();
            addMeshColliders = EditorGUILayout.ToggleLeft("Add Mesh Colliders", addMeshColliders);            
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            SerializedProperty meshesProperty = target.FindProperty("meshes");
            EditorGUILayout.PropertyField(meshesProperty, true);
            

            if (GUILayout.Button("Load Prefracture"))
            {
                Load();
            }
            
            EditorGUILayout.EndHorizontal();

            target.ApplyModifiedProperties();
        }
        
        private void Load()
        {
            string path = EditorUtility.OpenFilePanel("Load Prefracture", "", "json");
            string jsonString = System.IO.File.ReadAllText(path);
            CellData cellData = JsonUtility.FromJson<CellData>(jsonString);
            LoadFromCellData(cellData);
        }
        
        private void Save()
        {
            CellData cellData = CreateCellData();
            SaveAsJson(cellData);
        }

        private CellData CreateCellData()
        {
            int childCount = saveTarget.childCount;
            Vector3[] positions = new Vector3[childCount];
            Vector3[] rotations = new Vector3[childCount];
            Vector3[] scales = new Vector3[childCount];
            
            for (int i = 0; i < childCount; i++)
            {
                Transform child = saveTarget.GetChild(i);

                Vector3 localPos = child.localPosition;
                Vector3 rotation = child.rotation.eulerAngles;
                Vector3 scale = child.localScale;
                
                positions[i] = localPos;
                rotations[i] = rotation;
                scales[i] = scale;
            }

            CellData cells = new CellData(positions, rotations, scales);
            return cells;
        }

        private void LoadFromCellData(CellData cellData)
        {
            DestroyChildrens();

            int childCount = cellData.positions.Length;

            for (int i = 0; i < childCount; i++)
            {
                Transform child = new GameObject().transform;
                child.SetParent(loadTarget, false);
                child.localPosition = cellData.positions[i];
                child.rotation = Quaternion.Euler(cellData.rotations[i]);
                child.localScale = cellData.scales[i];
                
                AddMeshFilter(child.gameObject, i);
                AddMeshRenderer(child.gameObject);
                if (addMeshColliders) AddMeshCollider(child.gameObject);
            }
        }
        
        private void DestroyChildrens()
        {
            int childCount = loadTarget.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(loadTarget.GetChild(0).gameObject);
            }
        }

        private void AddMeshFilter(GameObject obj, int index)
        {
            MeshFilter filter = obj.AddComponent<MeshFilter>();
            filter.mesh = meshes[index];
        }
        
        private void AddMeshRenderer(GameObject obj)
        {
            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
        }

        private void AddMeshCollider(GameObject obj)
        {
            MeshCollider collider = obj.AddComponent<MeshCollider>();
            collider.sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
        }
        
        private void SaveAsJson(CellData cellData)
        {
            string jsonString = JsonUtility.ToJson(cellData);
            string path = EditorUtility.SaveFilePanel("Save Prefracture", "", "prefracture", "json");
            
            System.IO.File.WriteAllText(path, jsonString);
            AssetDatabase.Refresh();
        }
    }
}
#endif