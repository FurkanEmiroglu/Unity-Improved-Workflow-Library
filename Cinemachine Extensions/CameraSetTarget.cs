// #define CINEMACHINE
// #define ODIN_INSPECTOR

#if ODIN_INSPECTOR
#if CINEMACHINE
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ImprovedWorkflow.CinemachineExtensions
{
    [ExecuteInEditMode][SaveDuringPlay][AddComponentMenu("")]

    public class CameraSetTarget : CinemachineExtension
    {
        [InfoBox("Finds the game object with CameraTarget component in scene, and follows/looks at it" +
                 "if corresponding option is enabled")]
        [SerializeField]
        private bool followTargetEnable;

        [SerializeField]
        private bool lookAtTargetEnable;

        private CameraTarget cameraTarget;
        private ICinemachineCamera cam;

        protected override void Awake()
        {
            base.Awake();
            cameraTarget = FindObjectOfType<CameraTarget>();
        }

        private void Start()
        {
            if (followTargetEnable && cameraTarget != null) VirtualCamera.Follow = cameraTarget.transform;
            if (lookAtTargetEnable && cameraTarget != null) VirtualCamera.LookAt = cameraTarget.transform;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {

        }
    }
}
#endif
#endif