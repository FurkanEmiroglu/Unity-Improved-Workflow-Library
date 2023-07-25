//Assets/Editor/SearchForComponents.cs

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SearchForComponents : EditorWindow
{
    private readonly string[] m_checkType = { "Check single component", "Check all components" };
    private string m_componentName = "";
    private int m_editorMode, m_selectedCheckType;

    private List<string> m_listResult;

    private readonly string[] m_modes = { "Search for component usage", "Search for missing components" };
    private List<ComponentNames> m_prefabComponents, m_notUsedComponents, m_addedComponents, m_existingComponents, m_sceneComponents;
    private bool m_recursionVal;
    private Vector2 m_scroll, m_scroll1, m_scroll2, m_scroll3, m_scroll4;

    private bool m_showPrefabs, m_showAdded, m_showScene, m_showUnused = true;
    private MonoScript m_targetComponent;

    private void OnGUI()
    {
        GUILayout.Label(position + "");
        GUILayout.Space(3);
        int oldValue = GUI.skin.window.padding.bottom;
        GUI.skin.window.padding.bottom = -20;
        Rect windowRect = GUILayoutUtility.GetRect(1, 17);
        windowRect.x += 4;
        windowRect.width -= 7;
        m_editorMode = GUI.SelectionGrid(windowRect, m_editorMode, m_modes, 2, "Window");
        GUI.skin.window.padding.bottom = oldValue;

        switch (m_editorMode)
        {
            case 0:
                m_selectedCheckType = GUILayout.SelectionGrid(m_selectedCheckType, m_checkType, 2, "Toggle");
                m_recursionVal = GUILayout.Toggle(m_recursionVal, "Search all dependencies");
                GUI.enabled = m_selectedCheckType == 0;
                m_targetComponent = (MonoScript)EditorGUILayout.ObjectField(m_targetComponent, typeof(MonoScript), false);
                GUI.enabled = true;

                if (GUILayout.Button("Check component usage"))
                {
                    AssetDatabase.SaveAssets();
                    switch (m_selectedCheckType)
                    {
                        case 0:
                            m_componentName = m_targetComponent.name;
                            string targetPath = AssetDatabase.GetAssetPath(m_targetComponent);
                            string[] allPrefabs = GetAllPrefabs();
                            m_listResult = new List<string>();
                            foreach (string prefab in allPrefabs)
                            {
                                string[] single = { prefab };
                                string[] dependencies = AssetDatabase.GetDependencies(single, m_recursionVal);
                                foreach (string dependedAsset in dependencies)
                                    if (dependedAsset == targetPath)
                                        m_listResult.Add(prefab);
                            }

                            break;
                        case 1:
                            List<string> scenesToLoad = new();
                            m_existingComponents = new List<ComponentNames>();
                            m_prefabComponents = new List<ComponentNames>();
                            m_notUsedComponents = new List<ComponentNames>();
                            m_addedComponents = new List<ComponentNames>();
                            m_sceneComponents = new List<ComponentNames>();

                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            {
                                string projectPath = Application.dataPath;
                                projectPath = projectPath.Substring(0, projectPath.IndexOf("Assets"));

                                string[] allAssets = AssetDatabase.GetAllAssetPaths();

                                foreach (string asset in allAssets)
                                {
                                    int indexCs = asset.IndexOf(".cs");
                                    int indexJs = asset.IndexOf(".js");
                                    if (indexCs != -1 || indexJs != -1)
                                    {
                                        ComponentNames newComponent = new(NameFromPath(asset), "", asset);
                                        try
                                        {
                                            FileStream fs = new(projectPath + asset, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                            StreamReader sr = new(fs);
                                            string line;
                                            while (!sr.EndOfStream)
                                            {
                                                line = sr.ReadLine();
                                                int index1 = line.IndexOf("namespace");
                                                int index2 = line.IndexOf("{");
                                                if (index1 != -1 && index2 != -1)
                                                {
                                                    line = line.Substring(index1 + 9);
                                                    index2 = line.IndexOf("{");
                                                    line = line.Substring(0, index2);
                                                    line = line.Replace(" ", "");
                                                    newComponent.NamespaceName = line;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                        }

                                        m_existingComponents.Add(newComponent);

                                        try
                                        {
                                            FileStream fs = new(projectPath + asset, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                            StreamReader sr = new(fs);

                                            string line;
                                            int lineNum = 0;
                                            while (!sr.EndOfStream)
                                            {
                                                lineNum++;
                                                line = sr.ReadLine();
                                                int index = line.IndexOf("AddComponent");
                                                if (index != -1)
                                                {
                                                    line = line.Substring(index + 12);
                                                    if (line[0] == '(')
                                                        line = line.Substring(1, line.IndexOf(')') - 1);
                                                    else if (line[0] == '<')
                                                        line = line.Substring(1, line.IndexOf('>') - 1);
                                                    else
                                                        continue;
                                                    line = line.Replace(" ", "");
                                                    line = line.Replace("\"", "");
                                                    index = line.LastIndexOf('.');
                                                    ComponentNames newComp;
                                                    if (index == -1)
                                                        newComp = new ComponentNames(line, "", "");
                                                    else
                                                        newComp = new ComponentNames(line.Substring(index + 1, line.Length - (index + 1)),
                                                            line.Substring(0, index), "");
                                                    string pName = asset + ", Line " + lineNum;
                                                    newComp.UsageSource.Add(pName);
                                                    index = m_addedComponents.IndexOf(newComp);
                                                    if (index == -1)
                                                    {
                                                        m_addedComponents.Add(newComp);
                                                    }
                                                    else
                                                    {
                                                        if (!m_addedComponents[index].UsageSource.Contains(pName))
                                                            m_addedComponents[index].UsageSource.Add(pName);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }

                                    int indexPrefab = asset.IndexOf(".prefab");

                                    if (indexPrefab != -1)
                                    {
                                        string[] single = { asset };
                                        string[] dependencies = AssetDatabase.GetDependencies(single, m_recursionVal);
                                        foreach (string dependedAsset in dependencies)
                                            if (dependedAsset.IndexOf(".cs") != -1 || dependedAsset.IndexOf(".js") != -1)
                                            {
                                                ComponentNames newComponent = new(NameFromPath(dependedAsset),
                                                    GetNamespaceFromPath(dependedAsset), dependedAsset);
                                                int index = m_prefabComponents.IndexOf(newComponent);
                                                if (index == -1)
                                                {
                                                    newComponent.UsageSource.Add(asset);
                                                    m_prefabComponents.Add(newComponent);
                                                }
                                                else
                                                {
                                                    if (!m_prefabComponents[index].UsageSource.Contains(asset))
                                                        m_prefabComponents[index].UsageSource.Add(asset);
                                                }
                                            }
                                    }

                                    int indexUnity = asset.IndexOf(".unity");
                                    if (indexUnity != -1) scenesToLoad.Add(asset);
                                }

                                for (int i = m_addedComponents.Count - 1; i > -1; i--)
                                {
                                    m_addedComponents[i].AssetPath = GetPathFromNames(m_addedComponents[i].NamespaceName,
                                        m_addedComponents[i].ComponentName);
                                    if (m_addedComponents[i].AssetPath == "") m_addedComponents.RemoveAt(i);
                                }

                                foreach (string scene in scenesToLoad)
                                {
                                    EditorSceneManager.OpenScene(scene);
                                    GameObject[] sceneGOs = GetAllObjectsInScene();
                                    foreach (GameObject g in sceneGOs)
                                    {
                                        Component[] comps = g.GetComponentsInChildren<Component>(true);
                                        foreach (Component c in comps)
                                            if (c != null && c.GetType() != null && c.GetType().BaseType != null &&
                                                c.GetType().BaseType == typeof(MonoBehaviour))
                                            {
                                                SerializedObject so = new(c);
                                                SerializedProperty p = so.FindProperty("m_Script");
                                                string path = AssetDatabase.GetAssetPath(p.objectReferenceValue);
                                                ComponentNames newComp = new(NameFromPath(path), GetNamespaceFromPath(path), path);
                                                newComp.UsageSource.Add(scene);
                                                int index = m_sceneComponents.IndexOf(newComp);
                                                if (index == -1)
                                                {
                                                    m_sceneComponents.Add(newComp);
                                                }
                                                else
                                                {
                                                    if (!m_sceneComponents[index].UsageSource.Contains(scene))
                                                        m_sceneComponents[index].UsageSource.Add(scene);
                                                }
                                            }
                                    }
                                }

                                foreach (ComponentNames c in m_existingComponents)
                                {
                                    if (m_addedComponents.Contains(c)) continue;
                                    if (m_prefabComponents.Contains(c)) continue;
                                    if (m_sceneComponents.Contains(c)) continue;
                                    m_notUsedComponents.Add(c);
                                }

                                m_addedComponents.Sort(SortAlphabetically);
                                m_prefabComponents.Sort(SortAlphabetically);
                                m_sceneComponents.Sort(SortAlphabetically);
                                m_notUsedComponents.Sort(SortAlphabetically);
                            }

                            break;
                    }
                }

                break;
            case 1:
                if (GUILayout.Button("Search!"))
                {
                    string[] allPrefabs = GetAllPrefabs();
                    m_listResult = new List<string>();
                    foreach (string prefab in allPrefabs)
                    {
                        Object o = AssetDatabase.LoadMainAssetAtPath(prefab);
                        GameObject go;
                        try
                        {
                            go = (GameObject)o;
                            Component[] components = go.GetComponentsInChildren<Component>(true);
                            foreach (Component c in components)
                                if (c == null)
                                    m_listResult.Add(prefab);
                        }
                        catch
                        {
                            Debug.Log("For some reason, prefab " + prefab + " won't cast to GameObject");
                        }
                    }
                }

                break;
        }

        if (m_editorMode == 1 || m_selectedCheckType == 0)
        {
            if (m_listResult != null)
            {
                if (m_listResult.Count == 0)
                {
                    GUILayout.Label(m_editorMode == 0
                        ? m_componentName == "" ? "Choose a component" : "No prefabs use component " + m_componentName
                        : "No prefabs have missing components!\nClick Search to check again");
                }
                else
                {
                    GUILayout.Label(m_editorMode == 0
                        ? "The following " + m_listResult.Count + " prefabs use component " + m_componentName + ":"
                        : "The following prefabs have missing components:");
                    m_scroll = GUILayout.BeginScrollView(m_scroll);
                    foreach (string s in m_listResult)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(s, GUILayout.Width(position.width / 2));
                        if (GUILayout.Button("Select", GUILayout.Width(position.width / 2 - 10)))
                            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(s);
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndScrollView();
                }
            }
        }
        else
        {
            m_showPrefabs = GUILayout.Toggle(m_showPrefabs, "Show prefab components");
            if (m_showPrefabs)
            {
                GUILayout.Label("The following components are attatched to prefabs:");
                DisplayResults(ref m_scroll1, ref m_prefabComponents);
            }

            m_showAdded = GUILayout.Toggle(m_showAdded, "Show AddComponent arguments");
            if (m_showAdded)
            {
                GUILayout.Label("The following components are AddComponent arguments:");
                DisplayResults(ref m_scroll2, ref m_addedComponents);
            }

            m_showScene = GUILayout.Toggle(m_showScene, "Show Scene-used components");
            if (m_showScene)
            {
                GUILayout.Label("The following components are used by scene objects:");
                DisplayResults(ref m_scroll3, ref m_sceneComponents);
            }

            m_showUnused = GUILayout.Toggle(m_showUnused, "Show Unused Components");
            if (m_showUnused)
            {
                GUILayout.Label("The following components are not used by prefabs, by AddComponent, OR in any scene:");
                DisplayResults(ref m_scroll4, ref m_notUsedComponents);
            }
        }
    }

    [MenuItem("Tools/Workflow/Search For Components", default, 1)]
    private static void Init()
    {
        SearchForComponents window = (SearchForComponents)GetWindow(typeof(SearchForComponents));
        window.Show();
        window.position = new Rect(20, 80, 550, 500);
    }

    private int SortAlphabetically(ComponentNames a, ComponentNames b)
    {
        return a.AssetPath.CompareTo(b.AssetPath);
    }

    private GameObject[] GetAllObjectsInScene()
    {
        List<GameObject> objectsInScene = new();
        GameObject[] allGOs = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (GameObject go in allGOs)
        {
            //if ( go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave )
            //    continue;

            string assetPath = AssetDatabase.GetAssetPath(go.transform.root.gameObject);
            if (!string.IsNullOrEmpty(assetPath))
                continue;

            objectsInScene.Add(go);
        }

        return objectsInScene.ToArray();
    }

    private void DisplayResults(ref Vector2 scroller, ref List<ComponentNames> list)
    {
        if (list == null) return;
        scroller = GUILayout.BeginScrollView(scroller);
        foreach (ComponentNames c in list)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(c.AssetPath, GUILayout.Width(position.width / 5 * 4));
            if (GUILayout.Button("Select", GUILayout.Width(position.width / 5 - 30)))
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(c.AssetPath);
            GUILayout.EndHorizontal();
            if (c.UsageSource.Count == 1) GUILayout.Label("   In 1 Place: " + c.UsageSource[0]);
            if (c.UsageSource.Count > 1)
                GUILayout.Label("   In " + c.UsageSource.Count + " Places: " + c.UsageSource[0] + ", " + c.UsageSource[1] +
                                (c.UsageSource.Count > 2 ? ", ..." : ""));
        }

        GUILayout.EndScrollView();
    }

    private string NameFromPath(string s)
    {
        s = s.Substring(s.LastIndexOf('/') + 1);
        return s.Substring(0, s.Length - 3);
    }

    private string GetNamespaceFromPath(string path)
    {
        foreach (ComponentNames c in m_existingComponents)
            if (c.AssetPath == path)
                return c.NamespaceName;
        return "";
    }

    private string GetPathFromNames(string space, string name)
    {
        ComponentNames test = new(name, space, "");
        int index = m_existingComponents.IndexOf(test);
        if (index != -1) return m_existingComponents[index].AssetPath;
        return "";
    }

    public static string[] GetAllPrefabs()
    {
        string[] temp = AssetDatabase.GetAllAssetPaths();
        List<string> result = new();
        foreach (string s in temp)
            if (s.Contains(".prefab"))
                result.Add(s);
        return result.ToArray();
    }

    private class ComponentNames
    {
        public string AssetPath;
        public readonly string ComponentName;
        public string NamespaceName;
        public readonly List<string> UsageSource;

        public ComponentNames(string comp, string space, string path)
        {
            ComponentName = comp;
            NamespaceName = space;
            AssetPath = path;
            UsageSource = new List<string>();
        }

        public override bool Equals(object obj)
        {
            return ((ComponentNames)obj).ComponentName == ComponentName && ((ComponentNames)obj).NamespaceName == NamespaceName;
        }

        public override int GetHashCode()
        {
            return ComponentName.GetHashCode() + NamespaceName.GetHashCode();
        }
    }
}