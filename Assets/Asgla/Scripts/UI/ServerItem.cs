using Asgla.Data.Web;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI {
	public class ServerItem : MonoBehaviour {

		[SerializeField] private Button button;

		[Header("Texts")] [SerializeField] private TextMeshProUGUI serverName;

		[SerializeField] private TextMeshProUGUI serverCount;
		[SerializeField] private Image icon;

		private Server _server;

		protected void OnEnable() {
			button.onClick.AddListener(OnSelect);
		}

		protected void OnDisable() {
			button.onClick.RemoveListener(OnSelect);
		}

		public void SetServerInfo(Server info) {
			_server = info;

			serverName.text = _server.Name;

			serverCount.text = _server.Count + "/" + _server.Max;
		}

		public void SetIcon(Sprite sprite) {
			icon.sprite = sprite;
		}

		private void OnSelect() {
			Main.Singleton.Network.ConnectToServer(_server.Uri, Main.Singleton.Login.User.Token);
		}

	}
}