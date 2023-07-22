#if IW_CINEMACHINE_EXTENSIONS
using Cinemachine;
using UnityEngine;

namespace IW.CinemachineExtensions
{
    [ExecuteInEditMode][SaveDuringPlay][AddComponentMenu("")]

    public class LockCameraAxis : CinemachineExtension
    {
        [SerializeField]
        private LockedAxis m_lockedAxis;

        [SerializeField]
        private float m_lockedPosition;

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                pos = LockPosition(m_lockedAxis, pos);
                state.RawPosition = pos;
            }
        }

        private Vector3 LockPosition(LockedAxis axis, Vector3 position)
        {
            switch (axis)
            {
                case LockedAxis.X:
                    position.x = m_lockedPosition;
                    break;
                case LockedAxis.Y:
                    position.y = m_lockedPosition;
                    break;
                case LockedAxis.Z:
                    position.z = m_lockedPosition;
                    break;
            }

            return position;
        }
    }
}

/// <summary>
/// Add-on module for locking CinemachineCamera on a certain Axis and not following along
/// </summary>

public enum LockedAxis {X, Y, Z}
#endif