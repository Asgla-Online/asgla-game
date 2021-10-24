using Cinemachine;
using UnityEngine;

/// <summary>
///     An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
/// </summary>
public class CinemachineLockCameraY : CinemachineExtension {

	[Tooltip("Lock the camera's Y position to this value")]
	public float m_YPosition;

	protected override void PostPipelineStageCallback(
		CinemachineVirtualCameraBase vcam,
		CinemachineCore.Stage stage,
		ref CameraState state,
		float deltaTime) {
		if (stage == CinemachineCore.Stage.Body) {
			Vector3 pos = state.RawPosition;
			pos.y = m_YPosition;
			state.RawPosition = pos;
		}
	}

}