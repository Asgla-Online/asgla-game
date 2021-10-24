using AsglaUI.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI {
	public class Menu : MonoBehaviour {

		[SerializeField] private GameObject[] _contents;

		[SerializeField] private Button _button;

		[SerializeField] private UIFlippable _arrowFlippable;

		[SerializeField] private Vector2 _activeOffset = Vector2.zero;

		[SerializeField] private Vector2 _inactiveOffset = Vector2.zero;

		private bool _on = true;

		private void Awake() {
			_on = !(PlayerPrefs.GetInt("MenuToggle") == 0);
			if (!_on)
				Hide();
		}

		public void Toggle() {
			if (_on)
				Hide();
			else
				Show();

			PlayerPrefs.SetInt("MenuToggle", _on ? 1 : 0);
			PlayerPrefs.Save();
		}

		private void Show() {
			_on = true;

			if (_contents != null)
				foreach (GameObject button in _contents)
					button.SetActive(true);

			if (_arrowFlippable != null) {
				_arrowFlippable.horizontal = false;
				(_arrowFlippable.transform as RectTransform).anchoredPosition = _activeOffset;
			}
		}

		private void Hide() {
			_on = false;

			if (_contents != null)
				foreach (GameObject button in _contents)
					button.SetActive(false);

			if (_arrowFlippable != null) {
				_arrowFlippable.horizontal = true;
				(_arrowFlippable.transform as RectTransform).anchoredPosition = _inactiveOffset;
			}
		}

	}
}