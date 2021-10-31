using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Unit_Frame {
	public class UnitFrameSmall : UnitFrameBase {

		[SerializeField] private Button _close;

		[SerializeField] private Text _level;

		public void SetLevel(int level) {
			_level.text = level.ToString();
		}

		//private void Awake() => _close.onClick.AddListener(delegate { gameObject.SetActive(false); });

	}
}