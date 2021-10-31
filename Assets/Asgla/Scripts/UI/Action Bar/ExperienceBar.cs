using System;
using AsglaUI.UI;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Action_Bar {
	public class ExperienceBar : MonoBehaviour {

		[SerializeField] private UIProgressBar _progressBar;

		[SerializeField] private float _duration = 5f;
		[SerializeField] private TweenEasing _easing = TweenEasing.InOutQuint;

		[SerializeField] private Text _text;

		[SerializeField] private float _value = 500;
		[SerializeField] private float _valueMax = 500;

		private readonly string _valueFormat = "#.#";

		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected ExperienceBar() {
			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_FloatTweenRunner.Init(this);
		}

		public void Init() {
			StartTween(1f, _progressBar.fillAmount * _duration);
		}

		public void SetValue(float value) {
			_value = value;

			float percentage = _value / _valueMax * 1f;

			StartTween(percentage, _progressBar.fillAmount * _duration);
		}

		public void SetValueMax(float max) {
			_valueMax = max;
			StartTween(1f, _progressBar.fillAmount * _duration);
		}

		protected void SetFillAmount(float amount) {
			_progressBar.fillAmount = amount;

			_text.text = $"{(_valueMax * amount).ToString(_valueFormat)}/{_valueMax.ToString(_valueFormat)}";
		}

		protected void StartTween(float targetFloat, float duration) {
			if (_progressBar == null)
				return;

			FloatTween floatTween = new FloatTween
				{duration = duration, startFloat = _progressBar.fillAmount, targetFloat = targetFloat};
			floatTween.AddOnChangedCallback(SetFillAmount);
			floatTween.ignoreTimeScale = true;
			floatTween.easing = _easing;

			m_FloatTweenRunner.StartTween(floatTween);
		}

	}
}