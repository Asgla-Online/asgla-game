using Asgla.Controller;
using Asgla.Data.Web;
using Asgla.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.Scenes {
	public class Servers : MonoBehaviour {

		[SerializeField] private Button close;

		[SerializeField] private ServerItem serverItemPrefab;

		[SerializeField] private Transform serversGroupTransform;

		/// <summary>
		///     Adds a server to the servers list.
		/// </summary>
		/// <param name="info">The server info.</param>
		private void AddServer(Server info) {
			if (serverItemPrefab == null || serversGroupTransform == null)
				return;

			// Add the character
			GameObject serverInstantiate = Instantiate(serverItemPrefab.gameObject, serversGroupTransform);
			serverInstantiate.layer = serversGroupTransform.gameObject.layer;

			// Get the character component
			ServerItem character = serverInstantiate.GetComponent<ServerItem>();

			if (character == null)
				return;

			// Set the info
			character.SetServerInfo(info);
		}

		private static void OnClose() {
			UIController.CreateLoadingScene()
				.LoadScene(Main.SceneLogin);
		}

		#region Unity

		private void Awake() {
			Main.Singleton.Login.Servers.ForEach(AddServer);
		}

		protected void OnEnable() {
			close.onClick.AddListener(OnClose);
		}

		protected void OnDisable() {
			close.onClick.RemoveListener(OnClose);
		}

		#endregion

	}
}