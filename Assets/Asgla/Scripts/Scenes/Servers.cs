using Asgla.Data.Web;
using Asgla.UI;
using UnityEngine;

namespace Asgla.Scenes {
	public class Servers : MonoBehaviour {

		[SerializeField] private ServerItem serverItemPrefab;

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

	}
}