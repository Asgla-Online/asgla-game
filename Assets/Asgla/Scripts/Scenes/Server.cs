using Asgla.Controller;
using Asgla.UI.Buttons;
using UnityEngine;

namespace Asgla.Scenes {
	public class Server : MonoBehaviour {

		[SerializeField] private ButtonServer buttonServerPrefab;

		[SerializeField] private Transform serversGroupTransform;

		/// <summary>
		///     Adds a server to the servers list.
		/// </summary>
		/// <param name="info">The server info.</param>
		private void AddServer(Data.Web.Server info) {
			if (buttonServerPrefab == null || serversGroupTransform == null)
				return;

			// Add the character
			GameObject serverInstantiate = Instantiate(buttonServerPrefab.gameObject, serversGroupTransform);
			serverInstantiate.layer = serversGroupTransform.gameObject.layer;

			// Get the character component
			ButtonServer character = serverInstantiate.GetComponent<ButtonServer>();

			if (character == null)
				return;

			// Set the info
			character.SetServerInfo(info);
		}

		#region Unity

		private void Awake() {
			UIController.ClearChild(serversGroupTransform.transform);
		}

		private void Start() {
			Main.Singleton.Login.Servers.ForEach(AddServer);
		}

		#endregion

	}
}