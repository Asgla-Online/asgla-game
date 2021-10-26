using System.Linq;
using Asgla.UI.Loading;
using AsglaUI.UI;
using UnityEngine;

namespace Asgla.Controller {
	public class UIController : Controller {

		public LoadingOverlay LoadingOverlay;

		public UIModalBox Modal;

		public UIModalBox CreateModal(GameObject rel) {
			if (Modal != null)
				return Modal;

			Modal = UIModalBoxManager.Instance.Create(rel);

			return Modal;
		}

		public static LoadingMapOverlay CreateLoadingMap() {
			GameObject obj = Object.Instantiate(Main.Singleton.Loading);
			return obj.AddComponent<LoadingMapOverlay>();
		}

		public static LoadingAssetOverlay CreateLoadingAsset() {
			GameObject obj = Object.Instantiate(Main.Singleton.Loading);
			return obj.AddComponent<LoadingAssetOverlay>();
		}

		public static LoadingSceneOverlay CreateLoadingScene() {
			GameObject obj = Object.Instantiate(Main.Singleton.Loading);
			return obj.AddComponent<LoadingSceneOverlay>();
		}

		public static void ClearChild(params Transform[] transforms) {
			foreach (Transform child in from Transform transform in transforms
				from Transform child in transform
				select child)
				Object.Destroy(child.gameObject);
		}

	}
}