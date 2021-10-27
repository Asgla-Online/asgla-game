using UnityEngine;

namespace AsglaUI.UI {
	public class UILoadingOverlayManager : ScriptableObject {

#pragma warning disable 0649
		[SerializeField] private GameObject m_LoadingOverlayPrefab;
#pragma warning restore 0649

		/// <summary>
		///     Gets the loading overlay prefab.
		/// </summary>
		public GameObject prefab => m_LoadingOverlayPrefab;

		/// <summary>
		///     Creates a loading overlay.
		/// </summary>
		/// <returns>The loading overlay component.</returns>
		public UILoadingOverlay Create() {
			if (m_LoadingOverlayPrefab == null)
				return null;

			GameObject obj = Instantiate(m_LoadingOverlayPrefab);

			///obj.GetComponent<LoadingMapOverlay>().enabled = false;

			return obj.GetComponent<UILoadingOverlay>();
		}

		#region singleton

		private static UILoadingOverlayManager m_Instance;

		public static UILoadingOverlayManager Instance {
			get{
				if (m_Instance == null)
					m_Instance = Resources.Load("LoadingOverlayManager") as UILoadingOverlayManager;

				return m_Instance;
			}
		}

		#endregion

	}
}