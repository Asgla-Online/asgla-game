using System;
using System.Collections.Generic;
using Asgla.Controller;
using Asgla.Data.Web;
using AsglaUI.UI;
using BestHTTP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Asgla.Scenes {
	public class Login : MonoBehaviour {

		[SerializeField] private TMP_InputField inputUsername;
		[SerializeField] private TMP_InputField inputPassword;

		[SerializeField] private Toggle toggleRemember;

		[SerializeField] private Button button;

		private UIModalBox _modal;

		private HTTPRequest _request;

		private void StartAuthentication() {
			_modal.SetText1("Account")
				.SetText2("Authenticating account..")
				.SetConfirmButtonText("BACK")
				.SetActiveCancelButton(false);

			_modal.onConfirm.AddListener(OnConfirm);

			_modal.Show();

			_request = new HTTPRequest(new Uri(Main.URLLogin), HTTPMethods.Post, OnAuthRequestFinished);

			_request.SetHeader("Accept", "application/json; charset=UTF-8");

			_request.AddField("username", inputUsername.text);
			_request.AddField("password", inputPassword.text);

			_request.Send();
		}

		private void OnAuthRequestFinished(HTTPRequest req, HTTPResponse resp) {
			// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
			switch (req.State) {
				case HTTPRequestStates.Finished:
					Debug.Log(resp.DataAsText);

					//Test
					string[] names =
						{"Annika", "Evita", "Herb", "Thad", "Myesha", "Lucile", "Sharice", "Tatiana", "Isis", "Allen"};

					List<Server> servers = new List<Server>();

					for (int i = 0; i < Random.Range(1, 20); i++) {
						servers.Add(new Server {
							Count = Random.Range(100, 1000),
							Max = 1000,
							Name = names[Random.Range(0, names.Length - 1)],
							Uri = "ws://localhost:4431"
						});
					}

					LoginWebRequest login = new LoginWebRequest {
						Message = "test",
						User = new User {
							Username = "test",
							Token = "test"
						},
						Servers = servers
					};

					//LoginWebRequest login = JsonMapper.ToObject<LoginWebRequest>(resp.DataAsText.Trim());

					if (resp.IsSuccess && login != null && login.User != null && login.User.Token != null) {
						_modal.SetText2("Authentication successfully");
						_modal.SetActiveConfirmButton(false);

						if (toggleRemember.isOn) {
							PlayerPrefs.SetString("loginUsername", inputUsername.text);
							PlayerPrefs.SetString("loginPassword", inputPassword.text);
						}

						PlayerPrefs.Save();

						Main.Singleton.Login = login;

						UIController.CreateLoadingScene()
							.LoadScene(Main.SceneServers);
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

		private void OnConfirm() {
			Debug.Log("[Login] OnConfirm");

			_request.Abort();

			if (Main.Singleton.Network.Connection != null)
				Main.Singleton.Network.Connection.Close();
		}

		public void OnToggleUsername(bool on) {
			if (on) {
				PlayerPrefs.SetString("loginUsername", inputUsername.text);
				PlayerPrefs.SetString("loginPassword", inputPassword.text);
			} else if (PlayerPrefs.HasKey("loginUsername")) {
				PlayerPrefs.DeleteKey("loginUsername");
				PlayerPrefs.DeleteKey("loginPassword");
			}

			PlayerPrefs.Save();
		}

		#region Unity

		private void Awake() {
			_modal = Main.Singleton.UIManager.Modal;

			if (PlayerPrefs.HasKey("loginUsername")) {
				inputUsername.text = PlayerPrefs.GetString("loginUsername");
				inputPassword.text = PlayerPrefs.GetString("loginPassword");
				toggleRemember.isOn = true;
			} else {
				inputUsername.text = "";
				inputPassword.text = "";
				toggleRemember.isOn = false;
			}
		}

		private void Start() {
			// ReSharper disable once RedundantCheckBeforeAssignment
			if (Main.Singleton.Login != null)
				Main.Singleton.Login = null;

			//if (PlayerPrefs.HasKey("loginUsername"))
			//	StartAuthentication();
		}

		private void Update() {
			if (_modal == null && (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return)))
				StartAuthentication();
		}

		protected void OnEnable() {
			button.onClick.AddListener(StartAuthentication);
		}

		protected void OnDisable() {
			button.onClick.RemoveListener(StartAuthentication);
		}

		#endregion

	}
}