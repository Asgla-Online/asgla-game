using System;
using AsglaUI.UI.Tweens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/Tab", 58)]
	public class UITab : Toggle {

		public enum TextTransition {

			None,
			ColorTint

		}

		[SerializeField] private GameObject m_TargetContent;

		[SerializeField] private Image m_ImageTarget;
		[SerializeField] private Transition m_ImageTransition = Transition.None;
		[SerializeField] private ColorBlockExtended m_ImageColors = ColorBlockExtended.defaultColorBlock;
		[SerializeField] private SpriteStateExtended m_ImageSpriteState;
		[SerializeField] private AnimationTriggersExtended m_ImageAnimationTriggers = new AnimationTriggersExtended();

		[SerializeField] private TextMeshProUGUI m_TextTarget;
		[SerializeField] private TextTransition m_TextTransition = TextTransition.None;
		[SerializeField] private ColorBlockExtended m_TextColors = ColorBlockExtended.defaultColorBlock;

		// Tween controls
		[NonSerialized] private readonly TweenRunner<ColorTween> m_ColorTweenRunner;

		private SelectionState m_CurrentState = SelectionState.Normal;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UITab() {
			if (m_ColorTweenRunner == null)
				m_ColorTweenRunner = new TweenRunner<ColorTween>();

			m_ColorTweenRunner.Init(this);
		}

		/// <summary>
		///     Gets or sets the target content game object.
		/// </summary>
		public GameObject targetContent {
			get => m_TargetContent;
			set => m_TargetContent = value;
		}

		/// <summary>
		///     Gets or sets the image target.
		/// </summary>
		public Image imageTarget {
			get => m_ImageTarget;
			set => m_ImageTarget = value;
		}

		/// <summary>
		///     Gets or sets the image transition.
		/// </summary>
		public Transition imageTransition {
			get => m_ImageTransition;
			set => m_ImageTransition = value;
		}

		/// <summary>
		///     Gets or sets the image color block.
		/// </summary>
		public ColorBlockExtended imageColors {
			get => m_ImageColors;
			set => m_ImageColors = value;
		}

		/// <summary>
		///     Gets or sets the image sprite state.
		/// </summary>
		public SpriteStateExtended imageSpriteState {
			get => m_ImageSpriteState;
			set => m_ImageSpriteState = value;
		}

		/// <summary>
		///     Gets or sets the image animation triggers.
		/// </summary>
		public AnimationTriggersExtended imageAnimationTriggers {
			get => m_ImageAnimationTriggers;
			set => m_ImageAnimationTriggers = value;
		}

		/// <summary>
		///     Gets or sets the text target.
		/// </summary>
		public TextMeshProUGUI textTarget {
			get => m_TextTarget;
			set => m_TextTarget = value;
		}

		/// <summary>
		///     Gets or sets the text transition.
		/// </summary>
		public TextTransition textTransition {
			get => m_TextTransition;
			set => m_TextTransition = value;
		}

		/// <summary>
		///     Gets or sets the text colors block.
		/// </summary>
		public ColorBlockExtended textColors {
			get => m_TextColors;
			set => m_TextColors = value;
		}

		protected override void Awake() {
			base.Awake();

			// Make sure we have toggle group
			if (group == null) {
				// Try to find the group in the parents
				ToggleGroup grp = UIUtility.FindInParents<ToggleGroup>(gameObject);

				if (grp != null)
					group = grp;
				else
					// Add new group on the parent
					group = transform.parent.gameObject.AddComponent<ToggleGroup>();
			}
		}

		protected override void OnEnable() {
			base.OnEnable();

			// Hook an event listener
			onValueChanged.AddListener(OnToggleStateChanged);

			// Apply initial state
			InternalEvaluateAndTransitionState(true);
		}

		protected override void OnDisable() {
			base.OnDisable();

			// Unhook the event listener
			onValueChanged.RemoveListener(OnToggleStateChanged);
		}

		/// <summary>
		///     Raises the toggle state changed event.
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		protected void OnToggleStateChanged(bool state) {
			if (!IsActive() || !Application.isPlaying)
				return;

			InternalEvaluateAndTransitionState(false);
		}

		/// <summary>
		///     Evaluates and toggles the content visibility.
		/// </summary>
		public void EvaluateAndToggleContent() {
			if (m_TargetContent != null)
				m_TargetContent.SetActive(isOn);
		}

		/// <summary>
		///     Internaly evaluates and transitions to the current state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionState(bool instant) {
			if (!isActiveAndEnabled)
				return;

			// Toggle the content
			EvaluateAndToggleContent();

#if UNITY_EDITOR
			// Transition the active graphic
			// Hackfix because unity is not toggling it in edit mode
			if (instant && !Application.isPlaying && graphic != null)
				graphic.canvasRenderer.SetAlpha(!isOn ? 0f : 1f);
#endif

			// Transition the active graphic children
			if (graphic != null && graphic.transform.childCount > 0) {
				float targetAlpha = !isOn ? 0f : 1f;

				// Loop through the children
				foreach (Transform child in graphic.transform) {
					// Try getting a graphic component
					Graphic g = child.GetComponent<Graphic>();

					if (g != null)
						if (!g.canvasRenderer.GetAlpha().Equals(targetAlpha)) {
							if (instant) g.canvasRenderer.SetAlpha(targetAlpha);
							else g.CrossFadeAlpha(targetAlpha, 0.1f, true);
						}
				}
			}

			// Do a state transition
			DoStateTransition(m_CurrentState, instant);
		}

		/// <summary>
		///     Does the state transitioning.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected override void DoStateTransition(SelectionState state, bool instant) {
			if (!isActiveAndEnabled)
				return;

			// Save the state as current state
			m_CurrentState = state;

			Color newImageColor = m_ImageColors.normalColor;
			Color newTextColor = m_TextColors.normalColor;
			Sprite newSprite = null;
			string imageTrigger = m_ImageAnimationTriggers.normalTrigger;

			// Prepare state values
			switch (state) {
				case SelectionState.Normal:
					newImageColor = !isOn ? m_ImageColors.normalColor : m_ImageColors.activeColor;
					newTextColor = !isOn ? m_TextColors.normalColor : m_TextColors.activeColor;
					newSprite = !isOn ? null : m_ImageSpriteState.activeSprite;
					imageTrigger =
						!isOn ? m_ImageAnimationTriggers.normalTrigger : m_ImageAnimationTriggers.activeTrigger;
					break;
				case SelectionState.Highlighted:
					newImageColor = !isOn ? m_ImageColors.highlightedColor : m_ImageColors.activeHighlightedColor;
					newTextColor = !isOn ? m_TextColors.highlightedColor : m_TextColors.activeHighlightedColor;
					newSprite = !isOn
						? m_ImageSpriteState.highlightedSprite
						: m_ImageSpriteState.activeHighlightedSprite;
					imageTrigger = !isOn
						? m_ImageAnimationTriggers.highlightedTrigger
						: m_ImageAnimationTriggers.activeHighlightedTrigger;
					break;
				case SelectionState.Pressed:
					newImageColor = !isOn ? m_ImageColors.pressedColor : m_ImageColors.activePressedColor;
					newTextColor = !isOn ? m_TextColors.pressedColor : m_TextColors.activePressedColor;
					newSprite = !isOn ? m_ImageSpriteState.pressedSprite : m_ImageSpriteState.activePressedSprite;
					imageTrigger = !isOn
						? m_ImageAnimationTriggers.pressedTrigger
						: m_ImageAnimationTriggers.activePressedTrigger;
					break;
				case SelectionState.Disabled:
					newImageColor = m_ImageColors.disabledColor;
					newTextColor = m_TextColors.disabledColor;
					newSprite = m_ImageSpriteState.disabledSprite;
					imageTrigger = m_ImageAnimationTriggers.disabledTrigger;
					break;
			}

			// Check if the tab is active in the scene
			if (gameObject.activeInHierarchy) {
				// Do the image transition
				switch (m_ImageTransition) {
					case Transition.ColorTint:
						StartColorTween(m_ImageTarget, newImageColor * m_ImageColors.colorMultiplier,
							instant ? 0f : m_ImageColors.fadeDuration);
						break;
					case Transition.SpriteSwap:
						DoSpriteSwap(m_ImageTarget, newSprite);
						break;
					case Transition.Animation:
						TriggerAnimation(m_ImageTarget.gameObject, imageTrigger);
						break;
				}

				// Do the text transition
				switch (m_TextTransition) {
					case TextTransition.ColorTint:
						StartColorTweenText(newTextColor * m_TextColors.colorMultiplier,
							instant ? 0f : m_TextColors.fadeDuration);
						break;
				}
			}
		}

		/// <summary>
		///     Starts a color tween.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="targetColor">Target color.</param>
		/// <param name="duration">Duration.</param>
		private void StartColorTween(Graphic target, Color targetColor, float duration) {
			if (target == null)
				return;

			if (!Application.isPlaying || duration == 0f)
				target.canvasRenderer.SetColor(targetColor);
			else
				target.CrossFadeColor(targetColor, duration, true, true);
		}

		/// <summary>
		///     Starts a color tween.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		/// <param name="duration">Duration.</param>
		private void StartColorTweenText(Color targetColor, float duration) {
			if (m_TextTarget == null)
				return;

			if (!Application.isPlaying || duration == 0f) {
				m_TextTarget.color = targetColor;
			} else {
				ColorTween colorTween = new ColorTween
					{duration = duration, startColor = m_TextTarget.color, targetColor = targetColor};
				colorTween.AddOnChangedCallback(SetTextColor);
				colorTween.ignoreTimeScale = true;

				m_ColorTweenRunner.StartTween(colorTween);
			}
		}

		/// <summary>
		///     Sets the color of the text.
		/// </summary>
		/// <param name="color">Color.</param>
		private void SetTextColor(Color color) {
			if (m_TextTarget == null)
				return;

			m_TextTarget.color = color;
		}

		/// <summary>
		///     Does a sprite swap.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="newSprite">New sprite.</param>
		private void DoSpriteSwap(Image target, Sprite newSprite) {
			if (target == null)
				return;

			if (!target.overrideSprite.Equals(newSprite))
				target.overrideSprite = newSprite;
		}

		/// <summary>
		///     Triggers the animation.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="triggername">Triggername.</param>
		private void TriggerAnimation(GameObject target, string triggername) {
			if (target == null)
				return;

			Animator animator = target.GetComponent<Animator>();

			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(triggername))
				return;

			animator.ResetTrigger(m_ImageAnimationTriggers.normalTrigger);
			animator.ResetTrigger(m_ImageAnimationTriggers.pressedTrigger);
			animator.ResetTrigger(m_ImageAnimationTriggers.highlightedTrigger);
			animator.ResetTrigger(m_ImageAnimationTriggers.activeTrigger);
			animator.ResetTrigger(m_ImageAnimationTriggers.activeHighlightedTrigger);
			animator.ResetTrigger(m_ImageAnimationTriggers.activePressedTrigger);
			animator.ResetTrigger(m_ImageAnimationTriggers.disabledTrigger);
			animator.SetTrigger(triggername);
		}

		/// <summary>
		///     Activate the tab.
		/// </summary>
		public void Activate() {
			if (!isOn)
				isOn = true;
		}

#if UNITY_EDITOR
		protected override void OnValidate() {
			base.OnValidate();

			m_ImageColors.fadeDuration = Mathf.Max(m_ImageColors.fadeDuration, 0f);
			m_TextColors.fadeDuration = Mathf.Max(m_TextColors.fadeDuration, 0f);
		}

		/// <summary>
		///     Raises the property change event from the editor.
		/// </summary>
		public void OnProperyChange_Editor() {
			if (!isActiveAndEnabled)
				return;

			DoSpriteSwap(m_ImageTarget, null);
			InternalEvaluateAndTransitionState(true);
		}
#endif

	}
}