#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ImprovedWorkflow.UnityEditorExtensions
{
    internal static class ReflectionExtensions
    {
        internal static object FetchField(this Type type, string field)
        {
            return type.GetFieldRecursive(field, true).GetValue(null);
        }

        internal static object FetchField(this object obj, string field)
        {
            return obj.GetType().GetFieldRecursive(field, false).GetValue(obj);
        }

        internal static object FetchProperty(this Type type, string property)
        {
            return type.GetPropertyRecursive(property, true).GetValue(null, null);
        }

        internal static object FetchProperty(this object obj, string property)
        {
            return obj.GetType().GetPropertyRecursive(property, false).GetValue(obj, null);
        }

        internal static object CallMethod(this Type type, string method, params object[] parameters)
        {
            return type.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, parameters);
        }

        internal static object CallMethod(this object obj, string method, params object[] parameters)
        {
            return obj.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                      .Invoke(obj, parameters);
        }

        internal static object CreateInstance(this Type type, params object[] parameters)
        {
            Type[] parameterTypes;
            if (parameters == null)
            {
                parameterTypes = null;
            }
            else
            {
                parameterTypes = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    parameterTypes[i] = parameters[i].GetType();
            }

            return CreateInstance(type, parameterTypes, parameters);
        }

        internal static object CreateInstance(this Type type, Type[] parameterTypes, object[] parameters)
        {
            return type.GetConstructor(parameterTypes).Invoke(parameters);
        }

        private static FieldInfo GetFieldRecursive(this Type type, string field, bool isStatic)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly |
                                 (isStatic ? BindingFlags.Static : BindingFlags.Instance);
            do
            {
                FieldInfo fieldInfo = type.GetField(field, flags);
                if (fieldInfo != null)
                    return fieldInfo;

                type = type.BaseType;
            } while (type != null);

            return null;
        }

        private static PropertyInfo GetPropertyRecursive(this Type type, string property, bool isStatic)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly |
                                 (isStatic ? BindingFlags.Static : BindingFlags.Instance);
            do
            {
                PropertyInfo propertyInfo = type.GetProperty(property, flags);
                if (propertyInfo != null)
                    return propertyInfo;

                type = type.BaseType;
            } while (type != null);

            return null;
        }
    }

    public class MultiScreenshotCapture : EditorWindow
    {
        private const string session_data_path = "Library/MSC_Session.json";
        private const string temporary_resolution_label = "MSC_temp";
        private readonly GUILayoutOption m_glExpandWidth = GUILayout.ExpandWidth(true);
        private readonly GUILayoutOption m_glWidth25 = GUILayout.Width(25f);

        private readonly List<CustomResolution> m_queuedScreenshots = new();
        private bool m_allowTransparentBackground;
        private bool m_captureOverlayUI;
        private bool m_currentResolutionEnabled = true;
        private float m_prevTimeScale;

        private float m_resolutionMultiplier = 1f;
        //private static EditorWindow GameView { get { return (EditorWindow) GetType( "GameView" ).CallMethod( "GetMainGameView" ); } }

        private List<Vector2> m_resolutions = new() { new Vector2(1024, 768) }; // Not readonly to support serialization
        private List<bool> m_resolutionsEnabled = new() { true }; // Same as above
        private bool m_saveAsPNG = true;
        private string m_saveDirectory;

        private Vector2 m_scrollPos;
        private bool m_setTimeScaleToZero = true;

        private TargetCamera m_targetCamera = TargetCamera.GameView;

        private static object SizeHolder
        {
            get { return GetType("GameViewSizes").FetchProperty("instance").FetchProperty("currentGroup"); }
        }

        private static EditorWindow GameView
        {
            get { return GetWindow(GetType("GameView")); }
        }

        private void Awake()
        {
            if (File.Exists(session_data_path))
            {
                SessionData sessionData = JsonUtility.FromJson<SessionData>(File.ReadAllText(session_data_path));
                m_resolutions = sessionData._resolutions;
                m_resolutionsEnabled = sessionData._resolutionsEnabled;
                m_currentResolutionEnabled = sessionData._currentResolutionEnabled;
                m_resolutionMultiplier = sessionData._resolutionMultiplier > 0f ? sessionData._resolutionMultiplier : 1f;
                m_targetCamera = sessionData._targetCamera;
                m_captureOverlayUI = sessionData._captureOverlayUI;
                m_setTimeScaleToZero = sessionData._setTimeScaleToZero;
                m_saveAsPNG = sessionData._saveAsPNG;
                m_allowTransparentBackground = sessionData._allowTransparentBackground;
                m_saveDirectory = sessionData._saveDirectory;
            }
        }

        private void OnDestroy()
        {
            SessionData sessionData = new()
            {
                _resolutions = m_resolutions,
                _resolutionsEnabled = m_resolutionsEnabled,
                _currentResolutionEnabled = m_currentResolutionEnabled,
                _resolutionMultiplier = m_resolutionMultiplier,
                _targetCamera = m_targetCamera,
                _captureOverlayUI = m_captureOverlayUI,
                _setTimeScaleToZero = m_setTimeScaleToZero,
                _saveAsPNG = m_saveAsPNG,
                _allowTransparentBackground = m_allowTransparentBackground,
                _saveDirectory = m_saveDirectory
            };

            File.WriteAllText(session_data_path, JsonUtility.ToJson(sessionData));
        }

        private void OnGUI()
        {
            // In case resolutionsEnabled didn't exist when the latest SessionData was created
            if (m_resolutionsEnabled == null || m_resolutionsEnabled.Count != m_resolutions.Count)
            {
                m_resolutionsEnabled = new List<bool>(m_resolutions.Count);
                for (int i = 0; i < m_resolutions.Count; i++)
                    m_resolutionsEnabled.Add(true);
            }

            m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);

            GUILayout.BeginHorizontal();

            GUILayout.Label("Resolutions:", m_glExpandWidth);

            if (GUILayout.Button("Save"))
                SaveSettings();

            if (GUILayout.Button("Load"))
                LoadSettings();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUI.enabled = m_currentResolutionEnabled;
            GUILayout.Label("Current Resolution", m_glExpandWidth);
            GUI.enabled = true;

            m_currentResolutionEnabled = EditorGUILayout.Toggle(GUIContent.none, m_currentResolutionEnabled, m_glWidth25);

            if (GUILayout.Button("+", m_glWidth25))
            {
                m_resolutions.Insert(0, new Vector2());
                m_resolutionsEnabled.Insert(0, true);
            }

            GUI.enabled = false;
            GUILayout.Button("-", m_glWidth25);
            GUI.enabled = true;

            GUILayout.EndHorizontal();

            for (int i = 0; i < m_resolutions.Count; i++)
            {
                GUILayout.BeginHorizontal();

                GUI.enabled = m_resolutionsEnabled[i];
                m_resolutions[i] = EditorGUILayout.Vector2Field(GUIContent.none, m_resolutions[i]);
                GUI.enabled = true;
                m_resolutionsEnabled[i] = EditorGUILayout.Toggle(GUIContent.none, m_resolutionsEnabled[i], m_glWidth25);

                if (GUILayout.Button("+", m_glWidth25))
                {
                    m_resolutions.Insert(i + 1, new Vector2());
                    m_resolutionsEnabled.Insert(i + 1, true);
                }

                if (GUILayout.Button("-", m_glWidth25))
                {
                    m_resolutions.RemoveAt(i);
                    m_resolutionsEnabled.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            m_resolutionMultiplier = EditorGUILayout.FloatField("Resolution Multiplier", m_resolutionMultiplier);
            m_targetCamera = (TargetCamera)EditorGUILayout.EnumPopup("Target Camera", m_targetCamera);

            EditorGUILayout.Space();

            if (m_targetCamera == TargetCamera.GameView)
            {
                m_captureOverlayUI = EditorGUILayout.ToggleLeft("Capture Overlay UI", m_captureOverlayUI);
                if (m_captureOverlayUI && EditorApplication.isPlaying)
                {
                    EditorGUI.indentLevel++;
                    m_setTimeScaleToZero = EditorGUILayout.ToggleLeft("Set timeScale to 0 during capture", m_setTimeScaleToZero);
                    EditorGUI.indentLevel--;
                }
            }

            m_saveAsPNG = EditorGUILayout.ToggleLeft("Save as PNG", m_saveAsPNG);
            if (m_saveAsPNG && !m_captureOverlayUI && m_targetCamera == TargetCamera.GameView)
            {
                EditorGUI.indentLevel++;
                m_allowTransparentBackground = EditorGUILayout.ToggleLeft("Allow transparent background", m_allowTransparentBackground);
                if (m_allowTransparentBackground)
                    EditorGUILayout.HelpBox(
                        "For transparent background to work, you may need to disable post-processing on the Main Camera.",
                        MessageType.Info);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            m_saveDirectory = PathField("Save to:", m_saveDirectory);

            EditorGUILayout.Space();

            GUI.enabled = m_queuedScreenshots.Count == 0 && m_resolutionMultiplier > 0f;
            if (GUILayout.Button("Capture Screenshots"))
            {
                if (string.IsNullOrEmpty(m_saveDirectory))
                    m_saveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                if (m_currentResolutionEnabled)
                    CaptureScreenshot((m_targetCamera == TargetCamera.GameView ? Camera.main : SceneView.lastActiveSceneView.camera)
                                      .pixelRect.size);

                for (int i = 0; i < m_resolutions.Count; i++)
                    if (m_resolutionsEnabled[i])
                        CaptureScreenshot(m_resolutions[i]);

                if (!m_captureOverlayUI || m_targetCamera == TargetCamera.SceneView)
                {
                    Debug.Log("<b>Saved screenshots:</b> " + m_saveDirectory);
                }
                else
                {
                    if (EditorApplication.isPlaying && m_setTimeScaleToZero)
                    {
                        m_prevTimeScale = Time.timeScale;
                        Time.timeScale = 0f;
                    }

                    EditorApplication.update -= CaptureQueuedScreenshots;
                    EditorApplication.update += CaptureQueuedScreenshots;
                }
            }

            GUI.enabled = true;

            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Workflow/Multi Screenshot Capture", false, 2)]
        private static void Init()
        {
            MultiScreenshotCapture window = GetWindow<MultiScreenshotCapture>();
            window.titleContent = new GUIContent("Screenshot");
            window.minSize = new Vector2(325f, 150f);
            window.Show();
        }

        private void CaptureScreenshot(Vector2 resolution)
        {
            int width = Mathf.RoundToInt(resolution.x * m_resolutionMultiplier);
            int height = Mathf.RoundToInt(resolution.y * m_resolutionMultiplier);

            if (width <= 0 || height <= 0)
                Debug.LogWarning("Skipped resolution: " + resolution);
            else if (!m_captureOverlayUI || m_targetCamera == TargetCamera.SceneView)
                CaptureScreenshotWithoutUI(width, height);
            else
                m_queuedScreenshots.Add(new CustomResolution(width, height));
        }

        private void CaptureQueuedScreenshots()
        {
            if (m_queuedScreenshots.Count == 0)
            {
                EditorApplication.update -= CaptureQueuedScreenshots;
                return;
            }

            CustomResolution resolution = m_queuedScreenshots[0];
            if (!resolution.IsActive)
            {
                resolution.IsActive = true;

                if (EditorApplication.isPlaying && EditorApplication.isPaused)
                    EditorApplication.Step(); // Necessary to refresh overlay UI
            }
            else
            {
                try
                {
                    CaptureScreenshotWithUI();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                resolution.IsActive = false;

                m_queuedScreenshots.RemoveAt(0);
                if (m_queuedScreenshots.Count == 0)
                {
                    if (EditorApplication.isPlaying && EditorApplication.isPaused)
                        EditorApplication.Step(); // Necessary to restore overlay UI

                    if (EditorApplication.isPlaying && m_setTimeScaleToZero)
                        Time.timeScale = m_prevTimeScale;

                    Debug.Log("<b>Saved screenshots:</b> " + m_saveDirectory);
                    Repaint();
                }
                else
                {
                    // Activate the next resolution immediately
                    CaptureQueuedScreenshots();
                }
            }
        }

        private void CaptureScreenshotWithoutUI(int width, int height)
        {
            Camera camera = m_targetCamera == TargetCamera.GameView ? Camera.main : SceneView.lastActiveSceneView.camera;

            RenderTexture temp = RenderTexture.active;
            RenderTexture temp2 = camera.targetTexture;

            RenderTexture renderTex = RenderTexture.GetTemporary(width, height, 24);
            Texture2D screenshot = null;

            bool allowHDR = camera.allowHDR;
            if (m_saveAsPNG && m_allowTransparentBackground)
                camera.allowHDR = false;

            try
            {
                RenderTexture.active = renderTex;

                camera.targetTexture = renderTex;
                camera.Render();

                screenshot = new Texture2D(renderTex.width, renderTex.height,
                    m_saveAsPNG && m_allowTransparentBackground ? TextureFormat.RGBA32 : TextureFormat.RGB24, false);
                screenshot.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0, false);
                screenshot.Apply(false, false);

                File.WriteAllBytes(GetUniqueFilePath(renderTex.width, renderTex.height),
                    m_saveAsPNG ? screenshot.EncodeToPNG() : screenshot.EncodeToJPG(100));
            }
            finally
            {
                camera.targetTexture = temp2;
                if (m_saveAsPNG && m_allowTransparentBackground)
                    camera.allowHDR = allowHDR;

                RenderTexture.active = temp;
                RenderTexture.ReleaseTemporary(renderTex);

                if (screenshot != null)
                    DestroyImmediate(screenshot);
            }
        }

        private void CaptureScreenshotWithUI()
        {
            RenderTexture temp = RenderTexture.active;

            RenderTexture renderTex = (RenderTexture)GameView.FetchField("m_TargetTexture");
            Texture2D screenshot = null;

            int width = renderTex.width;
            int height = renderTex.height;

            try
            {
                RenderTexture.active = renderTex;

                screenshot = new Texture2D(width, height,
                    m_saveAsPNG && m_allowTransparentBackground ? TextureFormat.RGBA32 : TextureFormat.RGB24, false);
                screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);

                if (SystemInfo.graphicsUVStartsAtTop)
                {
                    Color32[] pixels = screenshot.GetPixels32();
                    for (int i = 0; i < height / 2; i++)
                    {
                        int startIndex0 = i * width;
                        int startIndex1 = (height - i - 1) * width;
                        for (int x = 0; x < width; x++)
                        {
                            Color32 color = pixels[startIndex0 + x];
                            pixels[startIndex0 + x] = pixels[startIndex1 + x];
                            pixels[startIndex1 + x] = color;
                        }
                    }

                    screenshot.SetPixels32(pixels);
                }

                screenshot.Apply(false, false);

                File.WriteAllBytes(GetUniqueFilePath(width, height), m_saveAsPNG ? screenshot.EncodeToPNG() : screenshot.EncodeToJPG(100));
            }
            finally
            {
                RenderTexture.active = temp;

                if (screenshot != null)
                    DestroyImmediate(screenshot);
            }
        }

        private string PathField(string label, string path)
        {
            GUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(label, path);
            if (GUILayout.Button("o", m_glWidth25))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Choose output directory", "", "");
                if (!string.IsNullOrEmpty(selectedPath))
                    path = selectedPath;

                GUIUtility.keyboardControl = 0; // Remove focus from active text field
            }

            GUILayout.EndHorizontal();

            return path;
        }

        private void SaveSettings()
        {
            string savePath = EditorUtility.SaveFilePanel("Choose destination", "", "resolutions", "json");
            if (!string.IsNullOrEmpty(savePath))
            {
                SaveData saveData = new()
                {
                    _resolutions = m_resolutions,
                    _resolutionsEnabled = m_resolutionsEnabled,
                    _currentResolutionEnabled = m_currentResolutionEnabled
                };

                File.WriteAllText(savePath, JsonUtility.ToJson(saveData, false));
            }
        }

        private void LoadSettings()
        {
            string loadPath = EditorUtility.OpenFilePanel("Choose save file", "", "json");
            if (!string.IsNullOrEmpty(loadPath))
            {
                SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(loadPath));
                m_resolutions = saveData._resolutions ?? new List<Vector2>();
                m_resolutionsEnabled = saveData._resolutionsEnabled ?? new List<bool>();
                m_currentResolutionEnabled = saveData._currentResolutionEnabled;
            }
        }

        private string GetUniqueFilePath(int width, int height)
        {
            string filename = string.Concat(width, "x", height, " {0}", m_saveAsPNG ? ".png" : ".jpeg");
            int fileIndex = 0;
            string path;
            do
            {
                path = Path.Combine(m_saveDirectory, string.Format(filename, ++fileIndex));
            } while (File.Exists(path));

            return path;
        }

        private static object GetFixedResolution(int width, int height)
        {
            object sizeType = Enum.Parse(GetType("GameViewSizeType"), "FixedResolution");
            return GetType("GameViewSize").CreateInstance(sizeType, width, height, temporary_resolution_label);
        }

        private static Type GetType(string type)
        {
            return typeof(EditorWindow).Assembly.GetType("UnityEditor." + type);
        }

        private enum TargetCamera
        {
            GameView = 0,
            SceneView = 1
        }

        private class CustomResolution
        {
            public readonly int Width, Height;

            private bool m_isActive;
            private int m_originalIndex, m_newIndex;

            public CustomResolution(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public bool IsActive
            {
                get { return m_isActive; }
                set
                {
                    if (m_isActive != value)
                    {
                        m_isActive = value;

                        int resolutionIndex;
                        if (m_isActive)
                        {
                            m_originalIndex = (int)GameView.FetchProperty("selectedSizeIndex");

                            object customSize = GetFixedResolution(Width, Height);
                            SizeHolder.CallMethod("AddCustomSize", customSize);
                            m_newIndex = (int)SizeHolder.CallMethod("IndexOf", customSize) + (int)SizeHolder.CallMethod("GetBuiltinCount");
                            resolutionIndex = m_newIndex;
                        }
                        else
                        {
                            SizeHolder.CallMethod("RemoveCustomSize", m_newIndex);
                            resolutionIndex = m_originalIndex;
                        }

                        GameView.CallMethod("SizeSelectionCallback", resolutionIndex, null);
                        GameView.Repaint();
                    }
                }
            }
        }

        [Serializable]
        private class SaveData
        {
            public List<Vector2> _resolutions;
            public List<bool> _resolutionsEnabled;
            public bool _currentResolutionEnabled;
        }

        [Serializable]
        private class SessionData
        {
            public List<Vector2> _resolutions;
            public List<bool> _resolutionsEnabled;
            public bool _currentResolutionEnabled;
            public float _resolutionMultiplier;
            public TargetCamera _targetCamera;
            public bool _captureOverlayUI;
            public bool _setTimeScaleToZero;
            public bool _saveAsPNG;
            public bool _allowTransparentBackground;
            public string _saveDirectory;
        }
    }
}
#endif