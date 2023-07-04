using UnityEditor;
using UnityEngine;

namespace ImprovedWorkflow.UnityEditorExtensions
{
    public class EasyObjectPreview : EditorWindow
    {
        private static EasyObjectPreview window;
        private static Object selectedObject;

        private Editor _previewEditor;

        private static Vector2 _windowSize = new Vector2(500, 500);
    
        [MenuItem("Tools/Easy Object Preview %SPACE")]
        private static void Init()
        {
            if (window == null)
            {
                ShowWindow();
            }
        }

        private static void ShowWindow()
        {
            selectedObject = Selection.activeObject;
            
            EasyObjectPreview wind = GetWindow<EasyObjectPreview>(true, string.Empty, true);
            
            wind.maxSize = new Vector2(_windowSize.x, _windowSize.y);
            wind.minSize = new Vector2(_windowSize.x, _windowSize.y);
            
            wind.ShowPopup();
    
            window = wind;
        }
    
        private void OnGUI()
        {
            PreviewObject();
            CheckCloseInput();
        }

        private void CheckCloseInput()
        {
            var currentEvent = Event.current;
        
            if (currentEvent.type == EventType.KeyUp && currentEvent.keyCode == KeyCode.Space)
            {
                window.Close();
            }

            if (focusedWindow != window)
            {
                window.Close();
            }
        }
    
        private void PreviewObject()
        {
            if (selectedObject == null) return;
        
            if (_previewEditor == null)
            {
                _previewEditor = Editor.CreateEditor(selectedObject);
            }
        
            _previewEditor.OnPreviewGUI(GUILayoutUtility.GetRect(_windowSize.x, _windowSize.y), new GUIStyle());
        }
    }
}
