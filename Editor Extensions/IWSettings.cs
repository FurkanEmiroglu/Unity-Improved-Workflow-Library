using UnityEditor;
using UnityEngine;

namespace IW.EditorExtensions
{
    public sealed class IWSettings : ScriptableObject
    {
        public bool _doTweenIncluded;
        public bool _rayfireIncluded;
        public bool _cinemachineIncluded;
        public bool _odinIncluded;

        public string[] _scriptableCreatorNamespaces = new[] { "GameCore", "Dotween" };

        private void Awake()
        {
            _doTweenIncluded = CheckIncluded(ImprovedWorkflowConstants.DOTWEEN_EXTENSIONS_SYMBOL);
            _rayfireIncluded = CheckIncluded(ImprovedWorkflowConstants.RAYFIRE_EXTENSIONS_SYMBOL);
            _cinemachineIncluded = CheckIncluded(ImprovedWorkflowConstants.CINEMACHINE_EXTENSIONS_SYMBOL);
            _odinIncluded = CheckIncluded(ImprovedWorkflowConstants.ODIN_INSPECTOR_EXTENSIONS);
        }

        private bool CheckIncluded(string symbol)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            // Check if the symbol already exists to avoid adding duplicates
            return defineSymbols.Contains(symbol);   
        }
    }
}