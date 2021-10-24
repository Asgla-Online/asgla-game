using Asgla.UI.Loading;
using BestHTTP.JSON.LitJson;

// ReSharper disable InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global

namespace Asgla.Requests.Unity {
	public class Login : IRequest {

		public bool status;
		public string message;

		public void onRequest(Main main, string json) {
			Login login = JsonMapper.ToObject<Login>(json);
			
			if (!login.status) {
				main.UIManager.Modal.SetText2(login.message);
				main.UIManager.Modal.SetActiveConfirmButton(true);
				return;
			}

			#if UNITY_WEBGL
				//Fix error "Abnormal disconnection"
				Main.StartCoroutine(SchedulePing());
			#endif

			main.UIManager.Modal.Close();

			LoadingSceneOverlay loadingScene = main.UIManager.CreateLoadingScene();
			loadingScene.LoadScene(main.SceneGame);
		}

	}
}