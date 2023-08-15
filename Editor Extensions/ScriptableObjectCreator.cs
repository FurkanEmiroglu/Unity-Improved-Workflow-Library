#if UNITY_EDITOR
#if IW_ODIN_INSPECTOR_EXTENSIONS

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IW.EditorExtensions
{
    public class ScriptableObjectCreator : OdinMenuEditorWindow
    {
        static HashSet<Type> ScriptableObjectTypes
        {
            get
            {
                IEnumerable<Type> temp = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes).Where(t =>
                    t.IsClass && typeof(ScriptableObject).IsAssignableFrom(t) &&
                    !typeof(EditorWindow).IsAssignableFrom(t) && !typeof(UnityEditor.Editor).IsAssignableFrom(t));

                HashSet<Type> gameCoreTemp = new HashSet<Type>();
                foreach (Type t in temp)
                {
                    if (t.Namespace != null && GetAnyNameSpaceContains(t.Namespace))
                    {
                        gameCoreTemp.Add(t);
                    }
                }
                return (HashSet<Type>)GetHashSet(gameCoreTemp);
            }
        }

        // Ref: https://stackoverflow.com/questions/34858338/how-to-elegantly-convert-ienumerablet-to-hashsett-at-runtime-without-knowing
        public static IEnumerable<T> GetHashSet<T>(IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        private static bool GetAnyNameSpaceContains(string @namespace)
        {
            IWSettings settings = Resources.Load("WorkflowSettings") as IWSettings;

            foreach (string s in settings._scriptableCreatorNamespaces)
            {
                if (@namespace.Contains(s))
                    return true;
            }
            return false;
        }

        [MenuItem("Assets/Create Scriptable Object", priority = -10000)]
        private static void ShowDialog()
        {
            string path = "Assets";
            Object obj = Selection.activeObject;
            if (obj && AssetDatabase.Contains(obj))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!Directory.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
            }

            ScriptableObjectCreator window = CreateInstance<ScriptableObjectCreator>();
            window.ShowUtility();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
            window.titleContent = new GUIContent(path);
            window.m_targetFolder = path.Trim('/');
        }

        private ScriptableObject m_previewObject;
        private string m_targetFolder;
        private Vector2 m_scroll;

        private Type SelectedType
        {
            get
            {
                OdinMenuItem m = this.MenuTree.Selection.LastOrDefault();
                return m == null ? null : m.Value as Type;
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            this.MenuWidth = 270;
            this.WindowPadding = Vector4.zero;

            OdinMenuTree tree = new OdinMenuTree(false);
            tree.Config.DrawSearchToolbar = true;
            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
            tree.AddRange(ScriptableObjectTypes.Where(x => !x.IsAbstract), GetMenuPathForType);
            tree.SortMenuItemsByName();
            tree.Selection.SelectionConfirmed += x => this.CreateAsset();
            tree.Selection.SelectionChanged += e =>
            {
                if (this.m_previewObject && !AssetDatabase.Contains(this.m_previewObject))
                {
                    DestroyImmediate(this.m_previewObject);
                }

                if (e != SelectionChangedType.ItemAdded)
                {
                    return;
                }

                Type t = this.SelectedType;
                if (t != null && !t.IsAbstract)
                {
                    this.m_previewObject = CreateInstance(t) as ScriptableObject;
                }
            };

            return tree;
        }

        private string GetMenuPathForType(Type t)
        {
            string path = "";

            if (t.Namespace != null)
            {
                foreach (string part in t.Namespace.Split('.'))
                {
                    path += part.SplitPascalCase() + "/";
                }
            }

            return path + t.Name.SplitPascalCase();
        }

        protected override IEnumerable<object> GetTargets()
        {
            yield return this.m_previewObject;
        }

        protected override void DrawEditor(int index)
        {
            this.m_scroll = GUILayout.BeginScrollView(this.m_scroll);
            {
                base.DrawEditor(index);
            }
            GUILayout.EndScrollView();

            if (this.m_previewObject)
            {
                GUILayout.FlexibleSpace();
                SirenixEditorGUI.HorizontalLineSeparator(1);

                if (GUILayout.Button("Create Asset", GUILayoutOptions.Height(30)))
                {
                    this.CreateAsset();
                }

                if (GUILayout.Button("Create Asset and Close", GUILayoutOptions.Height(30)))
                {
                    this.CreateAssetAndClose();
                }
            }
        }

        private void CreateAsset()
        {
            if (this.m_previewObject)
            {
                string dest = this.m_targetFolder + "/" + this.MenuTree.Selection.First().Name + ".asset";
                dest = AssetDatabase.GenerateUniqueAssetPath(dest);
                ProjectWindowUtil.CreateAsset(this.m_previewObject, dest);
                //EditorApplication.delayCall += this.Close;
            }
        }

        private void CreateAssetAndClose()
        {
            if (this.m_previewObject)
            {
                string dest = this.m_targetFolder + "/" + this.MenuTree.Selection.First().Name + ".asset";
                dest = AssetDatabase.GenerateUniqueAssetPath(dest);
                ProjectWindowUtil.CreateAsset(this.m_previewObject, dest);
                EditorApplication.delayCall += this.Close;
            }
        }
    }
}
#endif
#endif