using System;
using UnityEditor;
using UnityEngine;

namespace ImprowedWorkflow.UnityEditorExtensions
{
    [CustomEditor(typeof(Transform))][CanEditMultipleObjects]

    public class CustomTransformInspector : Editor
    {
        private Editor m_editor;
        private Transform m_targetTransform;

        private void OnEnable()
        {
            m_editor = CreateEditor(targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
            m_targetTransform = target as Transform;
        }

        public override void OnInspectorGUI()
        {
            if (m_editor == null) return;
            if (m_targetTransform == null) return;
            m_editor.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.gray;
            if (GUILayout.Button("Reset Position")) m_targetTransform.localPosition = Vector3.zero;

            if (GUILayout.Button("Reset Rotation")) m_targetTransform.localRotation = Quaternion.identity;

            if (GUILayout.Button("Reset Scale")) m_targetTransform.localScale = Vector3.one;

            EditorGUILayout.EndHorizontal();
        }
    }
}
