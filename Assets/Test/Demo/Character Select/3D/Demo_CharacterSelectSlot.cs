using System;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AsglaUI.UI {
	public class Demo_CharacterSelectSlot : MonoBehaviour {

#pragma warning disable 0649
		[SerializeField] private Light m_Light;
#pragma warning restore 0649

		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_TweenRunner;

		private float m_Intensity;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected Demo_CharacterSelectSlot() {
			if (m_TweenRunner == null)
				m_TweenRunner = new TweenRunner<FloatTween>();

			m_TweenRunner.Init(this);
		}

		public Demo_CharacterInfo info { get; set; }

		public int index { get; set; }

		protected void Awake() {
			if (m_Light != null) {
				m_Light.enabled = false;
				m_Intensity = m_Light.intensity;
				m_Light.intensity = 0;
			}
		}

		private void OnMouseDown() {
			if (info == null)
				return;

			if (EventSystem.current.IsPointerOverGameObject())
				return;

			if (Demo_CharacterSelectMgr.instance != null)
				Demo_CharacterSelectMgr.instance.SelectCharacter(this);
		}

		public void OnSelected() {
			if (m_Light != null) {
				m_Light.enabled = true;
				StartIntensityTween(m_Intensity, 0.3f);
			}
		}

		public void OnDeselected() {
			if (m_Light != null) {
				m_Light.enabled = false;
				m_Light.intensity = 0f;
			}
		}

		private void StartIntensityTween(float target, float duration) {
			if (m_Light == null)
				return;

			if (!Application.isPlaying || duration == 0f) {
				m_Light.intensity = target;
			} else {
				FloatTween colorTween = new FloatTween
					{duration = duration, startFloat = m_Light.intensity, targetFloat = target};
				colorTween.AddOnChangedCallback(SetIntensity);
				colorTween.ignoreTimeScale = true;

				m_TweenRunner.StartTween(colorTween);
			}
		}

		private void SetIntensity(float intensity) {
			if (m_Light == null)
				return;

			m_Light.intensity = intensity;
		}

	}
}