using UnityEngine;
using UnityEngine.EventSystems;

namespace AsglaUI.UI {

	[AddComponentMenu("UI/Audio/Play Audio")]
	public class UIPlayAudio : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler,
		IPointerDownHandler, IPointerUpHandler {

		public enum Event {

			None,
			PointerEnter,
			PointerExit,
			PointerDown,
			PointerUp,
			Click,
			DoubleClick

		}

		[SerializeField] private AudioClip m_AudioClip;
		[SerializeField] [Range(0f, 1f)] private float m_Volume = 1f;
		[SerializeField] private Event m_PlayOnEvent = Event.None;

		private bool m_Pressed;

		/// <summary>
		///     Gets or sets the audio clip.
		/// </summary>
		public AudioClip audioClip {
			get => m_AudioClip;
			set => m_AudioClip = value;
		}

		/// <summary>
		///     Gets or sets the volume level.
		/// </summary>
		public float volume {
			get => m_Volume;
			set => m_Volume = value;
		}

		/// <summary>
		///     Gets or sets the event on which the audio clip should be played.
		/// </summary>
		public Event playOnEvent {
			get => m_PlayOnEvent;
			set => m_PlayOnEvent = value;
		}

		public void OnPointerDown(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			TriggerEvent(Event.PointerDown);

			m_Pressed = true;
		}

		public void OnPointerEnter(PointerEventData eventData) {
			if (!m_Pressed)
				TriggerEvent(Event.PointerEnter);
		}

		public void OnPointerExit(PointerEventData eventData) {
			if (!m_Pressed)
				TriggerEvent(Event.PointerExit);
		}

		public void OnPointerUp(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			TriggerEvent(Event.PointerUp);

			if (m_Pressed) {
				if (eventData.clickCount > 1) {
					TriggerEvent(Event.DoubleClick);
					eventData.clickCount = 0;
				} else {
					TriggerEvent(Event.Click);
				}
			}

			m_Pressed = false;
		}

		private void TriggerEvent(Event e) {
			if (e == m_PlayOnEvent)
				PlayAudio();
		}

		public void PlayAudio() {
			if (!enabled || !gameObject.activeInHierarchy)
				return;

			if (m_AudioClip == null)
				return;

			if (UIAudioSource.Instance == null) {
				Debug.LogWarning("You dont have UIAudioSource in your scene. Cannot play audio clip.");
				return;
			}

			// Play the audio clip
			UIAudioSource.Instance.PlayAudio(m_AudioClip, m_Volume);
		}

	}

}