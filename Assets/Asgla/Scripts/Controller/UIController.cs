using System.Linq;
using Asgla.UI.Loading;
using AsglaUI.UI;
using UnityEngine;

namespace Asgla.Controller {
	public class UIController : Controller {

		private UIModalBox _modal;

		public LoadingOverlay LoadingOverlay;

		/// <summary>
		///     Create or return modal box to use everywhere.
		/// </summary>
		public UIModalBox ModalGlobal {
			get{
				if (_modal != null)
					return _modal;

				_modal = UIModalBoxManager.Instance.Create(GameObject.Find("Canvas"))
					.SetTitle(null)
					.SetDescription(null);

				return _modal;
			}
		}

		public static LoadingMapOverlay CreateLoadingMap() {
			return UILoadingOverlayManager.Instance.Create<LoadingMapOverlay>();
		}

		public static LoadingAssetOverlay CreateLoadingAsset() {
			return UILoadingOverlayManager.Instance.Create<LoadingAssetOverlay>();
		}

		public static LoadingSceneOverlay CreateLoadingScene() {
			return UILoadingOverlayManager.Instance.Create<LoadingSceneOverlay>();
		}

		public static void ClearChild(params Transform[] transforms) {
			foreach (Transform child in from Transform transform in transforms
				from Transform child in transform
				select child)
				Object.Destroy(child.gameObject);
		}

		public static Transform FindChildByName(string name, Transform transform) {
			return transform.Cast<Transform>().FirstOrDefault(child => child.name == name);
		}

	}
}