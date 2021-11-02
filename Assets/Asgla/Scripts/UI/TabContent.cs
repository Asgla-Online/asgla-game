using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI {
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/Tab content", 58)]
	public class TabContent : MonoBehaviour {

		public GameObject content;
		public ScrollRect scrollRect;

	}
}