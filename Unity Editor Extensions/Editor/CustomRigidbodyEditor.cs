using UnityEditor;
using UnityEngine;
using System;

namespace ImprovedWorkflow.UnityEditorExtensions
{ 
    [CustomEditor(typeof(Rigidbody))]
    [CanEditMultipleObjects]
    public sealed class CustomRigidbodyEditor : Editor
    {
        private Editor m_editor;
        private Rigidbody m_targetRigidbody;

        private void OnEnable()
        {
            m_editor = CreateEditor(targets, Type.GetType("UnityEditor.RigidbodyEditor, UnityEditor"));
            m_targetRigidbody = target as Rigidbody;
        }

        public override void OnInspectorGUI()
        {
            m_editor.OnInspectorGUI();

            GUI.color = Color.gray;
            if (GUILayout.Button("Reset Speed"))
            {
                m_targetRigidbody.velocity = Vector3.zero;
            }
        }
    }
}