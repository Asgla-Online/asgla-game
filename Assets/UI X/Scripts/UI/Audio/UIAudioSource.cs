using UnityEngine;

namespace AsglaUI.UI {

	[AddComponentMenu("UI/Audio/Audio Source")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AudioSource))]
	public class UIAudioSource : MonoBehaviour {

		[SerializeField] [Range(0f, 1f)] private float m_Volume = 1f;

		private AudioSource m_AudioSource;

		/// <summary>
		///     Gets or sets the volume level.
		/// </summary>
		public float volume {
			get => m_Volume;
			set => m_Volume = value;
		}

		#region Singleton

		public static UIAudioSource Instance { get; private set; }

		#endregion

		protected void Awake() {
			if (Instance != null) {
				Debug.LogWarning(
					"You have more than one UIAudioSource in the scene, please make sure you have only one.");
				return;
			}

			Instance = this;

			// Get the audio source
			m_AudioSource = gameObject.GetComponent<AudioSource>();
			m_AudioSource.playOnAwake = false;
		}

		public void PlayAudio(AudioClip clip) {
			m_AudioSource.PlayOneShot(clip, m_Volume);
		}

		public void PlayAudio(AudioClip clip, float volume) {
			m_AudioSource.PlayOneShot(clip, m_Volume * volume);
		}

	}

}