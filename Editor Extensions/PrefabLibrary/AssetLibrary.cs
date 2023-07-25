using UnityEngine;

namespace IW.EditorExtensions
{
    public class AssetLibrary : ScriptableObject
    {
        private static AssetLibrary s_instance;
#if UNITY_EDITOR
        public static AssetLibrary Instance
        {
            get
            {
                if (s_instance != null)
                    return s_instance;

                s_instance = Resources.Load<AssetLibrary>("AssetLibrary");

                if (s_instance == null)
                {
                    s_instance = CreateInstance<AssetLibrary>();
                    s_instance._rootFolders = new string[1] { "Assets/" };
                    UnityEditor.AssetDatabase.CreateAsset(s_instance, "Assets/Resources/AssetLibrary.asset");
                    UnityEditor.AssetDatabase.Refresh();
                }

                return s_instance;
            }
        }
#endif
        public string[] _rootFolders = new string[0];
        public string[] _blacklistFolders = new string[0];
        public string[] _blacklistLabels = new string[0];
    }



}
