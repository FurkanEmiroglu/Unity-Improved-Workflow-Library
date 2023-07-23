using UnityEditor;
using UnityEngine;
using static IW.EditorExtensions.ImprovedWorkflowConstants;

namespace IW.EditorExtensions
{
    public class IWManager : EditorWindow
    {
        #region Opener

        [MenuItem("Tools/Workflow/Preferences", default, 3000)]
        private static void ShowWindow()
        {
            var window = GetWindow<IWManager>();
            window.titleContent = new GUIContent("Improved Workflow Settings");
            window.Show();
        }

        #endregion

        private ScriptableObject m_workflowSettings;
        private Editor m_workflowSettingsEditor;

        private ScriptableObject m_prefabLibrarySettings;
        private Editor m_prefabLibrarySettingsEditor;
        
        private bool m_settingsTitlebar;
        private bool m_assetLibraryTitlebar;

        private void OnEnable()
        {
            m_workflowSettings = Resources.Load("WorkflowSettings") as IWSettings;
            m_workflowSettings ??= CreateSettingsAsset();
            m_workflowSettingsEditor = Editor.CreateEditor(m_workflowSettings);
            
            m_prefabLibrarySettings = Resources.Load("AssetLibrary") as AssetLibrary;
            m_prefabLibrarySettings ??= CreateAssetLibrary();
            m_prefabLibrarySettingsEditor = Editor.CreateEditor(m_prefabLibrarySettings);
        }

        private void OnGUI()
        {
            m_settingsTitlebar = EditorGUILayout.InspectorTitlebar(m_settingsTitlebar, m_workflowSettings);
            if (m_settingsTitlebar) 
                m_workflowSettingsEditor.OnInspectorGUI();
            
            m_assetLibraryTitlebar = EditorGUILayout.InspectorTitlebar(m_assetLibraryTitlebar, m_prefabLibrarySettings);
            
            if (m_assetLibraryTitlebar) 
                m_prefabLibrarySettingsEditor.OnInspectorGUI();
        }
        
        private static IWSettings CreateSettingsAsset()
        {
            IWSettings settings = CreateInstance<IWSettings>();
            // path has to start at "Assets"
            string path = SETTINGS_PATH;
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }
        
        private static AssetLibrary CreateAssetLibrary()
        {
            AssetLibrary settings = CreateInstance<AssetLibrary>();
            // path has to start at "Assets"
            string path = ASSET_LIBRARY_PATH;
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }
    }
}