using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.UnitFrame {
	public class UnitFrameBig : UnitFrameBase {

		[SerializeField] private Button _close;

		[SerializeField] private Text _username;

		[SerializeField] private Text _level;

		[SerializeField] private GameObject _skull;

		public Button Close => _close;

		public void SetUsername(string username) {
			_username.text = username;
		}

		public void SetLevel(int level) {
			_level.text = level.ToString();
		}

	}
}