using System.Collections;
using AsglaUI.UI;
using TMPro;
using UnityEngine;

namespace Asgla.Window {

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class RespawnWindow : UIWindow {

		[SerializeField] private TextMeshProUGUI _respawnTime;

		public override void Show() {
			base.Show();
			StartCoroutine(Countdown());
		}

		private IEnumerator Countdown() {
			int counter = 5;
			while (counter > 0) {
				yield return new WaitForSeconds(1);
				counter--;
				_respawnTime.text = $"Respawn {counter}s";
			}
		}

	}

}