using System;
using AsglaUI.UI;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Unit_Frame {
	public class UnitFrameBar : MonoBehaviour {

		[SerializeField] private UIProgressBar _progressBar;

		[SerializeField] private float _duration = 5f;
		[SerializeField] private TweenEasing _easing = TweenEasing.InOutQuint;

		[SerializeField] private Text _text;

		[SerializeField] private float _value = 500;
		[SerializeField] private float _valueMax = 500;

		private readonly string _valueFormat = "#.#";

		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		protected UnitFrameBar() {
			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_FloatTweenRunner.Init(this);
		}

		public void Init() {
			StartTween(1f, _progressBar.fillAmount * _duration);
		}

		public void SetValue(float value) {
			_value = value;

			if (_progressBar is null)
				Debug.Log("_progressBar null1");

			StartTween(_value / _valueMax * 1f, _progressBar.fillAmount * _duration);
		}

		public void SetValueMax(float max) {
			_valueMax = max;

			SetValue(_valueMax);

			if (_progressBar is null)
				return;

			StartTween(1f, _progressBar.fillAmount * _duration);
		}

		protected void SetFillAmount(float amount) {
			_progressBar.fillAmount = amount;

			if (_text != null)
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