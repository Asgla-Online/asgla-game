using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Select Field - Transition", 58)]
	[RequireComponent(typeof(UISelectField))]
	public class UISelectField_Transition : MonoBehaviour {

		[SerializeField] [Tooltip("Graphic that will have the selected transtion applied.")]
		private Graphic m_TargetGraphic;

		[SerializeField] [Tooltip("GameObject that will have the selected transtion applied.")]
		private GameObject m_TargetGameObject;

		[SerializeField] private Selectable.Transition m_Transition = Selectable.Transition.None;
		[SerializeField] private ColorBlockExtended m_Colors = ColorBlockExtended.defaultColorBlock;
		[SerializeField] private SpriteStateExtended m_SpriteState;
		[SerializeField] private AnimationTriggersExtended m_AnimationTriggers = new AnimationTriggersExtended();

		/// <summary>
		///     The select field.
		/// </summary>
		private UISelectField m_Select;

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

		/// <summary>
		///     Gets or sets the transition type.
		/// </summary>
		public Selectable.Transition transition {
			get => m_Transition;
			set => m_Transition = value;
		}

		/// <summary>
		///     Gets or sets the color block.
		/// </summary>
		public ColorBlockExtended colors {
			get => m_Colors;
			set => m_Colors = value;
		}

		/// <summary>
		///     Gets or sets the sprite state.
		/// </summary>
		public SpriteStateExtended spriteState {
			get => m_SpriteState;
			set => m_SpriteState = value;
		}

		/// <summary>
		///     Gets or sets the animation triggers.
		/// </summary>
		public AnimationTriggersExtended animationTriggers {
			get => m_AnimationTriggers;
			set => m_AnimationTriggers = value;
		}

		protected void Awake() {
			m_Select = gameObject.GetComponent<UISelectField>();
		}

		protected void OnEnable() {
			if (m_Select != null)
				m_Select.onTransition.AddListener(OnTransition);

			OnTransition(UISelectField.VisualState.Normal, true);
		}

		protected void OnDisable() {
			if (m_Select != null)
				m_Select.onTransition.RemoveListener(OnTransition);

			InstantClearState();
		}

		/// <summary>
		///     Instantly clears the visual state.
		/// </summary>
		protected void InstantClearState() {
			switch (m_Transition) {
				case Selectable.Transition.ColorTint:
					StartColorTween(Color.white, true);
					break;
				case Selectable.Transition.SpriteSwap:
					DoSpriteSwap(null);
					break;
			}
		}

		public void OnTransition(UISelectField.VisualState state, bool instant) {
			if (targetGraphic == null && targetGameObject == null || !gameObject.activeInHierarchy ||
			    m_Transition == Selectable.Transition.None)
				return;

			Color color = colors.normalColor;
			Sprite newSprite = null;
			string triggername = animationTriggers.normalTrigger;

			// Prepare the state values
			switch (state) {
				case UISelectField.VisualState.Normal:
					color = colors.normalColor;
					newSprite = null;
					triggername = animationTriggers.normalTrigger;
					break;
				case UISelectField.VisualState.Highlighted:
					color = colors.highlightedColor;
					newSprite = spriteState.highlightedSprite;
					triggername = animationTriggers.highlightedTrigger;
					break;
				case UISelectField.VisualState.Pressed:
					color = colors.pressedColor;
					newSprite = spriteState.pressedSprite;
					triggername = animationTriggers.pressedTrigger;
					break;
				case UISelectField.VisualState.Active:
					color = colors.activeColor;
					newSprite = spriteState.activeSprite;
					triggername = animationTriggers.activeTrigger;
					break;
				case UISelectField.VisualState.ActiveHighlighted:
					color = colors.activeHighlightedColor;
					newSprite = spriteState.activeHighlightedSprite;
					triggername = animationTriggers.activeHighlightedTrigger;
					break;
				case UISelectField.VisualState.ActivePressed:
					color = colors.activePressedColor;
					newSprite = spriteState.activePressedSprite;
					triggername = animationTriggers.activePressedTrigger;
					break;
				case UISelectField.VisualState.Disabled:
					color = colors.disabledColor;
					newSprite = spriteState.disabledSprite;
					triggername = animationTriggers.disabledTrigger;
					break;
			}

			// Do the transition
			switch (m_Transition) {
				case Selectable.Transition.ColorTint:
					StartColorTween(color * colors.colorMultiplier, instant ? true : colors.fadeDuration == 0f);
					break;
				case Selectable.Transition.SpriteSwap:
					DoSpriteSwap(newSprite);
					break;
				case Selectable.Transition.Animation:
					TriggerAnimation(triggername);
					break;
			}
		}

		private void StartColorTween(Color color, bool instant) {
			if (targetGraphic == null)
				return;

			if (instant)
				targetGraphic.canvasRenderer.SetColor(color);
			else
				targetGraphic.CrossFadeColor(color, colors.fadeDuration, true, true);
		}

		private void DoSpriteSwap(Sprite newSprite) {
			if (targetGraphic == null)
				return;

			Image image = targetGraphic as Image;

			if (image != null)
				image.overrideSprite = newSprite;
		}

		private void TriggerAnimation(string trigger) {
			Animator animator = GetComponent<Animator>();

			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(trigger))
				return;

			animator.ResetTrigger(animationTriggers.normalTrigger);
			animator.ResetTrigger(animationTriggers.pressedTrigger);
			animator.ResetTrigger(animationTriggers.highlightedTrigger);
			animator.ResetTrigger(animationTriggers.activeTrigger);
			animator.ResetTrigger(animationTriggers.activeHighlightedTrigger);
			animator.ResetTrigger(animationTriggers.activePressedTrigger);
			animator.ResetTrigger(animationTriggers.disabledTrigger);
			animator.SetTrigger(trigger);
		}

	}
}