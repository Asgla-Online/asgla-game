using System;
using System.Collections;
using AsglaUI.UI;
using AsglaUI.UI.Tweens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Loading {

	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(GraphicRaycaster))]
	public abstract class LoadingOverlay : MonoBehaviour {

		[NonSerialized] private readonly TweenRunner<FloatTween> _floatTweenRunner;
		protected bool _firstLoad = true;

		protected bool _showing;

		protected LoadingOverlay() {
			Main.Singleton.UIManager.LoadingOverlay = this;

			if (_floatTweenRunner == null)
				_floatTweenRunner = new TweenRunner<FloatTween>();

			_floatTweenRunner.Init(this);
		}

		public void SetDefault() {
			_showing = false;
			_firstLoad = true;
		}

		protected void StartAlphaTween(float targetAlpha, float duration, bool ignoreTimeScale) {
			if (_canvasGroup == null)
				return;

			FloatTween floatTween = new FloatTween
				{duration = duration, startFloat = _canvasGroup.alpha, targetFloat = targetAlpha};
			floatTween.AddOnChangedCallback(SetCanvasAlpha);
			floatTween.AddOnFinishCallback(OnTweenFinished);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			floatTween.easing = _transitionEasing;
			_floatTweenRunner.StartTween(floatTween);
		}

		public void SetCanvasAlpha(float alpha) {
			if (_canvasGroup == null)
				return;

			// Set the alpha
			_canvasGroup.alpha = alpha;
		}

		public void SetLoadingText(string text) {
			_text.text = text;
		}

		protected void OnTweenFinished() {
			if (_showing) {
				_showing = false;
				StartCoroutine(AsynchronousLoad());
			} else {
				//gameObject.SetActive(false);
				//GameObject.Destroy();
				//Main.Singleton.UIManager.LoadingCurrent = null;
				Destroy(gameObject);
			}
		}

		protected abstract IEnumerator AsynchronousLoad();

		protected void UpdateProgress(float progress) {
			float p = Mathf.Clamp01(progress / 0.9f);

			if (_progressBar != null)
				_progressBar.fillAmount = p;
		}

#pragma warning disable 0649
		[SerializeField] protected TextMeshProUGUI _text;
		[SerializeField] protected UIProgressBar _progressBar;
		[SerializeField] protected CanvasGroup _canvasGroup;

		[Header("Transition")] [SerializeField]
		protected TweenEasing _transitionEasing = TweenEasing.InOutQuint;

		[SerializeField] protected float _transitionDuration = 0.4f;
#pragma warning restore 0649

	}
}