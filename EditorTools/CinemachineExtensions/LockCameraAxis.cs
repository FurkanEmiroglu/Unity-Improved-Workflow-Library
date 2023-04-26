#if CINEMACHINE
using Cinemachine;
using UnityEngine;

namespace ImprovedWorkflow.EditorTools.CinemachineExtensions
{
    [ExecuteInEditMode][SaveDuringPlay][AddComponentMenu("")]

    public class LockCameraAxis : CinemachineExtension
    {
        [SerializeField]
        private LockedAxis lockedAxis;

        [SerializeField]
        private float lockedPosition;

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                pos = LockPosition(lockedAxis, pos);
                state.RawPosition = pos;
            }
        }

        private Vector3 LockPosition(LockedAxis axis, Vector3 position)
        {
            switch (axis)
            {
                case LockedAxis.X:
                    position.x = lockedPosition;
                    break;
                case LockedAxis.Y:
                    position.y = lockedPosition;
                    break;
                case LockedAxis.Z:
                    position.z = lockedPosition;
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