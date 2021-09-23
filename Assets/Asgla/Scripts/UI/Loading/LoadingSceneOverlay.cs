using AsglaUI.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asgla.UI.Loading {
    public class LoadingSceneOverlay : LoadingOverlay {

        private int _loadSceneID = 0;

        protected void Awake() {
            DontDestroyOnLoad(gameObject);

            _text = transform.GetComponentInChildren<TextMeshProUGUI>();

            _progressBar = gameObject.transform.Find("Loading Bar").GetComponent<UIProgressBar>();
            _canvasGroup = GetComponent<CanvasGroup>();

            name = "Loading Scene Overlay";

            // Make sure it's top most ordering number
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            Canvas currentCanvas = gameObject.GetComponent<Canvas>();

            foreach (Canvas canvas in canvases) {
                // Make sure it's not our canvas1
                if (!canvas.Equals(currentCanvas)) {
                    if (canvas.sortingOrder > currentCanvas.sortingOrder)
                        currentCanvas.sortingOrder = canvas.sortingOrder + 1;
                }
            }

            _progressBar.fillAmount = 0f;
            _canvasGroup.alpha = 0f;
        }

        protected void OnEnable() {
            SceneManager.sceneLoaded += OnSceneFinishedLoading;
        }

        protected void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneFinishedLoading;
        }

        public void LoadScene(string sceneName) {
            Debug.LogWarningFormat("Loading Scene {0}", sceneName);

            Scene scene = SceneManager.GetSceneByName(sceneName);

            if (scene.IsValid())
                LoadScene(scene.buildIndex);
        }

        public void LoadScene(int sceneIndex) {
            Debug.LogWarningFormat("Loading Scene {0}", _loadSceneID);

            SetLoadingText("LOADING SCENE");

            _showing = true;
            _loadSceneID = sceneIndex;

            _progressBar.fillAmount = 0f;

            _canvasGroup.alpha = 0f;

            // Start the tween
            StartAlphaTween(1f, _transitionDuration, true);
        }


        protected override IEnumerator AsynchronousLoad() {
            yield return null;

            AsyncOperation ao = SceneManager.LoadSceneAsync(_loadSceneID);
            ao.allowSceneActivation = false;

            while (!ao.isDone) {
                // [0, 0.9] > [0, 1]
                float progress = Mathf.Clamp01(ao.progress / 0.9f);

                // Update the progress bar
                _progressBar.fillAmount = progress;

                // Loading completed
                if (ao.progress == 0.9f)
                    ao.allowSceneActivation = true;

                yield return null;
            }
        }

        private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
            if (scene.buildIndex != _loadSceneID)
                return;

            // Hide the loading overlay
            StartAlphaTween(0f, _transitionDuration, true);
        }

    }
}
