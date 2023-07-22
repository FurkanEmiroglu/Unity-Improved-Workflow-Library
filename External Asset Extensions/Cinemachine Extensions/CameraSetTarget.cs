#if IW_CINEMACHINE_EXTENSIONS
using Cinemachine;
using UnityEngine;

namespace IW.CinemachineExtensions
{
    [ExecuteInEditMode][SaveDuringPlay][AddComponentMenu("")]

    public class CameraSetTarget : CinemachineExtension
    {
        [SerializeField]
        private bool m_followTargetEnable;

        [SerializeField]
        private bool m_lookAtTargetEnable;

        private CameraTarget m_cameraTarget;
        private ICinemachineCamera m_cam;

        protected override void Awake()
        {
            base.Awake();
            m_cameraTarget = FindObjectOfType<CameraTarget>();
        }

        private void Start()
        {
            if (m_followTargetEnable && m_cameraTarget != null) VirtualCamera.Follow = m_cameraTarget.transform;
            if (m_lookAtTargetEnable && m_cameraTarget != null) VirtualCamera.LookAt = m_cameraTarget.transform;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {

        }
    }
}
#endif