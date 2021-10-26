using System;
using AsglaUI.UI;
using BestHTTP;
using BestHTTP.JSON.LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Asgla.Data.Web.WebSetting;

namespace Asgla {
	public class Login : MonoBehaviour {

		[SerializeField] private TMP_InputField _inputUsername;
		[SerializeField] private TMP_InputField _inputPassword;

		[SerializeField] private Toggle _toggleRemember;

		[SerializeField] private Button _button;

		private UIModalBox _modal;

		private HTTPRequest _request;

		public void StartAuthentication() {
			Main.Singleton.UIManager.CreateModal(gameObject);

			_modal = Main.Singleton.UIManager.Modal;

			_modal.SetText1("Account");
			_modal.SetText2("Authenticating account..");
			_modal.SetConfirmButtonText("BACK");
			_modal.SetActiveCancelButton(false);

			_modal.onConfirm.AddListener(OnConfirm);

			_modal.Show();

			_request = new HTTPRequest(new Uri(Main.Singleton.url_login), HTTPMethods.Post, OnAuthRequestFinished);

			_request.SetHeader("Accept", "application/json; charset=UTF-8");

			_request.AddField("username", _inputUsername.text);
			_request.AddField("password", _inputPassword.text);

			_request.Send();
		}

		private void OnAuthRequestFinished(HTTPRequest req, HTTPResponse resp) {
			switch (req.State) {
				case HTTPRequestStates.Finished:
					Debug.Log(resp.DataAsText);

					LoginWebRequest login = JsonMapper.ToObject<LoginWebRequest>(resp.DataAsText.Trim());

					if (resp.IsSuccess && login != null && login.user != null && login.user.Token != null) {
						_modal.SetText2("Authentication successfully");
						_modal.SetActiveConfirmButton(false);

						if (_toggleRemember.isOn) {
							PlayerPrefs.SetString("loginUsername", _inputUsername.text);
							PlayerPrefs.SetString("loginPassword", _inputPassword.text);
						}

						PlayerPrefs.Save();

						Main.Singleton.Login(_modal, login.user.Token);
					} else {
						_modal.SetText1("Warning");
						_modal.SetText2(login?.Message ?? "Error");
					}

					break;
				case HTTPRequestStates.Error:
					_modal.SetText1("Error");
					_modal.SetText2("Request Finished with Error!");
					break;
				case HTTPRequestStates.Aborted:
					_modal.SetText1("Error");
					_modal.SetText2("Request Aborted!");
					break;
				case HTTPRequestStates.ConnectionTimedOut:
				case HTTPRequestStates.TimedOut:
					_modal.SetText1("Error");
					_modal.SetText2("Timed Out!");
					break;
			}
		}

		public void OnConfirm() {
			Debug.Log("[Login] OnConfirm");

			_request.Abort();

			if (Main.Singleton.Network.Connection != null)
				Main.Singleton.Network.Connection.Close();
		}

		public void OnToggleUsername(bool on) {
			if (on) {
				PlayerPrefs.SetString("loginUsername", _inputUsername.text);
				PlayerPrefs.SetString("loginPassword", _inputPassword.text);
			} else if (PlayerPrefs.HasKey("loginUsername")) {
				PlayerPrefs.DeleteKey("loginUsername");
				PlayerPrefs.DeleteKey("loginPassword");
			}

			PlayerPrefs.Save();
		}

		#region Unity

		private void Awake() {
			if (PlayerPrefs.HasKey("loginUsername")) {
				_inputUsername.text = PlayerPrefs.GetString("loginUsername");
				_inputPassword.text = PlayerPrefs.GetString("loginPassword");
				_toggleRemember.isOn = true;
			} else {
				_inputUsername.text = "";
				_inputPassword.text = "";
				_toggleRemember.isOn = false;
			}
		}

		private void Start() {
			if (PlayerPrefs.HasKey("loginUsername"))
				StartAuthentication();
		}

		private void Update() {
			if (_modal == null && (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return)))
				StartAuthentication();
		}

		protected void OnEnable() {
			_button.onClick.AddListener(StartAuthentication);
		}

		protected void OnDisable() {
			_button.onClick.RemoveListener(StartAuthentication);
		}

		#endregion

	}
}