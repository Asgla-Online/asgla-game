using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Joystick", 36)]
	[RequireComponent(typeof(RectTransform))]
	public class UIJoystick : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler,
		IPointerUpHandler {

		public enum AxisOption {

			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical

		}

		[SerializeField] [Tooltip("The child graphic that will be moved around.")]
		private RectTransform m_Handle;

		[SerializeField] [Tooltip("The handling area that the handle is allowed to be moved in.")]
		private RectTransform m_HandlingArea;

		[SerializeField] [Tooltip("The child graphic that will be shown when the joystick is active.")]
		private Image m_ActiveGraphic;

		[SerializeField] private Vector2 m_Axis;

		[SerializeField]
		private AxisOption m_AxesToUse = AxisOption.Both; // The options for the axes that the still will use

		[SerializeField] [Tooltip("How fast the joystick will go back to the center")]
		private float m_Spring = 25f;

		[SerializeField] [Tooltip("How close to the center that the axis will be output as 0")]
		private float m_DeadZone = 0.1f;

		[Tooltip("Customize the output that is sent in OnValueChange")]
		public AnimationCurve outputCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

		[SerializeField] private string
			m_HorizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input

		[SerializeField] private string
			m_VerticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

		private CrossPlatformInputManager.VirtualAxis
			m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input

		private bool m_IsDragging;
		private bool m_UseX; // Toggle for using the x axis
		private bool m_UseY; // Toggle for using the Y axis

		private CrossPlatformInputManager.VirtualAxis
			m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

		public RectTransform Handle {
			get => m_Handle;
			set{
				m_Handle = value;
				UpdateHandle();
			}
		}

		public RectTransform HandlingArea {
			get => m_HandlingArea;
			set => m_HandlingArea = value;
		}

		public Image ActiveGraphic {
			get => m_ActiveGraphic;
			set => m_ActiveGraphic = value;
		}

		public float Spring {
			get => m_Spring;
			set => m_Spring = value;
		}

		public float DeadZone {
			get => m_DeadZone;
			set => m_DeadZone = value;
		}

		public Vector2 JoystickAxis {
			get{
				Vector2 outputPoint = m_Axis.magnitude > m_DeadZone ? m_Axis : Vector2.zero;
				float magnitude = outputPoint.magnitude;

				outputPoint *= outputCurve.Evaluate(magnitude);

				return outputPoint;
			}
			set => SetAxis(value);
		}

		protected void LateUpdate() {
			if (isActiveAndEnabled && !m_IsDragging)
				if (m_Axis != Vector2.zero) {
					Vector2 newAxis = m_Axis - m_Axis * Time.unscaledDeltaTime * m_Spring;

					if (newAxis.sqrMagnitude <= .0001f)
						newAxis = Vector2.zero;

					SetAxis(newAxis);
				}
		}

		protected override void OnEnable() {
			base.OnEnable();
			CreateVirtualAxes();

			if (m_HandlingArea == null)
				m_HandlingArea = transform as RectTransform;

			if (m_ActiveGraphic != null)
				m_ActiveGraphic.canvasRenderer.SetAlpha(0f);
		}

#if UNITY_EDITOR
		protected override void OnValidate() {
			base.OnValidate();

			// Fix anchors
			if (m_HandlingArea != null) {
				m_HandlingArea.anchorMin = new Vector2(0.5f, 0.5f);
				m_HandlingArea.anchorMax = new Vector2(0.5f, 0.5f);
			}

			// Hide active
			if (m_ActiveGraphic != null)
				m_ActiveGraphic.canvasRenderer.SetAlpha(0f);

			UpdateHandle();
		}
#endif

		public void OnBeginDrag(PointerEventData eventData) {
			if (!IsActive() || m_HandlingArea == null)
				return;

			Vector2 newAxis = m_HandlingArea.InverseTransformPoint(eventData.position);
			newAxis.x /= m_HandlingArea.sizeDelta.x * 0.5f;
			newAxis.y /= m_HandlingArea.sizeDelta.y * 0.5f;

			SetAxis(newAxis);
			m_IsDragging = true;
		}

		public void OnDrag(PointerEventData eventData) {
			if (m_HandlingArea == null)
				return;

			Vector2 axis = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandlingArea, eventData.position,
				eventData.pressEventCamera, out axis);

			axis -= m_HandlingArea.rect.center;
			axis.x /= m_HandlingArea.sizeDelta.x * 0.5f;
			axis.y /= m_HandlingArea.sizeDelta.y * 0.5f;

			SetAxis(axis);
		}

		public void OnEndDrag(PointerEventData eventData) {
			m_IsDragging = false;
		}

		public void OnPointerDown(PointerEventData eventData) {
			if (m_ActiveGraphic != null)
				m_ActiveGraphic.CrossFadeAlpha(1f, 0.2f, false);
		}

		public void OnPointerUp(PointerEventData eventData) {
			if (m_ActiveGraphic != null)
				m_ActiveGraphic.CrossFadeAlpha(0f, 0.2f, false);
		}

		protected void CreateVirtualAxes() {
			// set axes to use
			m_UseX = m_AxesToUse == AxisOption.Both || m_AxesToUse == AxisOption.OnlyHorizontal;
			m_UseY = m_AxesToUse == AxisOption.Both || m_AxesToUse == AxisOption.OnlyVertical;

			if (m_UseX) {
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(m_HorizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}

			if (m_UseY) {
				m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(m_VerticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
			}
		}

		public void SetAxis(Vector2 axis) {
			m_Axis = Vector2.ClampMagnitude(axis, 1);

			Vector2 outputPoint = m_Axis.magnitude > m_DeadZone ? m_Axis : Vector2.zero;
			float magnitude = outputPoint.magnitude;

			outputPoint *= outputCurve.Evaluate(magnitude);

			if (m_UseX)
				m_HorizontalVirtualAxis.Update(outputPoint.x);
			if (m_UseY)
				m_VerticalVirtualAxis.Update(outputPoint.y);

			UpdateHandle();
		}

		private void UpdateHandle() {
			if (m_Handle && m_HandlingArea)
				m_Handle.anchoredPosition = new Vector2(m_Axis.x * m_HandlingArea.sizeDelta.x * 0.5f,
					m_Axis.y * m_HandlingArea.sizeDelta.y * 0.5f);
		}

	}
}