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

		private UIWindow m_Window;

		/// <summary>
		///     Gets a value indicating whether this modal box is active.
		/// </summary>
		public bool isActive { get; private set; }

		protected void Awake() {
			// Make sure we have the window component
			if (m_Window == null)
				m_Window = gameObject.GetComponent<UIWindow>();

			// Prepare some window parameters
			m_Window.ID = UIWindowID.ModalBox;
			m_Window.escapeKeyAction = UIWindow.EscapeKeyAction.None;

			// Hook an event to the window
			m_Window.onTransitionComplete.AddListener(OnWindowTransitionEnd);

			// Prepare the always on top component
			UIAlwaysOnTop aot = gameObject.GetComponent<UIAlwaysOnTop>();
			aot.order = UIAlwaysOnTop.ModalBoxOrder;

			// Hook the button click event
			if (m_ConfirmButton != null)
				m_ConfirmButton.onClick.AddListener(Confirm);

			if (m_CancelButton != null)
				m_CancelButton.onClick.AddListener(Close);
		}

		protected void Update() {
			if (!string.IsNullOrEmpty(m_CancelInput) && Input.GetButtonDown(m_CancelInput))
				Close();

			if (!string.IsNullOrEmpty(m_ConfirmInput) && Input.GetButtonDown(m_ConfirmInput))
				Confirm();
		}

		/// <summary>
		///     Sets the text on the first line.
		/// </summary>
		/// <param name="text"></param>
		public void SetText1(string text) {
			if (m_Text1 != null) {
				m_Text1.text = text;
				m_Text1.gameObject.SetActive(!string.IsNullOrEmpty(text));
			}
		}

		/// <summary>
		///     Sets the text on the second line.
		/// </summary>
		/// <param name="text"></param>
		public void SetText2(string text) {
			if (m_Text2 != null) {
				m_Text2.text = text;
				m_Text2.gameObject.SetActive(!string.IsNullOrEmpty(text));
			}
		}

		/// <summary>
		///     Sets the confirm button text.
		/// </summary>
		/// <param name="text">The confirm button text.</param>
		public void SetConfirmButtonText(string text) {
			if (m_ConfirmButtonText != null)
				m_ConfirmButtonText.text = text;
		}

		/// <summary>
		/// Sets the cancel button text.
		/// </summary>
		/// <param name="text">The cancel button text.</param>
		//public void SetCancelButtonText(string text) {
		//    if (this.m_CancelButtonText != null) {
		//        this.m_CancelButtonText.text = text;
		//    }
		//}

		/// <summary>
		///     Set cancel button active.
		/// </summary>
		/// <param name="value">The cancel button SetActive value.</param>
		public void SetActiveCancelButton(bool value) {
			if (m_CancelButton != null)
				m_CancelButton.gameObject.SetActive(value);
		}

		/// <summary>
		///     Set confirm button active.
		/// </summary>
		/// <param name="value">The confirm button SetActive value.</param>
		public void SetActiveConfirmButton(bool value) {
			if (m_ConfirmButton != null)
				m_ConfirmButton.gameObject.SetActive(value);
		}

		/// <summary>
		///     Shows the modal box.
		/// </summary>
		public void Show() {
			isActive = true;

			if (UIModalBoxManager.Instance != null)
				UIModalBoxManager.Instance.RegisterActiveBox(this);

			// Show the modal
			if (m_Window != null)
				m_Window.Show();
		}

		/// <summary>
		///     Closes the modal box.
		/// </summary>
		public void Close() {
			_Hide();

			// Invoke the cancel event
			if (onCancel != null)
				onCancel.Invoke();
		}

		public void Confirm() {
			_Hide();

			// Invoke the confirm event
			if (onConfirm != null)
				onConfirm.Invoke();
		}

		private void _Hide() {
			isActive = false;

			if (UIModalBoxManager.Instance != null)
				UIModalBoxManager.Instance.UnregisterActiveBox(this);

			// Hide the modal
			if (m_Window != null)
				m_Window.Hide();
		}

		public void OnWindowTransitionEnd(UIWindow window, UIWindow.VisualState state) {
			// Destroy the modal box when hidden
			if (state == UIWindow.VisualState.Hidden)
				Destroy(gameObject);
		}

#pragma warning disable 0649
		[Header("Texts")] [SerializeField] private TextMeshProUGUI m_Text1;

		[SerializeField] private TextMeshProUGUI m_Text2;

		[Header("Buttons")] [SerializeField] private Button m_ConfirmButton;

		[SerializeField] private TextMeshProUGUI m_ConfirmButtonText;

		[SerializeField] private Button m_CancelButton;
		//[SerializeField] private TextMeshProUGUI m_CancelButtonText;

		[Header("Inputs")] [SerializeField] private string m_ConfirmInput = "Submit";

		[SerializeField] private string m_CancelInput = "Cancel";
#pragma warning restore 0649

	}
}