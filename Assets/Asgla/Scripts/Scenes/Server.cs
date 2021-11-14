using Asgla.UI;
using UnityEngine;

namespace Asgla.Scenes {
	public class Server : MonoBehaviour {

		[SerializeField] private ServerButton serverButtonPrefab;

		[SerializeField] private Transform serversGroupTransform;

		#region Unity

		private void Awake() {
			Main.Singleton.Login.Servers.ForEach(AddServer);
		}

		#endregion

		/// <summary>
		///     Adds a server to the servers list.
		/// </summary>
		/// <param name="info">The server info.</param>
		private void AddServer(Data.Web.Server info) {
			if (serverButtonPrefab == null || serversGroupTransform == null)
				return;

			// Add the character
			GameObject serverInstantiate = Instantiate(serverButtonPrefab.gameObject, serversGroupTransform);
			serverInstantiate.layer = serversGroupTransform.gameObject.layer;

			// Get the character component
			ServerButton character = serverInstantiate.GetComponent<ServerButton>();

			if (character == null)
				return;

			// Set the info
			character.SetServerInfo(info);
		}

	}
}