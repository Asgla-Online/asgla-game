using System.Collections;
using Asgla.Data.Area;
using AsglaUI.UI;
using AssetBundles;
using TMPro;
using UnityEngine;
using static AssetBundles.AssetBundleManager;

namespace Asgla.UI.Loading {
	public class LoadingMapOverlay : LoadingOverlay {

		private AreaData _areaData;

		private void Awake() {
			_text = transform.GetComponentInChildren<TextMeshProUGUI>();

			_progressBar = gameObject.transform.Find("Loading Bar").GetComponent<UIProgressBar>();
			_canvasGroup = GetComponent<CanvasGroup>();

			name = "Loading Map Overlay";
		}

		public void LoadMap(AreaData areaData) {
			SetLoadingText("LOADING MAP");

			//Debug.LogFormat("<color=green>[LoadingMapOverlay]</color> LoadMap {0}", _firstLoad);
			_showing = true;
			_areaData = areaData;

			if (_progressBar != null && !_firstLoad)
				_progressBar.fillAmount = 0f;

			if (_canvasGroup != null && !_firstLoad)
				_canvasGroup.alpha = 0f;

			StartAlphaTween(1f, _transitionDuration, true);

			_firstLoad = false;
		}

		protected override IEnumerator AsynchronousLoad() {
			yield return null;

			AssetBundleManager abm = new AssetBundleManager();

			abm.DisableDebugLogging();
			abm.SetPrioritizationStrategy(PrioritizationStrategy.PrioritizeRemote);
			abm.SetBaseUri(Main.Singleton.url_bundle);

			AssetBundleManifestAsync manifest = abm.InitializeAsync();
			yield return manifest;

			if (!manifest.Success) {
				SetLoadingText("[Asset] Error initializing");
				yield break;
			}

			string bundle = "maps/" + _areaData.bundle;
			//string asset = MapData.Bundle + ".prefab";

			AssetBundleAsync assetBundle = abm.GetBundleAsync(bundle);

			abm.RegisterDownloadProgressHandler(bundle, UpdateProgress);

			yield return assetBundle;

			if (assetBundle.AssetBundle == null) {
				SetLoadingText("Error AssetBundle null.");
				yield break;
			}

			AssetBundleRequest asyncAsset =
				assetBundle.AssetBundle.LoadAssetAsync($"assets/asgla/game/maps/{_areaData.asset}", typeof(GameObject));

			GameObject map = asyncAsset.asset as GameObject;

			if (map == null) {
				Debug.LogErrorFormat(
					"<color=blue>[MapArea]</color> null GameObject: assets/asgla/game/items/{0}, bundle: {1}",
					_areaData.asset, _areaData.bundle);
				yield break;
			}

			GameObject obj = Instantiate(map, Vector3.zero, Quaternion.identity, Main.Singleton.Game.transform);

			if (obj == null) {
				SetLoadingText("Null error, please contact staff.");
				yield break;
			}

			obj.transform.localPosition = Vector3.zero;

			Main.Singleton.MapManager.Create(_areaData, obj);

			abm.UnloadBundle(assetBundle.AssetBundle);

			StartAlphaTween(0f, 1f, true); //Hide loading screen

			abm.Dispose();
		}

	}
}