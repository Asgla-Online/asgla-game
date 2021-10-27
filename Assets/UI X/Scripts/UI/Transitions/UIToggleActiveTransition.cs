using System;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Toggle Active Transition")]
	public class UIToggleActiveTransition : MonoBehaviour, IEventSystemHandler {

		public enum Transition {

			None,
			ColorTint,
			SpriteSwap,
			Animation,
			TextColor,
			CanvasGroup

		}

		public enum VisualState {

			Normal,
			Active

		}

		// Tween controls
		[NonSerialized] private readonly TweenRunner<ColorTween> m_ColorTweenRunner;

		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		private bool m_Active;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UIToggleActiveTransition() {
			if (m_ColorTweenRunner == null)
				m_ColorTweenRunner = new TweenRunner<ColorTween>();

			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_ColorTweenRunner.Init(this);
			m_FloatTweenRunner.Init(this);
		}

		/// <summary>
		///     Gets or sets the transition type.
		/// </summary>
		/// <value>The transition.</value>
		public Transition transition {
			get => m_Transition;
			set => m_Transition = value;
		}

		/// <summary>
		///     Gets or sets the target graphic.
		/// </summary>
		/// <value>The target graphic.</value>
		public Graphic targetGraphic {
			get => m_TargetGraphic;
			set => m_TargetGraphic = value;
		}

		/// <summary>
		///     Gets or sets the target game object.
		/// </summary>
		/// <value>The target game object.</value>
		public GameObject targetGameObject {
			get => m_TargetGameObject;
			set => m_TargetGameObject = value;
		}

		/// <summary>
		///     Gets or sets the target canvas group.
		/// </summary>
		/// <value>The target canvas group.</value>
		public CanvasGroup targetCanvasGroup {
			get => m_TargetCanvasGroup;
			set => m_TargetCanvasGroup = value;
		}

		/// <summary>
		///     Gets the animator.
		/// </summary>
		/// <value>The animator.</value>
		public Animator animator {
			get{
				if (m_TargetGameObject != null)
					return m_TargetGameObject.GetComponent<Animator>();

				// Default
				return null;
			}
		}

		protected void Awake() {
			if (m_TargetToggle == null)
				m_TargetToggle = gameObject.GetComponent<Toggle>();

			if (m_TargetToggle != null)
				m_Active = m_TargetToggle.isOn;
		}

		protected void OnEnable() {
			if (m_TargetToggle != null)
				m_TargetToggle.onValueChanged.AddListener(OnToggleValueChange);

			InternalEvaluateAndTransitionToNormalState(true);
		}

		protected void OnDisable() {
			if (m_TargetToggle != null)
				m_TargetToggle.onValueChanged.RemoveListener(OnToggleValueChange);

			InstantClearState();
		}

#if UNITY_EDITOR
		protected void OnValidate() {
			m_Duration = Mathf.Max(m_Duration, 0f);

			if (isActiveAndEnabled) {
				DoSpriteSwap(null);

				if (m_Transition != Transition.CanvasGroup)
					InternalEvaluateAndTransitionToNormalState(true);
			}
		}
#endif

		/// <summary>
		///     Internally evaluates and transitions to normal state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionToNormalState(bool instant) {
			DoStateTransition(m_Active ? VisualState.Active : VisualState.Normal, instant);
		}

		protected void OnToggleValueChange(bool value) {
			if (m_TargetToggle == null)
				return;

			m_Active = m_TargetToggle.isOn;

			if (m_Transition == Transition.Animation) {
				if (targetGameObject == null || animator == null || !animator.isActiveAndEnabled ||
				    animator.runtimeAnimatorController == null || string.IsNullOrEmpty(m_ActiveBool))
					return;

				animator.SetBool(m_ActiveBool, m_Active);
			}

			DoStateTransition(m_Active ? VisualState.Active : VisualState.Normal, false);
		}

		/// <summary>
		///     Instantly clears the visual state.
		/// </summary>
		protected void InstantClearState() {
			switch (m_Transition) {
				case Transition.ColorTint:
					StartColorTween(Color.white, true);
					break;
				case Transition.SpriteSwap:
					DoSpriteSwap(null);
					break;
				case Transition.TextColor:
					SetTextColor(m_NormalColor);
					break;
				case Transition.CanvasGroup:
					SetCanvasGroupAlpha(1f);
					break;
			}
		}

		/// <summary>
		///     Does the state transition.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void DoStateTransition(VisualState state, bool instant) {
			// Check if active in the scene
			if (!gameObject.activeInHierarchy)
				return;

			Color color = m_NormalColor;
			Sprite newSprite = null;
			string triggername = m_NormalTrigger;
			float alpha = m_NormalAlpha;

			// Prepare the transition values
			switch (state) {
				case VisualState.Normal:
					color = m_NormalColor;
					newSprite = null;
					triggername = m_NormalTrigger;
					alpha = m_NormalAlpha;
					break;
				case VisualState.Active:
					color = m_ActiveColor;
					newSprite = m_ActiveSprite;
					triggername = m_NormalTrigger;
					alpha = m_ActiveAlpha;
					break;
			}

			// Do the transition
			switch (m_Transition) {
				case Transition.ColorTint:
					StartColorTween(color * m_ColorMultiplier, instant);
					break;
				case Transition.SpriteSwap:
					DoSpriteSwap(newSprite);
					break;
				case Transition.Animation:
					TriggerAnimation(triggername);
					break;
				case Transition.TextColor:
					StartTextColorTween(color, false);
					break;
				case Transition.CanvasGroup:
					StartCanvasGroupTween(alpha, instant);
					break;
			}
		}

		/// <summary>
		///     Starts the color tween.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void StartColorTween(Color targetColor, bool instant) {
			if (m_TargetGraphic == null)
				return;

			if (instant || m_Duration == 0f || !Application.isPlaying)
				m_TargetGraphic.canvasRenderer.SetColor(targetColor);
			else
				m_TargetGraphic.CrossFadeColor(targetColor, m_Duration, true, true);
		}

		private void DoSpriteSwap(Sprite newSprite) {
			Image image = m_TargetGraphic as Image;

			if (image == null)
				return;

			image.overrideSprite = newSprite;
		}

		private void TriggerAnimation(string triggername) {
			if (targetGameObject == null || animator == null || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(triggername))
				return;

			animator.ResetTrigger(m_NormalTrigger);
			animator.SetTrigger(triggername);
		}

		private void StartTextColorTween(Color targetColor, bool instant) {
			if (m_TargetGraphic == null)
				return;

			if (m_TargetGraphic is Text == false)
				return;

			if (instant || m_Duration == 0f || !Application.isPlaying) {
				(m_TargetGraphic as Text).color = targetColor;
			} else {
				ColorTween colorTween = new ColorTween
					{duration = m_Duration, startColor = (m_TargetGraphic as Text).color, targetColor = targetColor};
				colorTween.AddOnChangedCallback(SetTextColor);
				colorTween.ignoreTimeScale = true;

				m_ColorTweenRunner.StartTween(colorTween);
			}
		}

		/// <summary>
		///     Sets the text color.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		private void SetTextColor(Color targetColor) {
			if (m_TargetGraphic == null)
				return;

			if (m_TargetGraphic is Text)
				(m_TargetGraphic as Text).color = targetColor;
		}

		/// <summary>
		///     Starts the color tween.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void StartCanvasGroupTween(float targetAlpha, bool instant) {
			if (m_TargetCanvasGroup == null)
				return;

			if (instant || m_Duration == 0f || !Application.isPlaying) {
				SetCanvasGroupAlpha(targetAlpha);
			} else {
				FloatTween floatTween = new FloatTween
					{duration = m_Duration, startFloat = m_TargetCanvasGroup.alpha, targetFloat = targetAlpha};
				floatTween.AddOnChangedCallback(SetCanvasGroupAlpha);
				floatTween.ignoreTimeScale = true;

				m_FloatTweenRunner.StartTween(floatTween);
			}
		}

		/// <summary>
		///     Sets the canvas group alpha value.
		/// </summary>
		/// <param name="alpha">The alpha value.</param>
		private void SetCanvasGroupAlpha(float alpha) {
			if (m_TargetCanvasGroup == null)
				return;

			m_TargetCanvasGroup.alpha = alpha;
		}

#pragma warning disable 0649
		[SerializeField] private Transition m_Transition = Transition.None;

		[SerializeField] private Color m_NormalColor = new Color(1f, 1f, 1f, 0f);
		[SerializeField] private Color m_ActiveColor = Color.white;
		[SerializeField] private float m_Duration = 0.1f;

		[SerializeField] [Range(1f, 6f)] private float m_ColorMultiplier = 1f;

		[SerializeField] private Sprite m_ActiveSprite;

		[SerializeField] private string m_NormalTrigger = "Normal";
		[SerializeField] private string m_ActiveBool = "Active";

		[SerializeField] [Range(0f, 1f)] private float m_NormalAlpha;
		[SerializeField] [Range(0f, 1f)] private float m_ActiveAlpha = 1f;

		[SerializeField] [Tooltip("Graphic that will have the selected transtion applied.")]
		private Graphic m_TargetGraphic;

		[SerializeField] [Tooltip("GameObject that will have the selected transtion applied.")]
		private GameObject m_TargetGameObject;

		[SerializeField] [Tooltip("CanvasGroup that will have the selected transtion applied.")]
		private CanvasGroup m_TargetCanvasGroup;

		[SerializeField] private Toggle m_TargetToggle;
#pragma warning restore 0649

	}
}