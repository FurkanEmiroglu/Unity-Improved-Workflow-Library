using UnityEditor;
using UnityEngine;

public class NamePreSufixer : EditorWindow
{
    private const int width = 300;
    private const int height = 100;

    private string m_fix;

    private static ModificationType s_modificationType;
    
    [MenuItem("Assets/Rename Fix/Prefix", priority = 30)]
    public static void NamePrefix()
    {
        OpenWindow(ModificationType.Prefix);
    }
    
    [MenuItem("Assets/Rename Fix/Suffix", priority = 30)]   
    public static void NameSuffix()
    {
        OpenWindow(ModificationType.Suffix);
    }
    
    public void ApplyNameChanges()
    {
        switch (s_modificationType)
        {
            case ModificationType.Prefix:
                ApplyPrefix();
                break;
            case ModificationType.Suffix:
                ApplySuffix();
                break;
        }
    }

    private void ApplyPrefix()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            string name = obj.name;
            name = m_fix + name;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(obj), name);
            AssetDatabase.Refresh();
        }
    }
    
    private void ApplySuffix()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            string name = obj.name;
            name = name + m_fix;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(obj), name);
            AssetDatabase.Refresh();
        }
    }

    private static void OpenWindow(ModificationType type)
    {
        var window = GetWindow<NamePreSufixer>("Asset Namer");
        window.minSize = new Vector2(width, height);
        window.maxSize = new Vector2(width, height);

        s_modificationType = type;
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(s_modificationType == ModificationType.Prefix ? "Prefix" : "Suffix");

        // give focus name
        GUI.SetNextControlName("Fix");
        m_fix = EditorGUILayout.TextField(m_fix);
        // set focus to text field
        EditorGUI.FocusTextInControl("Fix");
        
        // check enter key
        if (Event.current.keyCode == KeyCode.Return)
        {
            // if tab key is pressed, move focus to next control
            ApplyNameChanges();
            Close();
        }
        
        if (GUILayout.Button("Apply"))
            ApplyNameChanges();
    }

    private enum ModificationType
    {
        Prefix,
        Suffix
    }
}
