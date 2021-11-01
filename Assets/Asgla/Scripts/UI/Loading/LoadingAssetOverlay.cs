using System.Collections;
using Asgla.Controller;
using Asgla.Data.Effect;
using Asgla.Effect;
using AsglaUI.UI;
using AssetBundles;
using TMPro;
using UnityEngine;
using static AssetBundles.AssetBundleManager;

namespace Asgla.UI.Loading {
	public class LoadingAssetOverlay : LoadingOverlay {

		private void Awake() {
			_text = transform.GetComponentInChildren<TextMeshProUGUI>(); //TODO: Slow replace.

			_progressBar = gameObject.transform.Find("Loading Bar").GetComponent<UIProgressBar>();
			_canvasGroup = GetComponent<CanvasGroup>();

			name = "Loading Asset Overlay";
		}

		public void LoadAsset() {
			SetLoadingText("Loading Asset");

			//Debug.Log("<color=orange>[LoadingAssetOverlay]</color> LoadAsset");
			_showing = true;

			_progressBar.fillAmount = 0f;
			_canvasGroup.alpha = 1f;

			StartAlphaTween(1f, _transitionDuration, true);

			//_firstLoad = false;
		}

		protected override IEnumerator AsynchronousLoad() {
			//Debug.Log("<color=orange>[LoadingAssetOverlay]</color> LoadAsset 1");

			AssetBundleManager abm = new AssetBundleManager();

			abm.DisableDebugLogging();
			abm.SetPrioritizationStrategy(PrioritizationStrategy.PrioritizeRemote);
			abm.SetBaseUri(Main.URLBundle);

			AssetBundleManifestAsync manifest = abm.InitializeAsync();
			yield return manifest;

			if (!manifest.Success) {
				SetLoadingText("[Asset] Error initializing");
				yield break;
			}

			//Debug.Log("<color=orange>[LoadingAssetOverlay]</color> LoadAsset 2");

			string bundle = "asset";

			AssetBundleAsync assetBundle = abm.GetBundleAsync(bundle);

			abm.RegisterDownloadProgressHandler(bundle, UpdateProgress);

			yield return assetBundle;

			if (assetBundle.AssetBundle == null) {
				SetLoadingText("[Asset] Null");
				yield break;
			}

			GameObject obj = new GameObject("Effects GG", typeof(EffectMain));

			EffectMain em = obj.GetComponent<EffectMain>();

			foreach (GameObject go in assetBundle.AssetBundle.LoadAllAssets<GameObject>())
				if (go.GetComponent<ParticleSystem>() != null) {
					em.AddAsset(new EffectData {
						Name = go.name,
						Prefab = go.gameObject
					});

					if (go.GetComponent<AudioSource>() != null)
						go.GetComponent<AudioSource>().outputAudioMixerGroup = Main.Singleton.AudioMixer;
				}

			Main.Singleton.SetGameAsset(obj);

			abm.UnloadBundle(assetBundle.AssetBundle);

			//Debug.Log("<color=orange>[LoadingAssetOverlay]</color> LoadAsset Set loading map overlay");
			//Set loading map overlay
			UIController.CreateLoadingMap();
			Main.Singleton.UIManager.LoadingOverlay.SetLoadingText("LOADING MAP");

			Main.Singleton.Request.Send("JoinFirst");
			//---

			StartAlphaTween(0f, 1f, true);

			abm.Dispose();
		}

	}
}