using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class UISelectField_List : Selectable {

		public enum State {

			Opened,
			Closed

		}

		public AnimationFinishEvent onAnimationFinish = new AnimationFinishEvent();
		public UnityEvent onDimensionsChange = new UnityEvent();
		private string m_AnimationCloseTrigger = string.Empty;

		private string m_AnimationOpenTrigger = string.Empty;
		private State m_State = State.Closed;

		protected override void Start() {
			base.Start();
			transition = Transition.None;

			Navigation nav = new Navigation();
			nav.mode = Navigation.Mode.None;
			navigation = nav;
		}

		protected void Update() {
			if (animator != null && !string.IsNullOrEmpty(m_AnimationOpenTrigger) &&
			    !string.IsNullOrEmpty(m_AnimationCloseTrigger)) {
				AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

				// Check which is the current state
				if (state.IsName(m_AnimationOpenTrigger) && m_State == State.Closed) {
					if (state.normalizedTime >= state.length) {
						// Flag as opened
						m_State = State.Opened;

						// Invoke the animation finish event
						if (onAnimationFinish != null)
							onAnimationFinish.Invoke(m_State);
					}
				} else if (state.IsName(m_AnimationCloseTrigger) && m_State == State.Opened) {
					if (state.normalizedTime >= state.length) {
						// Flag as closed
						m_State = State.Closed;

						// Invoke the animation finish event
						if (onAnimationFinish != null)
							onAnimationFinish.Invoke(m_State);
					}
				}
			}
		}

		protected override void OnRectTransformDimensionsChange() {
			base.OnRectTransformDimensionsChange();

			if (onDimensionsChange != null)
				onDimensionsChange.Invoke();
		}

		/// <summary>
		///     Sets the animation triggers (Used to detect animation finish).
		/// </summary>
		/// <param name="openTrigger">Open trigger.</param>
		/// <param name="closeTrigger">Close trigger.</param>
		public void SetTriggers(string openTrigger, string closeTrigger) {
			m_AnimationOpenTrigger = openTrigger;
			m_AnimationCloseTrigger = closeTrigger;
		}

		/// <summary>
		///     Determines whether list is pressed.
		/// </summary>
		/// <returns><c>true</c> if the list is pressed by the specified eventData; otherwise, <c>false</c>.</returns>
		public new bool IsPressed() {
			return base.IsPressed();
		}

		/// <summary>
		///     Determines whether list is highlighted.
		/// </summary>
		/// <returns><c>true</c> if this instance is highlighted the specified eventData; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		public bool IsHighlighted(BaseEventData eventData) {
			return base.IsHighlighted();
		}

		[Serializable]
		public class AnimationFinishEvent : UnityEvent<State> {

		}

	}
}