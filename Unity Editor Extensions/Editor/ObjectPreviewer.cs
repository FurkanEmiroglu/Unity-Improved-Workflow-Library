using UnityEditor;
using UnityEngine;

namespace ImprovedWorkflow.UnityEditorExtensions
{
    public class ObjectPreviewer : EditorWindow
    {
        private static ObjectPreviewer s_window;
        private static Object s_selectedObject;

        private static readonly Vector2 s_windowSize = new(500, 500);

        private Editor m_previewEditor;

        private void OnGUI()
        {
            PreviewObject();
            CheckCloseInput();
        }
        
        public static void Init()
        {
            if (s_window == null) ShowWindow();
        }

        private static void ShowWindow()
        {
            s_selectedObject = Selection.activeObject;

            ObjectPreviewer wind = GetWindow<ObjectPreviewer>(true, string.Empty, true);

            wind.maxSize = new Vector2(s_windowSize.x, s_windowSize.y);
            wind.minSize = new Vector2(s_windowSize.x, s_windowSize.y);

            wind.ShowPopup();

            s_window = wind;
        }

        private void CheckCloseInput()
        {
            Event currentEvent = Event.current;

#if UNITY_EDITOR_WIN
            if (currentEvent.type == EventType.KeyUp && currentEvent.keyCode == KeyCode.A) s_window.Close();
            if (focusedWindow != s_window) s_window.Close();
#elif UNITY_EDITOR_OSX
            if (currentEvent.type == EventType.KeyUp && currentEvent.keyCode == KeyCode.A) s_window.Close();
#endif
        }

        private void PreviewObject()
        {
            if (s_selectedObject == null) return;

            if (m_previewEditor == null) m_previewEditor = Editor.CreateEditor(s_selectedObject);

            m_previewEditor.OnPreviewGUI(GUILayoutUtility.GetRect(s_windowSize.x, s_windowSize.y), new GUIStyle());
        }
    }
}