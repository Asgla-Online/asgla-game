using AsglaUI.UI;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Asgla.Window {

    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(CanvasGroup))]
    public class RespawnWindow : UIWindow {

        [SerializeField] private TextMeshProUGUI _respawnTime = null;

        public override void Show() {
            base.Show();
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown() {
            int counter = 5;
            while (counter > 0) {
                yield return new WaitForSeconds(1);
                counter--;
                _respawnTime.text = $"Respawn {(int)counter}s";
            }
        }

    }

}
