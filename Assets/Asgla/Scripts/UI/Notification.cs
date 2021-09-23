using AsglaUI.UI;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Asgla.UI {
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(CanvasGroup))]
    public class Notification : UIWindow {

        [SerializeField] private TextMeshProUGUI _title = null;

        [SerializeField] private TextMeshProUGUI _description = null;

        private Coroutine _coroutine = null;

        private bool _running = false;

        public void Init(string title, string description) {
            if (title == null)
                _title.gameObject.SetActive(false);
            else {
                _title.gameObject.SetActive(true);
                _title.text = title;
            }

            _description.text = description;

            Show();

            if (_running)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(DoSomething());
        }

        private IEnumerator DoSomething() {
            _running = true;

            //Wait
            yield return new WaitForSecondsRealtime(3);

            Hide();
            _running = false;
        }

    }
}