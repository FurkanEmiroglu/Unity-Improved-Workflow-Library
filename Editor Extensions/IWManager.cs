using UnityEditor;
using UnityEngine;
using static IW.EditorExtensions.ImprovedWorkflowConstants;

namespace IW.EditorExtensions
{
    public class IWManager : EditorWindow
    {
        #region Opener

        [MenuItem("Tools/Workflow/Preferences")]
        private static void ShowWindow()
        {
            var window = GetWindow<IWManager>();
            window.titleContent = new GUIContent("Improved Workflow Settings");
            window.Show();
        }

        #endregion

        private ScriptableObject m_workflowSettings;
        private Editor m_workflowSettingsEditor;

        private void OnEnable()
        {
            m_workflowSettings = Resources.Load("WorkflowSettings") as IWSettings;
            m_workflowSettings ??= CreateSettingsAsset();
            m_workflowSettingsEditor = Editor.CreateEditor(m_workflowSettings);
        }

        private void OnGUI()
        {
            m_workflowSettingsEditor.OnInspectorGUI();
        }
        
        private IWSettings CreateSettingsAsset()
        {
            IWSettings settings = CreateInstance<IWSettings>();
            // path has to start at "Assets"
            string path = SETTINGS_PATH;
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }
    }
}