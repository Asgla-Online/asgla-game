using System.Collections.Generic;
using UnityEngine;

namespace AsglaUI.UI {
	public class UIModalBoxManager : ScriptableObject {

#pragma warning disable 0649
		[SerializeField] private GameObject m_ModalBoxPrefab;
#pragma warning restore 0649

		private readonly List<UIModalBox> m_ActiveBoxes = new List<UIModalBox>();

		/// <summary>
		///     Gets the modal box prefab.
		/// </summary>
		public GameObject prefab => m_ModalBoxPrefab;

		/// <summary>
		///     Gets an array of the currently active modal boxes.
		/// </summary>
		public UIModalBox[] activeBoxes {
			get{
				// Do a cleanup
				m_ActiveBoxes.RemoveAll(item => item == null);
				return m_ActiveBoxes.ToArray();
			}
		}

		/// <summary>
		///     Creates a modal box.
		/// </summary>
		/// <param name="rel">Relative game object used to find the canvas.</param>
		public UIModalBox Create(GameObject rel) {
			if (m_ModalBoxPrefab == null || rel == null)
				return null;

			Canvas canvas = UIUtility.FindInParents<Canvas>(rel);

			if (canvas != null) {
				GameObject obj = Instantiate(m_ModalBoxPrefab, canvas.transform, false);

				return obj.GetComponent<UIModalBox>();
			}

			return null;
		}

		/// <summary>
		///     Register a box as active (Used internally).
		/// </summary>
		/// <param name="box"></param>
		public void RegisterActiveBox(UIModalBox box) {
			if (!m_ActiveBoxes.Contains(box))
				m_ActiveBoxes.Add(box);
		}

		/// <summary>
		///     Unregister an active box (Used internally).
		/// </summary>
		/// <param name="box"></param>
		public void UnregisterActiveBox(UIModalBox box) {
			if (m_ActiveBoxes.Contains(box))
				m_ActiveBoxes.Remove(box);
		}

		#region singleton

		private static UIModalBoxManager m_Instance;

		public static UIModalBoxManager Instance {
			get{
				if (m_Instance == null)
					m_Instance = Resources.Load("ModalBoxManager") as UIModalBoxManager;

				return m_Instance;
			}
		}

		#endregion

	}
}