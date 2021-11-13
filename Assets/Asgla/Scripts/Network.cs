using System;
using BestHTTP.WebSocket;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asgla {
	public class Network {

		private bool _disconnected;

		private string _token;

		public Main Main = null;

		public WebSocket Connection { get; private set; }

		public void ConnectToServer(string uri, string token) {
			_token = token;

			_disconnected = false;

			//BestHTTP.HTTPManager.Logger.Level = BestHTTP.Logger.Loglevels.None;

			Connection = new WebSocket(new Uri(uri));

			Connection.OnOpen += OnOpen;
			Connection.OnMessage += OnReceived;
			Connection.OnClosed += OnClosed;
			Connection.OnError += OnError;

#if !UNITY_WEBGL
			Connection.StartPingThread = true;
			Connection.PingFrequency = 10000;
#endif

			Connection.Open();

			Main.UIManager.Modal.SetText1(null);

			Main.UIManager.Modal.SetText2("Connecting...");
		}

		private void OnOpen(WebSocket ws) {
			Main.Request.Send("Login", _token);
		}

		private void OnReceived(WebSocket ws, string message) {
			Main.Request.Get(message.Trim());
		}

		private void OnClosed(WebSocket ws, ushort code, string message) {
			Debug.LogFormat("WebSocket closed! Code: {0} Message: {1}", code, message);
			Disconnect();
		}

		private void OnError(WebSocket ws, string error) {
			Debug.LogFormat("An error occured: <color=red>{0}</color>", error);
			Disconnect();
		}

		private void Disconnect() {
			if (_disconnected)
				return;

			_disconnected = true;

			if (Main.Game != null)
				Main.Game.AvatarController.Player = null;

			if (SceneManager.GetActiveScene().buildIndex == Main.SceneLogin)
				return;

			//UIController.CreateLoadingScene()
			//	.LoadScene(Main.SceneLogin);

			Main.UIManager.Modal.SetText2("Connection closed unexpectedly by the remote server.");
		}

	}
}