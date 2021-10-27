using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class UISelectField_Option : Toggle, IPointerClickHandler {

		/// <summary>
		///     The select field referrence.
		/// </summary>
		public UISelectField selectField;

		/// <summary>
		///     The text component referrence.
		/// </summary>
		public Text textComponent;

		/// <summary>
		///     The On Select Option event.
		/// </summary>
		public SelectOptionEvent onSelectOption = new SelectOptionEvent();

		/// <summary>
		///     The On Pointer Up event.
		/// </summary>
		public PointerUpEvent onPointerUp = new PointerUpEvent();

		protected override void Start() {
			base.Start();

			// Disable navigation
			Navigation nav = new Navigation();
			nav.mode = Navigation.Mode.None;
			navigation = nav;

			// Set initial transition to none
			transition = Transition.None;

			// Disable toggle transition
			toggleTransition = ToggleTransition.None;
		}

#if UNITY_EDITOR
		protected override void OnValidate() {
			base.OnValidate();

			if (selectField != null)
				selectField.optionBackgroundTransColors.fadeDuration =
					Mathf.Max(selectField.optionBackgroundTransColors.fadeDuration, 0f);
		}
#endif

		public override void OnPointerClick(PointerEventData eventData) {
			base.OnPointerClick(eventData);

			// Transition to the correct state
			DoStateTransition(SelectionState.Normal, false);

			// Invoke the select event
			if (onSelectOption != null && textComponent != null)
				onSelectOption.Invoke(textComponent.text);
		}

		/// <summary>
		///     Initialize the option.
		/// </summary>
		/// <param name="select">Select.</param>
		/// <param name="text">Text.</param>
		public void Initialize(UISelectField select, Text text) {
			selectField = select;
			textComponent = text;
			OnEnable();
		}

		/// <summary>
		///     Determines whether this option is pressed.
		/// </summary>
		/// <returns><c>true</c> if this instance is pressed the specified eventData; otherwise, <c>false</c>.</returns>
		public new bool IsPressed() {
			return base.IsPressed();
		}

		/// <summary>
		///     Determines whether this option is highlighted.
		/// </summary>
		/// <returns><c>true</c> if this instance is highlighted the specified eventData; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		public bool IsHighlighted(BaseEventData eventData) {
			return base.IsHighlighted();
		}

		/// <summary>
		///     Raises the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnPointerUp(PointerEventData eventData) {
			base.OnPointerUp(eventData);

			if (onPointerUp != null)
				onPointerUp.Invoke(eventData);
		}

		/// <summary>
		///     Does the state transition.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected override void DoStateTransition(SelectionState state, bool instant) {
			if (!enabled || !enabled || !gameObject.activeInHierarchy || selectField == null)
				return;

			Color color = selectField.optionBackgroundTransColors.normalColor;
			Sprite newSprite = null;
			string triggername = selectField.optionBackgroundAnimationTriggers.normalTrigger;

			// Check if this is the disabled state before any others
			if (state == SelectionState.Disabled) {
				color = selectField.optionBackgroundTransColors.disabledColor;
				newSprite = selectField.optionBackgroundSpriteStates.disabledSprite;
				triggername = selectField.optionBackgroundAnimationTriggers.disabledTrigger;
			} else {
				// Prepare the state values
				switch (state) {
					case SelectionState.Normal:
						color = isOn
							? selectField.optionBackgroundTransColors.activeColor
							: selectField.optionBackgroundTransColors.normalColor;
						newSprite = isOn ? selectField.optionBackgroundSpriteStates.activeSprite : null;
						triggername = isOn
							? selectField.optionBackgroundAnimationTriggers.activeTrigger
							: selectField.optionBackgroundAnimationTriggers.normalTrigger;
						break;
					case SelectionState.Highlighted:
						color = isOn
							? selectField.optionBackgroundTransColors.activeHighlightedColor
							: selectField.optionBackgroundTransColors.highlightedColor;
						newSprite = isOn
							? selectField.optionBackgroundSpriteStates.activeHighlightedSprite
							: selectField.optionBackgroundSpriteStates.highlightedSprite;
						triggername = isOn
							? selectField.optionBackgroundAnimationTriggers.activeHighlightedTrigger
							: selectField.optionBackgroundAnimationTriggers.highlightedTrigger;
						break;
					case SelectionState.Pressed:
						color = isOn
							? selectField.optionBackgroundTransColors.activePressedColor
							: selectField.optionBackgroundTransColors.pressedColor;
						newSprite = isOn
							? selectField.optionBackgroundSpriteStates.activePressedSprite
							: selectField.optionBackgroundSpriteStates.pressedSprite;
						triggername = isOn
							? selectField.optionBackgroundAnimationTriggers.activePressedTrigger
							: selectField.optionBackgroundAnimationTriggers.pressedTrigger;
						break;
				}
			}

			// Do the transition
			switch (selectField.optionBackgroundTransitionType) {
				case Transition.ColorTint:
					StartColorTween(color * selectField.optionBackgroundTransColors.colorMultiplier,
						instant ? 0f : selectField.optionBackgroundTransColors.fadeDuration);
					break;
				case Transition.SpriteSwap:
					DoSpriteSwap(newSprite);
					break;
				case Transition.Animation:
					TriggerAnimation(triggername);
					break;
			}

			// Do the transition of the text component
			DoTextStateTransition(state, instant);
		}

		/// <summary>
		///     Does the text state transition.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void DoTextStateTransition(SelectionState state, bool instant) {
			// Make sure we have the select field and text component
			if (selectField != null && textComponent != null)
				// Cross Fade transition
				// Currently the only supported
				if (selectField.optionTextTransitionType == UISelectField.OptionTextTransitionType.CrossFade) {
					Color color = selectField.optionTextTransitionColors.normalColor;

					if (state == SelectionState.Disabled)
						color = selectField.optionTextTransitionColors.disabledColor;
					else
						switch (state) {
							case SelectionState.Normal:
								color = isOn
									? selectField.optionTextTransitionColors.activeColor
									: selectField.optionTextTransitionColors.normalColor;
								break;
							case SelectionState.Highlighted:
								color = isOn
									? selectField.optionTextTransitionColors.activeHighlightedColor
									: selectField.optionTextTransitionColors.highlightedColor;
								break;
							case SelectionState.Pressed:
								color = isOn
									? selectField.optionTextTransitionColors.activePressedColor
									: selectField.optionTextTransitionColors.pressedColor;
								break;
						}

					// Start the tween
					textComponent.CrossFadeColor(color * selectField.optionTextTransitionColors.colorMultiplier,
						instant ? 0f : selectField.optionTextTransitionColors.fadeDuration, true, true);
				}
		}

		/// <summary>
		///     Starts the color tween.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="duration">Duration.</param>
		private void StartColorTween(Color color, float duration) {
			if (targetGraphic == null)
				return;

			targetGraphic.CrossFadeColor(color, duration, true, true);
		}

		/// <summary>
		///     Does the sprite swap.
		/// </summary>
		/// <param name="newSprite">New sprite.</param>
		private void DoSpriteSwap(Sprite newSprite) {
			Image image = targetGraphic as Image;

			if (image == null)
				return;

			image.overrideSprite = newSprite;
		}

		/// <summary>
		///     Triggers the animation.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		private void TriggerAnimation(string trigger) {
			if (selectField == null || animator == null || !animator.enabled || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(trigger))
				return;

			animator.ResetTrigger(selectField.optionBackgroundAnimationTriggers.normalTrigger);
			animator.ResetTrigger(selectField.optionBackgroundAnimationTriggers.pressedTrigger);
			animator.ResetTrigger(selectField.optionBackgroundAnimationTriggers.highlightedTrigger);
			animator.ResetTrigger(selectField.optionBackgroundAnimationTriggers.activeTrigger);
			animator.ResetTrigger(selectField.optionBackgroundAnimationTriggers.activeHighlightedTrigger);
			animator.ResetTrigger(selectField.optionBackgroundAnimationTriggers.activePressedTrigger);
			animator.ResetTrigger(selectField.optionBackgroundAnimationTriggers.disabledTrigger);
			animator.SetTrigger(trigger);
		}

		[Serializable]
		public class SelectOptionEvent : UnityEvent<string> {

		}

		[Serializable]
		public class PointerUpEvent : UnityEvent<BaseEventData> {

		}

	}
}