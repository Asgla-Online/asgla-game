using UnityEngine;

namespace AsglaUI.UI {
	public class UILoadingOverlayManager : ScriptableObject {

		[SerializeField] private GameObject loadingOverlayPrefab;

		/// <summary>
		///     Creates a loading overlay.
		/// </summary>
		/// <returns>The loading overlay.</returns>
		public T Create<T>() where T : Component {
			if (loadingOverlayPrefab == null)
				return null;

			GameObject obj = Instantiate(loadingOverlayPrefab);

			obj.name = typeof(T).Name;

			return obj.AddComponent<T>();
		}

		#region singleton

		private static UILoadingOverlayManager _instance;

		public static UILoadingOverlayManager Instance {
			get{
				if (_instance == null)
					_instance = Resources.Load("LoadingOverlayManager") as UILoadingOverlayManager;

				return _instance;
			}
		}

		#endregion

	}
}