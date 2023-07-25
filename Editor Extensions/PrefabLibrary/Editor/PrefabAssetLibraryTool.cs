using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IW.EditorExtensions
{
    public class PrefabAssetLibraryTool : AssetLibraryTool
    {
        private readonly bool m_singleTag = false;
        private List<string> m_allProjectLabels;
        private bool m_enableBlacklistTags = true;

        private List<string> m_filteredAssetPaths;
        private List<string> m_filteredLabels;
        private List<string> m_prefabLabels;
        private Dictionary<string, GameObject> m_scannedAssetObjects;

        private List<string> m_scannedAssetPaths;
        private string m_searchfilter;
        private string m_singleFilteredLabel;

        public override string ToolName()
        {
            return "Prefab";
        }

        public override void Init()
        {
            try
            {
                ScanAssets();
                ScanAllLabels();
            }
            catch
            {
                EditorUtility.ClearProgressBar();
            }

            SearchAssets(m_searchfilter);
        }

        public override void DrawTopbar()
        {
            bool search = false;

            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Refresh"), EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                try
                {
                    ScanAssets();
                    ScanAllLabels();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n\n" + e.StackTrace);
                    EditorUtility.ClearProgressBar();
                }

                //Search
                SearchAssets(m_searchfilter);
            }

            if (GUILayout.Toggle(m_enableBlacklistTags, new GUIContent("BL", "Hide Blacklisted Labels"), EditorStyles.toolbarButton,
                    GUILayout.Width(40)) != m_enableBlacklistTags)
            {
                m_enableBlacklistTags = !m_enableBlacklistTags;
                search = true;
            }

            EditorGUI.BeginChangeCheck();

            m_searchfilter = EditorGUILayout.TextField(m_searchfilter, EditorStyles.toolbarSearchField);

            if (m_singleTag)
                m_singleFilteredLabel = SingleSelectDropdown("", m_singleFilteredLabel, m_prefabLabels, EditorStyles.toolbarDropDown);
            else
                m_filteredLabels = MultiSelectDropdown("", m_filteredLabels, m_prefabLabels, EditorStyles.toolbarDropDown,
                    GUILayout.MaxWidth(200));

            if (EditorGUI.EndChangeCheck())
                search = true;

            if (search)
                SearchAssets(m_searchfilter);
        }

        protected override bool EnableHoverContent()
        {
            return true;
        }

        protected override void DrawHoverContent(int index)
        {
            GameObject prefab = m_scannedAssetObjects[m_filteredAssetPaths[index]];

            string name = prefab.name;

            if (name.StartsWith("pre_"))
                name = name.Substring(4, name.Length - 4);

            GUIStyle labelst = new(EditorStyles.boldLabel);
            labelst.wordWrap = true;

            EditorGUILayout.LabelField(name, labelst);

            string[] labels = AssetDatabase.GetLabels(prefab);

            if (labels.Length > 0)
            {
                string labelName = "";

                foreach (string label in AssetDatabase.GetLabels(prefab))
                    labelName += "[" + label + "] ";

                EditorGUILayout.LabelField(labelName);
            }
        }

        protected override int GetItemCount()
        {
            return m_filteredAssetPaths.Count;
        }

        protected override void OnClickItem(int index)
        {
            GameObject prefab = m_scannedAssetObjects[m_filteredAssetPaths[index]];
            Selection.activeGameObject = prefab;
        }

        protected override bool IsItemValid(int index)
        {
            return m_scannedAssetObjects[m_filteredAssetPaths[index]] != null;
        }

        protected override LibraryItem GetItem(int index)
        {
            GameObject prefab = m_scannedAssetObjects[m_filteredAssetPaths[index]];

            LibraryItem item = new();

            item.IsSelected = Selection.activeObject == prefab;

            if (Event.current.type != EventType.Layout)
                item.Thumbnail = AssetPreview.GetAssetPreview(prefab);

            return item;
        }

        protected override bool EnableDragIntoScene()
        {
            return true;
        }

        protected override GameObject CreateGhostPrefab(int index)
        {
            GameObject prefab = m_scannedAssetObjects[m_filteredAssetPaths[index]];
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }

        protected override void OnPlaceInScene(int index, Vector3 position)
        {
            GameObject prefab = m_scannedAssetObjects[m_filteredAssetPaths[index]];

            GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            spawnedObject.name = prefab.name;
            spawnedObject.transform.position = position;
            spawnedObject.transform.rotation = Quaternion.identity;

            Selection.activeObject = spawnedObject;

            Undo.RegisterCreatedObjectUndo(spawnedObject, "Drop Prefab Into Scene");
        }

        public override void OnDestroy()
        {
        }

        private List<string> GetFilteredLabels()
        {
            if (m_singleTag)
                if (m_singleFilteredLabel != null)
                    return new List<string>(new[] { m_singleFilteredLabel });
                else
                    return new List<string>();

            return m_filteredLabels;
        }

        private void SearchAssets(string search = "")
        {
            m_filteredAssetPaths = new List<string>();

            if (search == null)
                search = "";

            search = search.ToLower();
            foreach (string path in m_scannedAssetPaths)
                //Check search string
                if (search == string.Empty || Path.GetFileName(path).ToLower().Contains(search))
                {
                    //Check labels
                    string[] labels = AssetDatabase.GetLabels(m_scannedAssetObjects[path]);
                    bool success = false;

                    //No filtered labels, no problem
                    if (GetFilteredLabels().Count == 0)
                        success = true;

                    foreach (string label in GetFilteredLabels())
                        if (labels.Contains(label))
                        {
                            success = true;
                            break;
                        }

                    //Make sure is not in blacklisted label
                    foreach (string blacklistLabel in AssetLibrary.Instance._blacklistLabels)
                        if (labels.Contains(blacklistLabel))
                        {
                            success = false;
                            break;
                        }

                    if (success)
                        m_filteredAssetPaths.Add(path);
                }
        }

        private void ScanAssets()
        {
            EditorUtility.DisplayProgressBar("Scanning Assets", "Finding Prefabs", 0.1f);

            string[] guids = AssetDatabase.FindAssets("t:prefab");

            m_scannedAssetObjects = new Dictionary<string, GameObject>();
            m_scannedAssetPaths = new List<string>();
            m_filteredAssetPaths = new List<string>();

            int total = guids.Length;
            int sofar = 0;

            foreach (string guid in guids)
            {
                EditorUtility.DisplayProgressBar("Scanning Assets", "Loading Prefabs", sofar / (float)total);
                sofar++;

                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (AssetPassesScan(path))
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path.Replace("\\", "/"));

                    if (obj != null)
                    {
                        m_scannedAssetPaths.Add(path);
                        m_scannedAssetObjects.Add(path, obj);
                    }
                }
            }

            AssetPreview.SetPreviewTextureCacheSize(60);

            EditorUtility.ClearProgressBar();
        }

        private bool AssetPassesScan(string path)
        {
            //File is in blacklist
            foreach (string black in AssetLibrary.Instance._blacklistFolders)
                if (path.ToLower().StartsWith(black.ToLower()))
                    return false;

            foreach (string root in AssetLibrary.Instance._rootFolders)
                if (path.ToLower().StartsWith(root.ToLower()))
                    return true;

            return false;
        }

        private void ScanAllLabels()
        {
            m_allProjectLabels = new List<string>();

            int total = m_scannedAssetObjects.Count;
            int sofar = 0;

            foreach (KeyValuePair<string, GameObject> pair in m_scannedAssetObjects)
            {
                EditorUtility.DisplayProgressBar("Scanning Assets", "Building Labels", sofar / (float)total);
                sofar++;

                string[] labels = AssetDatabase.GetLabels(pair.Value);

                foreach (string label in labels)
                    if (!m_allProjectLabels.Contains(label))
                        m_allProjectLabels.Add(label);
            }

            m_prefabLabels = m_allProjectLabels; //Temporary

            if (m_filteredLabels == null)
                m_filteredLabels = new List<string>();

            EditorUtility.ClearProgressBar();
        }
    }
}