using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace IW.EditorExtensions
{
    public class LightingHelper : EditorWindow
    {
        private static readonly Vector2 s_size = new(300, 500);
        
        private AmbientMode m_ambientMode;
        
        private Color m_equatorColor;
        private Color m_fogColor;
        private Color m_groundColor;
        private Color m_skyColor;
        
        private FogMode m_fogMode;
        
        private float m_endDistance;
        private float m_fogDensity;
        private float m_startDistance;
        private bool m_isFogEnabled;
        private Material m_skyboxMaterial;

        private void OnEnable()
        {
            m_skyboxMaterial = RenderSettings.skybox;
            m_ambientMode = RenderSettings.ambientMode;
            m_skyColor = RenderSettings.ambientSkyColor;
            m_equatorColor = RenderSettings.ambientEquatorColor;
            m_groundColor = RenderSettings.ambientGroundColor;

            m_isFogEnabled = RenderSettings.fog;
            m_fogColor = RenderSettings.fogColor;
            m_fogMode = RenderSettings.fogMode;
            m_fogDensity = RenderSettings.fogDensity;
            m_startDistance = RenderSettings.fogStartDistance;
            m_endDistance = RenderSettings.fogEndDistance;
        }

        private void OnGUI()
        {
            SetValues();

            if (GUILayout.Button("Replace lighting settings on all build scenes"))
                if (Warn())
                    ReplaceLights();
        }

        [MenuItem("Tools/Workflow/Scene Tools/Lighting Helper", false, 1)]
        private static void Init()
        {
            LightingHelper window = (LightingHelper)GetWindow(typeof(LightingHelper));

            window.minSize = s_size;
            window.maxSize = s_size;

            window.Show();
        }

        private void SetValues()
        {
            m_skyboxMaterial =
                (Material)EditorGUILayout.ObjectField("SkyBox Material", m_skyboxMaterial, typeof(Material), true);
            m_ambientMode = (AmbientMode)EditorGUILayout.EnumPopup("Ambient Mode", m_ambientMode);

            if (m_ambientMode != AmbientMode.Skybox)
            {
                m_skyColor = EditorGUILayout.ColorField(new GUIContent("Skybox Color"), m_skyColor, true, false, true);

                if (m_ambientMode == AmbientMode.Trilight)
                {
                    m_equatorColor = EditorGUILayout.ColorField(new GUIContent("Equator Color"), m_equatorColor, true,
                        false, true);
                    m_groundColor =
                        EditorGUILayout.ColorField(new GUIContent("Ground Color"), m_groundColor, true, false, true);
                }
            }

            m_isFogEnabled = EditorGUILayout.Toggle("Fog", m_isFogEnabled);

            if (m_isFogEnabled)
            {
                m_fogColor = EditorGUILayout.ColorField(new GUIContent("Fog Color"), m_fogColor, true, true, false);
                m_fogMode = (FogMode)EditorGUILayout.EnumPopup("Fog Mode", m_fogMode);

                if (m_fogMode != FogMode.Linear)
                {
                    m_fogDensity = EditorGUILayout.FloatField("Fog Density", m_fogDensity);
                }
                else
                {
                    m_startDistance = EditorGUILayout.FloatField("Start Density", m_startDistance);
                    m_endDistance = EditorGUILayout.FloatField("Fog Density", m_endDistance);
                }
            }
        }

        private void ReplaceLights()
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                EditorSceneManager.OpenScene(scene.path);

                RenderSettings.skybox = m_skyboxMaterial;

                RenderSettings.ambientMode = m_ambientMode;

                RenderSettings.ambientSkyColor = m_skyColor;
                RenderSettings.ambientEquatorColor = m_equatorColor;
                RenderSettings.ambientGroundColor = m_groundColor;

                RenderSettings.fog = m_isFogEnabled;
                RenderSettings.fogMode = m_fogMode;

                RenderSettings.fogColor = m_fogColor;

                RenderSettings.fogStartDistance = m_startDistance;
                RenderSettings.fogEndDistance = m_endDistance;

                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }
        }

        private bool Warn()
        {
            return EditorUtility.DisplayDialog("Warning",
                "This will replace lighting settings of all build scenes. Are you sure?", "Yes", "No");
        }
    }
}