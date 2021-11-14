using System.Collections.Generic;
using UnityEngine;

namespace AsglaUI.UI {
	public class UIModalBoxManager : ScriptableObject {

		[SerializeField] private GameObject modalBoxPrefab;

		private readonly List<UIModalBox> _activeBoxes = new List<UIModalBox>();

		/// <summary>
		///     Gets an array of the currently active modal boxes.
		/// </summary>
		public UIModalBox[] ActiveBoxes {
			get{
				// Do a cleanup
				_activeBoxes.RemoveAll(item => item == null);
				return _activeBoxes.ToArray();
			}
		}

		/// <summary>
		///     Creates a modal box.
		/// </summary>
		/// <param name="rel">Relative game object used to find the canvas.</param>
		public UIModalBox Create(GameObject rel) {
			if (modalBoxPrefab == null || rel == null)
				return null;

			Canvas canvas = UIUtility.FindInParents<Canvas>(rel);

			if (canvas == null)
				return null;

			GameObject obj = Instantiate(modalBoxPrefab, canvas.transform, false);

			return obj.GetComponent<UIModalBox>();
		}

		/// <summary>
		///     Register a box as active (Used internally).
		/// </summary>
		/// <param name="box"></param>
		public void RegisterActiveBox(UIModalBox box) {
			if (!_activeBoxes.Contains(box))
				_activeBoxes.Add(box);
		}

		/// <summary>
		///     Unregister an active box (Used internally).
		/// </summary>
		/// <param name="box"></param>
		public void UnregisterActiveBox(UIModalBox box) {
			if (_activeBoxes.Contains(box))
				_activeBoxes.Remove(box);
		}

		#region singleton

		private static UIModalBoxManager _instance;

		public static UIModalBoxManager Instance {
			get{
				if (_instance == null)
					_instance = Resources.Load("ModalBoxManager") as UIModalBoxManager;

				return _instance;
			}
		}

		#endregion

	}
}