using System.Collections;
using AsglaUI.UI;
using TMPro;
using UnityEngine;

namespace Asgla.UI.Window {

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class RespawnWindow : UIWindow {

		[SerializeField] private TextMeshProUGUI respawnTime;

		public override void Show() {
			base.Show();
			StartCoroutine(Countdown());
		}

		private IEnumerator Countdown() {
			int counter = 5;
			while (counter > 0) {
				yield return new WaitForSeconds(1);
				counter--;
				respawnTime.text = $"Respawn {counter}s";
			}
		}

	}

}