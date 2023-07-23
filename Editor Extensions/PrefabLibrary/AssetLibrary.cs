using UnityEngine;

namespace IW.EditorExtensions
{
    public class AssetLibrary : ScriptableObject
    {
        private static AssetLibrary instance;
#if UNITY_EDITOR
        public static AssetLibrary Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = Resources.Load<AssetLibrary>("AssetLibrary");

                if (instance == null)
                {
                    instance = CreateInstance<AssetLibrary>();
                    instance.rootFolders = new string[1] { "Assets/" };
                    UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/Resources/AssetLibrary.asset");
                    UnityEditor.AssetDatabase.Refresh();
                }

                return instance;
            }
        }
#endif
        public string[] rootFolders = new string[0];
        public string[] blacklistFolders = new string[0];
        public string[] blacklistLabels = new string[0];
    }



}
