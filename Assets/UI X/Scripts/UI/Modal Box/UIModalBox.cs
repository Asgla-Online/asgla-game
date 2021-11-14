using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(UIWindow))]
	[RequireComponent(typeof(UIAlwaysOnTop))]
	public class UIModalBox : MonoBehaviour {

		[Header("Events")] public UnityEvent onConfirm = new UnityEvent();

		public UnityEvent onCancel = new UnityEvent();

		[Header("Texts")] [SerializeField] private TextMeshProUGUI title;

		[SerializeField] private TextMeshProUGUI description;

		[Header("Buttons")] [SerializeField] private Button confirm;

		[SerializeField] private TextMeshProUGUI confirmText;

		[SerializeField] private Button cancel;

		private UIWindow _window;

		/// <summary>
		///     Gets a value indicating whether this modal box is active.
		/// </summary>
		public bool IsActive { get; private set; }

		protected void Awake() {
			// Make sure we have the window component
			if (_window == null)
				_window = gameObject.GetComponent<UIWindow>();

			// Prepare some window parameters
			_window.ID = UIWindowID.ModalBox;
			_window.escapeKeyAction = UIWindow.EscapeKeyAction.None;

			// Hook an event to the window
			_window.onTransitionComplete.AddListener(OnWindowTransitionEnd);

			// Prepare the always on top component
			UIAlwaysOnTop aot = gameObject.GetComponent<UIAlwaysOnTop>();
			aot.order = UIAlwaysOnTop.ModalBoxOrder;

			// Hook the button click event
			if (confirm != null)
				confirm.onClick.AddListener(Confirm);

			if (cancel != null)
				cancel.onClick.AddListener(Close);
		}

		protected void Update() {
			if (!string.IsNullOrEmpty("Submit") && Input.GetButtonDown("Submit"))
				Close();

			if (!string.IsNullOrEmpty("Cancel") && Input.GetButtonDown("Cancel"))
				Confirm();
		}

		/// <summary>
		///     Sets the text on the first line.
		/// </summary>
		/// <param name="text"></param>
		public UIModalBox SetTitle(string text) {
			if (title != null) {
				title.text = text;
				title.gameObject.SetActive(!string.IsNullOrEmpty(text));
			}

			return this;
		}

		/// <summary>
		///     Sets the text on the second line.
		/// </summary>
		/// <param name="text"></param>
		public UIModalBox SetDescription(string text) {
			if (description == null)
				return this;

			description.text = text;
			description.gameObject.SetActive(!string.IsNullOrEmpty(text));

			return this;
		}

		/// <summary>
		///     Sets the confirm button text.
		/// </summary>
		/// <param name="text">The confirm button text.</param>
		public UIModalBox SetConfirmText(string text) {
			if (confirmText != null)
				confirmText.text = text;

			return this;
		}

		/// <summary>
		///     Set cancel button active.
		/// </summary>
		/// <param name="value">The cancel button SetActive value.</param>
		public UIModalBox SetActiveCancelButton(bool value) {
			cancel.gameObject.SetActive(value);

			return this;
		}

		/// <summary>
		///     Set confirm button active.
		/// </summary>
		/// <param name="value">The confirm button SetActive value.</param>
		public UIModalBox SetActiveConfirmButton(bool value) {
			confirm.gameObject.SetActive(value);

			return this;
		}

		/// <summary>
		///     Shows the modal box.
		/// </summary>
		public void Show() {
			IsActive = true;

			if (UIModalBoxManager.Instance != null)
				UIModalBoxManager.Instance.RegisterActiveBox(this);

			// Show the modal
			if (_window != null)
				_window.Show();
		}

		/// <summary>
		///     Closes the modal box.
		/// </summary>
		public void Close() {
			Debug.Log(1);
			Hide();

			// Invoke the cancel event
			onCancel?.Invoke();
		}

		private void Confirm() {
			Debug.Log(2);
			Hide();

			// Invoke the confirm event
			onConfirm?.Invoke();
		}

		public void Hide() {
			IsActive = false;

			if (UIModalBoxManager.Instance != null)
				UIModalBoxManager.Instance.UnregisterActiveBox(this);

			// Hide the modal
			if (_window != null)
				_window.Hide();
			Debug.Log(3);
		}

		private void OnWindowTransitionEnd(UIWindow window, UIWindow.VisualState state) {
			// Destroy the modal box when hidden
			//if (state == UIWindow.VisualState.Hidden)
			//	Destroy(gameObject);
		}

	}
}