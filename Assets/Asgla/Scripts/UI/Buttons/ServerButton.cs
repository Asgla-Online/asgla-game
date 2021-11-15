using Asgla.Data.Web;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Buttons {
	public class ServerButton : MonoBehaviour {

		[SerializeField] private TextMeshProUGUI serverName;

		[SerializeField] private TextMeshProUGUI serverCount;
		[SerializeField] private TextMeshProUGUI serverStatus;

		private Button _button;

		private Server _server;

		private void Awake() {
			_button = GetComponent<Button>();
		}

		private void OnEnable() {
			_button.onClick.AddListener(OnSelect);
		}

		private void OnDisable() {
			_button.onClick.RemoveListener(OnSelect);
		}

		public void SetServerInfo(Server info) {
			_server = info;

			serverName.text = _server.Name;
			serverCount.text = _server.Count + "/" + _server.Max;
			serverStatus.text = info.IsOnline ? info.Count >= info.Max ? "Full" : "" : "Offline";

			_button.interactable = info.IsOnline && info.Count < info.Max;
		}

		private void OnSelect() {
			Main.Singleton.Network.ConnectToServer(_server.Uri, Main.Singleton.Login.User.Token);
		}

	}
}