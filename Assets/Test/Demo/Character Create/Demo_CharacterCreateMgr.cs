using UnityEngine;

namespace AsglaUI.UI {
	public class Demo_CharacterCreateMgr : MonoBehaviour {

		public static Demo_CharacterCreateMgr instance { get; private set; }

		protected void Awake() {
			// Save a reference to the instance
			instance = this;
		}

		protected void OnDestroy() {
			instance = null;
		}

	}
}