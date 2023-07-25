using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IW.EditorExtensions
{
    public class AssetLibraryWindow : EditorWindow
    {
        public static GUIStyle ToggleButtonStyleNormal;
        public static GUIStyle ToggleButtonStyleToggled;

        public int _buttonSize = 80;
        private int m_currentTool;

        private List<AssetLibraryTool> m_tools;

        private void Update()
        {
            //if(this.position.Contains(Event.current.mousePosition))
            Repaint();
        }

        private void OnEnable()
        {
            List<Type> types = TypeCache.GetTypesDerivedFrom<AssetLibraryTool>().ToList();

            m_tools = new List<AssetLibraryTool>();
            foreach (Type type in types)
                if (!type.IsAbstract)
                    m_tools.Add((AssetLibraryTool)Activator.CreateInstance(type));

            foreach (AssetLibraryTool tool in m_tools)
            {
                tool.Init();
                tool.SetRedraw(Repaint);
            }

            m_currentTool = 0;
        }

        protected void OnDestroy()
        {
            for (int i = 0; i < m_tools.Count; i++)
                m_tools[i].OnDestroy();
        }

        protected void OnGUI()
        {
            BuildStyles();

            DrawTopbar();
            m_tools[m_currentTool].DrawContent(position.height, _buttonSize);
        }

        [MenuItem("Tools/Workflow/Prefab Library", default, 1)]
        public static void OpenWindow()
        {
            AssetLibraryWindow window = GetWindow<AssetLibraryWindow>();
            window.titleContent = new GUIContent("Prefab Library");
        }

        private void DrawTopbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            //Tool selector
            if (GUILayout.Button(m_tools[m_currentTool].ToolName(), EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                GenericMenu menu = new();

                for (int i = 0; i < m_tools.Count; i++)
                {
                    int index = i;
                    menu.AddItem(new GUIContent(m_tools[i].ToolName()), m_currentTool == i, () => { SelectTool(index); });
                }

                menu.ShowAsContext();
            }

            m_tools[m_currentTool].DrawTopbar();

            EditorGUILayout.EndHorizontal();
        }

        private void SelectTool(int index)
        {
            m_currentTool = index;
        }

        private void BuildStyles()
        {
            if (ToggleButtonStyleNormal == null)
                ToggleButtonStyleNormal = "button";

            if (ToggleButtonStyleToggled == null)
            {
                ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleNormal.active.background;
                ToggleButtonStyleToggled.hover.background = ToggleButtonStyleNormal.active.background;
                ToggleButtonStyleToggled.focused.background = ToggleButtonStyleNormal.active.background;
                ToggleButtonStyleToggled.active.background = ToggleButtonStyleNormal.active.background;
            }
        }
    }

    public static class SceneDragAndDrop
    {
        private const string drag_id = "SceneDragAndDrop";
        private static readonly int s_sceneDragHint = "SceneDragAndDrop".GetHashCode();

        private static readonly Object[] s_emptyObjects = new Object[0];
        private static readonly string[] s_emptyPaths = new string[0];

        public static void StartDrag(ISceneDragReceiver receiver, string title, object data = null)
        {
            //stop any drag before starting a new one

            StopDrag();

            if (receiver != null)
            {
                //make sure we release any control from something that has it
                //this is done because SceneView delegate needs DragEvents!

                GUIUtility.hotControl = 0;

                //do the necessary steps to start a drag
                //we set the GenericData to our receiver so it can handle

                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = s_emptyObjects;
                DragAndDrop.paths = s_emptyPaths;
                DragAndDrop.SetGenericData(drag_id, new DragData(receiver, data));

                receiver.StartDrag(data);

                //start drag and listen for Scene drop

                DragAndDrop.StartDrag(title);

                SceneView.duringSceneGui += OnSceneGUI;
            }
        }

        public static void StopDrag()
        {
            //cleanup delegate
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            //get a controlId so we can grab events

            int controlId = GUIUtility.GetControlID(s_sceneDragHint, FocusType.Passive);

            Event evt = Event.current;
            EventType eventType = evt.GetTypeForControl(controlId);

            ISceneDragReceiver receiver;
            DragData dragData;

            switch (eventType)
            {
                case EventType.DragPerform:
                case EventType.DragUpdated:

                    //check that GenericData is the expected type
                    //if not, we do nothing
                    //it would seem that whenever a Drag is started, GenericData is cleared, so we don't have to explicitly clear it ourself

                    try
                    {
                        dragData = (DragData)DragAndDrop.GetGenericData(drag_id);
                        receiver = dragData.Receiver;
                    }
                    catch
                    {
                        return;
                    }

                    if (receiver != null)
                    {
                        //let receiver handle the drag functionality

                        DragAndDrop.visualMode = receiver.UpdateDrag(evt, eventType, dragData.Data);

                        //perform drag if accepted

                        if (eventType == EventType.DragPerform && DragAndDrop.visualMode != DragAndDropVisualMode.None)
                        {
                            receiver.PerformDrag(evt, dragData.Data);

                            DragAndDrop.AcceptDrag();
                            DragAndDrop.SetGenericData(drag_id, default(ISceneDragReceiver));

                            //we can safely stop listening to scene gui now

                            StopDrag();
                        }

                        evt.Use();
                    }

                    break;

                case EventType.DragExited:

                    //Drag exited, This can happen when:
                    // - focus left the SceneView
                    // - user cancelled manually (Escape Key)
                    // - user released mouse
                    //So we want to inform the receiver (if any) that is was cancelled, and it can handle appropriatley
                    try
                    {
                        dragData = (DragData)DragAndDrop.GetGenericData(drag_id);
                        receiver = dragData.Receiver;
                    }
                    catch
                    {
                        return;
                    }

                    if (receiver != null)
                    {
                        receiver.StopDrag(dragData.Data);
                        evt.Use();
                    }

                    break;
            }
        }

        private struct DragData
        {
            public DragData(ISceneDragReceiver receiver, object data)
            {
                this.Receiver = receiver;
                this.Data = data;
            }

            public readonly ISceneDragReceiver Receiver;
            public readonly object Data;
        }
    }

    public interface ISceneDragReceiver
    {
        void StartDrag(object data);
        void StopDrag(object data);

        DragAndDropVisualMode UpdateDrag(Event evt, EventType eventType, object data);

        void PerformDrag(Event evt, object data);
    }
}