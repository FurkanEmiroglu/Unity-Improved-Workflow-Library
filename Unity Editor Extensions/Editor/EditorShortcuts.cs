using System.Diagnostics;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ImprovedWorkflow.UnityEditorExtensions
{
    public static class EditorShortcuts
    {
        /// <summary>
        ///     Opens terminal in current directory
        /// </summary>
        [MenuItem("Assets/Open Terminal...", priority = -10000)]
        private static void OpenTerminal()
        {
            Process process = Process.Start("wt.exe");
        }

        /// <summary>
        ///     Editor play mode toggle
        ///     Shortcut : Shift + e
        /// </summary>
        /// <returns></returns>
        [MenuItem("Workflow/Editor Shortcuts/Set Editor Play Mode #e")]
        public static void SetEditorPlayMode()
        {
            EditorSettings.enterPlayModeOptionsEnabled = !EditorSettings.enterPlayModeOptionsEnabled;
            EditorSettings.enterPlayModeOptions = EditorSettings.enterPlayModeOptionsEnabled
                ? EnterPlayModeOptions.DisableDomainReload
                : EnterPlayModeOptions.None;

            Debug.Log($"Editor play mode enabled: {EditorSettings.enterPlayModeOptionsEnabled}");
        }

        /// <summary>
        ///     Purpose: Locks the current inspector
        ///     Shortcut : ctrl + w
        /// </summary>
        [MenuItem("Workflow/Editor Shortcuts/Lock Inspector #w")]
        public static void LockActiveInspector()
        {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        /// <summary>
        ///     Purpose: Collapses all components in the inspector
        ///     Shortcut : Shift + b
        /// </summary>
        [MenuItem("Workflow/Editor Shortcuts/Collapse Components #b")]
        public static void CollapseComponents()
        {
            SetAllInspectorsExpanded(false);

            static void SetAllInspectorsExpanded(bool expanded)
            {
                ActiveEditorTracker activeEditorTracker = ActiveEditorTracker.sharedTracker;

                for (int i = 0; i < activeEditorTracker.activeEditors.Length; i++)
                    activeEditorTracker.SetVisible(i, expanded ? 1 : 0);

                if (!Selection.activeGameObject.TryGetComponent(out Renderer renderer)) return;

                Material[] mats = renderer.sharedMaterials;

                foreach (Material m in mats) InternalEditorUtility.SetIsInspectorExpanded(m, expanded);
            }
        }

        /// <summary>
        ///     Purpose: Clears the player prefs
        ///     Shortcut : Shift + p
        /// </summary>
        [MenuItem("Workflow/Editor Shortcuts/Clear player prefs #p")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Cleared all player prefs");
        }
    }
}