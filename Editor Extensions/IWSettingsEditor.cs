using System.Linq;
using UnityEditor;

namespace IW.EditorExtensions
{
    [CustomEditor(typeof(IWSettings))]
    public class IWSettingsEditor : Editor
    {
        private SerializedProperty[] m_dependencyProperties;
        
        private string[] m_dependencyLabels;
        private string[] m_dependencySymbols;
        
        private SerializedProperty m_dotweenEnabled;
        private SerializedProperty m_rayfireEnabled;
        private SerializedProperty m_cinemachineEnabled;
        private SerializedProperty m_odinEnabled;
        
        private SerializedProperty m_scriptableCreatorNamespaces;

        private void OnEnable()
        {
            m_dotweenEnabled = serializedObject.FindProperty("_doTweenIncluded");
            m_rayfireEnabled = serializedObject.FindProperty("_rayfireIncluded");
            m_cinemachineEnabled = serializedObject.FindProperty("_cinemachineIncluded");
            m_odinEnabled = serializedObject.FindProperty("_odinIncluded");
            
            m_scriptableCreatorNamespaces = serializedObject.FindProperty("_scriptableCreatorNamespaces");

            m_dependencyProperties = new[]
            {
                m_dotweenEnabled,
                m_rayfireEnabled,
                m_cinemachineEnabled,
                m_odinEnabled
            };
            
            m_dependencyLabels = new[]
            {
                "Dotween Extensions",
                "Rayfire Extensions",
                "Cinemachine Extensions",
                "Odin Inspector Extensions"
            };
            
            m_dependencySymbols = new[]
            {
                ImprovedWorkflowConstants.DOTWEEN_EXTENSIONS_SYMBOL,
                ImprovedWorkflowConstants.RAYFIRE_EXTENSIONS_SYMBOL,
                ImprovedWorkflowConstants.CINEMACHINE_EXTENSIONS_SYMBOL,
                ImprovedWorkflowConstants.ODIN_INSPECTOR_EXTENSIONS
            };
        }

        public override void OnInspectorGUI()
        {
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.HelpBox("Please wait until compilation has finished.", MessageType.Info);
                return;
            }
            serializedObject.Update();
            
            AddDependencyToggles(m_dependencyLabels, m_dependencyProperties, m_dependencySymbols);
            
#if IW_ODIN_INSPECTOR_EXTENSIONS
            EditorGUILayout.PropertyField(m_scriptableCreatorNamespaces, true);
#endif

            serializedObject.ApplyModifiedProperties();
        }

        private void AddDependencyToggles(string[] labels, SerializedProperty[] props, string[] symbols)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                EditorGUI.BeginChangeCheck();
                props[i].boolValue = EditorGUILayout.ToggleLeft(labels[i], props[i].boolValue);
                if (EditorGUI.EndChangeCheck())
                {
                    if (props[i].boolValue)
                        AddDefineSymbol(symbols[i]);
                    else
                        RemoveDefineSymbol(symbols[i]);
                }
            }
        }
        
        private static void AddDefineSymbol(string symbol)
        {
            string defineSymbolToAdd = symbol;
        
            // Get the current list of scripting define symbols for the current build target group
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            // Check if the symbol already exists to avoid adding duplicates
            if (defineSymbols.Contains(defineSymbolToAdd)) return;
            // Add the new symbol
            if (!string.IsNullOrEmpty(defineSymbols))
                defineSymbols += ";"; // Add a separator if the list is not empty
            defineSymbols += defineSymbolToAdd;

            // Set the updated scripting define symbols
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defineSymbols);

            // Save the changes and refresh the editor
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private static void RemoveDefineSymbol(string symbol)
        {
            string defineSymbolToRemove = symbol;

            // Get the current list of scripting define symbols for the current build target group
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            // Check if the symbol exists in the list
            if (defineSymbols.Contains(defineSymbolToRemove))
            {
                // Remove the symbol from the list
                string[] symbols = defineSymbols.Split(';');
                defineSymbols = string.Join(";", symbols.Where(sym => sym != defineSymbolToRemove));

                // Set the updated scripting define symbols
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defineSymbols);

                // Save the changes and refresh the editor
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}