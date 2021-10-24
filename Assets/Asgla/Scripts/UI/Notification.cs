using System.Collections;
using AsglaUI.UI;
using TMPro;
using UnityEngine;

namespace Asgla.UI {
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class Notification : UIWindow {

		[SerializeField] private TextMeshProUGUI _title;

		[SerializeField] private TextMeshProUGUI _description;

		private Coroutine _coroutine;

		private bool _running;

		public void Init(string title, string description) {
			if (title == null) {
				_title.gameObject.SetActive(false);
			} else {
				_title.gameObject.SetActive(true);
				_title.text = title;
			}

			_description.text = description;

			Show();

			if (_running)
				StopCoroutine(_coroutine);

			_coroutine = StartCoroutine(DoSomething());
		}

		private IEnumerator DoSomething() {
			_running = true;

			//Wait
			yield return new WaitForSecondsRealtime(3);

			Hide();
			_running = false;
		}

	}
}