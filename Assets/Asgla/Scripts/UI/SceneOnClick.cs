using Asgla.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI {

	[AddComponentMenu("Asgla/Scene on click", 8)]
	[DisallowMultipleComponent]
	public class SceneOnClick : MonoBehaviour {

		[SerializeField] public int scene;

		private void Awake() {
			GetComponent<Button>().onClick.AddListener(delegate {
				UIController.CreateLoadingScene()
					.LoadScene(scene);
			});
		}

	}
}