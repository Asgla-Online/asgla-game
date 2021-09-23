using Asgla.Data.Map;
using AsglaUI.UI;
using AssetBundles;
using System.Collections;
using TMPro;
using UnityEngine;

using static AssetBundles.AssetBundleManager;

namespace Asgla.UI.Loading {
    public class LoadingMapOverlay : LoadingOverlay {

        private void Awake() {
            _text = transform.GetComponentInChildren<TextMeshProUGUI>();

            _progressBar = gameObject.transform.Find("Loading Bar").GetComponent<UIProgressBar>();
            _canvasGroup = GetComponent<CanvasGroup>();

            name = "Loading Map Overlay";
        }

        private MapData MapData;

        public void LoadMap(MapData mapData) {
            SetLoadingText("LOADING MAP");

            //Debug.LogFormat("<color=green>[LoadingMapOverlay]</color> LoadMap {0}", _firstLoad);
            _showing = true;
            MapData = mapData;

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

            abm.DisableDebugLogging(true);
            abm.SetPrioritizationStrategy(PrioritizationStrategy.PrioritizeRemote);
            abm.SetBaseUri(Main.Singleton.url_bundle);

            var manifest = abm.InitializeAsync();
            yield return manifest;

            if (!manifest.Success) {
                SetLoadingText("[Asset] Error initializing");
                yield break;
            }

            string bundle = "maps/" + MapData.Bundle;
            //string asset = MapData.Bundle + ".prefab";

            AssetBundleAsync assetBundle = abm.GetBundleAsync(bundle);

            abm.RegisterDownloadProgressHandler(bundle, UpdateProgress);

            yield return assetBundle;

            if (assetBundle.AssetBundle == null) {
                SetLoadingText("Error AssetBundle null.");
                yield break;
            }

            AssetBundleRequest asyncAsset = assetBundle.AssetBundle.LoadAssetAsync($"assets/asgla/game/maps/{MapData.Asset}", typeof(GameObject));

            GameObject Map = asyncAsset.asset as GameObject;

            if (Map == null) {
                Debug.LogErrorFormat("<color=blue>[MapArea]</color> null GameObject: assets/asgla/game/items/{0}, bundle: {1}", MapData.Asset, MapData.Bundle);
                yield break;
            }

            GameObject obj = Instantiate(Map, Vector3.zero, Quaternion.identity, Main.Singleton.Game.transform);

            if (obj == null) {
                SetLoadingText("Null error, please contact staff.");
                yield break;
            }

            obj.transform.localPosition = Vector3.zero;

            Main.Singleton.MapManager.Create(MapData, obj);

            abm.UnloadBundle(assetBundle.AssetBundle);

            StartAlphaTween(0f, 1f, true); //Hide loading screen

            abm.Dispose();
        }

    }
}
