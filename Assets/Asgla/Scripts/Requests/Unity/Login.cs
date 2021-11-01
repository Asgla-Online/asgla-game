using Asgla.Controller;
using Asgla.UI.Loading;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class Login : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string message;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public bool status;

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

			LoadingSceneOverlay loadingScene = UIController.CreateLoadingScene();
			loadingScene.LoadScene(Main.SceneGame);
		}

	}
}