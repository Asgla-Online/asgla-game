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

		private HTTPRequest _request;

		private void StartAuthentication() {
			Main.Singleton.UIManager.ModalGlobal
				.SetTitle("Account")
				.SetDescription("Authenticating account..")
				.SetActiveCancelButton(false);

			Main.Singleton.UIManager.ModalGlobal.onConfirm.AddListener(OnConfirm);

			Main.Singleton.UIManager.ModalGlobal.Show();

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

					//TODO: Replace with json

					List<Data.Web.Server> servers = new List<Data.Web.Server> {
						new Data.Web.Server {
							//Normal
							Count = Random.Range(100, 1000),
							Max = 1000,
							Name = "Annika",
							Uri = "ws://localhost:4431",
							IsOnline = true
						},
						new Data.Web.Server {
							//Full
							Count = Random.Range(500, 2000),
							Max = 1000,
							Name = "Evita",
							Uri = "ws://localhost:4431",
							IsOnline = true
						},
						new Data.Web.Server {
							//Offline
							Count = Random.Range(100, 1000),
							Max = 1000,
							Name = "Herb",
							Uri = "ws://localhost:4431",
							IsOnline = false
						},
						new Data.Web.Server {
							//Normal
							Count = Random.Range(100, 1000),
							Max = 1000,
							Name = "Tatiana",
							Uri = "ws://localhost:4431",
							IsOnline = true
						},
						new Data.Web.Server {
							//VIP
							Count = Random.Range(100, 1000),
							Max = 1000,
							Name = "Tatiana",
							Uri = "ws://localhost:4431",
							IsOnline = false
						},
						new Data.Web.Server {
							//VIP
							Count = Random.Range(100, 1000),
							Max = 1000,
							Name = "Tatiana 2",
							Uri = "ws://localhost:4431",
							IsOnline = false
						},
						new Data.Web.Server {
							//VIP
							Count = Random.Range(100, 1000),
							Max = 1000,
							Name = "Tatiana 3",
							Uri = "ws://localhost:4431",
							IsOnline = false
						},
						new Data.Web.Server {
							//VIP
							Count = Random.Range(100, 1000),
							Max = 1000,
							Name = "Tatiana 4",
							Uri = "ws://localhost:4431",
							IsOnline = false
						},
						new Data.Web.Server {
							//VIP
							Count = Random.Range(100, 1000),
							Max = 1000,
							Name = "Tatiana 5",
							Uri = "ws://localhost:4431",
							IsOnline = false
						}
					};
					LoginWebRequest login = new LoginWebRequest {
						Message = "test",
						User = new User {
							Username = "test",
							Token = "test"
						},
						Servers = servers
					};

					//LoginWebRequest login = JsonMapper.ToObject<LoginWebRequest>(resp.DataAsText.Trim());

					//TODO: END

					if (resp.IsSuccess && login != null && login.User != null && login.User.Token != null) {
						Main.Singleton.UIManager.ModalGlobal
							.SetDescription("Authentication successfully")
							.SetActiveConfirmButton(false);

						if (toggleRemember.isOn) {
							PlayerPrefs.SetString("loginUsername", inputUsername.text);
							PlayerPrefs.SetString("loginPassword", inputPassword.text);
						}

						PlayerPrefs.Save();

						Main.Singleton.Login = login;

						UIController.CreateLoadingScene()
							.LoadScene(Main.SceneServers);
					} else {
						Main.Singleton.UIManager.ModalGlobal.SetTitle("Warning")
							.SetDescription(login?.Message ?? "Error");
					}

					break;
				case HTTPRequestStates.Error:
					Main.Singleton.UIManager.ModalGlobal
						.SetTitle("Error")
						.SetDescription("Request Finished with Error!");
					break;
				case HTTPRequestStates.Aborted:
					Main.Singleton.UIManager.ModalGlobal
						.SetTitle("Error")
						.SetDescription("Request Aborted!");
					break;
				case HTTPRequestStates.ConnectionTimedOut:
				case HTTPRequestStates.TimedOut:
					Main.Singleton.UIManager.ModalGlobal
						.SetTitle("Error")
						.SetDescription("Timed Out!");
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

			if (Main.Singleton.Game != null)
				Main.Singleton.Game.AvatarController.Player = null;

			//if (PlayerPrefs.HasKey("loginUsername"))
			//	StartAuthentication();
		}

		private void Update() {
			if (UIModalBoxManager.Instance.ActiveBoxes.Length == 0 &&
			    (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return)))
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