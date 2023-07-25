using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IW.EditorExtensions
{
    public abstract class BaseScriptableObjectAssetLibraryTool : AssetLibraryTool
    {
        private Dictionary<ScriptableObject, Texture2D> m_cachedThumbnails;
        protected List<string> FilteredAssetPaths;
        protected Dictionary<string, ScriptableObject> ScannedAssetObjects;

        private string m_searchfilter;

        protected abstract Type GetScriptableObjectType();

        protected abstract Texture2D GenerateScriptableObjectThumbnail(int index);

        public override string ToolName()
        {
            return "Item";
        }

        protected void ClearThumbnailChache()
        {
            m_cachedThumbnails = new Dictionary<ScriptableObject, Texture2D>();
        }

        public override void Init()
        {
            m_cachedThumbnails = new Dictionary<ScriptableObject, Texture2D>();
            ScanAssets();
            SearchAssets(m_searchfilter);
        }

        public override void DrawTopbar()
        {
            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Refresh"), EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                m_cachedThumbnails = new Dictionary<ScriptableObject, Texture2D>();

                ScanAssets();
                SearchAssets(m_searchfilter);
            }

            EditorGUI.BeginChangeCheck();

            m_searchfilter = EditorGUILayout.TextField(m_searchfilter, EditorStyles.toolbarSearchField);

            bool search = EditorGUI.EndChangeCheck();
            if (search)
                SearchAssets(m_searchfilter);
        }

        protected override int GetItemCount()
        {
            return FilteredAssetPaths.Count;
        }

        protected override void OnClickItem(int index)
        {
            ScriptableObject asset = ScannedAssetObjects[FilteredAssetPaths[index]];
            Selection.activeObject = asset;
        }

        public override void OnDestroy()
        {
            if (m_cachedThumbnails == null)
                return;

            foreach (KeyValuePair<ScriptableObject, Texture2D> pair in m_cachedThumbnails) Object.DestroyImmediate(pair.Value);
        }

        protected override bool IsItemValid(int index)
        {
            return ScannedAssetObjects[FilteredAssetPaths[index]] != null;
        }

        protected override LibraryItem GetItem(int index)
        {
            ScriptableObject scriptableObject = ScannedAssetObjects[FilteredAssetPaths[index]];

            LibraryItem item = new();

            /*
            //Tooltip
            item.tooltip = scriptableObject.name;

            if (item.tooltip.StartsWith("pre_"))
                item.tooltip = item.tooltip.Substring(4, item.tooltip.Length - 4);
            */

            if (!m_cachedThumbnails.ContainsKey(scriptableObject))
                m_cachedThumbnails[scriptableObject] = GenerateScriptableObjectThumbnail(index);

            item.Thumbnail = m_cachedThumbnails[scriptableObject];
            item.IsSelected = Selection.activeObject == scriptableObject;

            return item;
        }

        private void SearchAssets(string search)
        {
            FilteredAssetPaths = new List<string>();

            foreach (KeyValuePair<string, ScriptableObject> pair in ScannedAssetObjects)
                if (search == string.Empty || search == null || Path.GetFileName(pair.Key).ToLower().Contains(search))
                    FilteredAssetPaths.Add(pair.Key);
        }

        private void ScanAssets()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + GetScriptableObjectType().Name);

            ScannedAssetObjects = new Dictionary<string, ScriptableObject>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path.Replace("\\", "/"));

                ScannedAssetObjects[path] = obj;
            }
        }
    }
}