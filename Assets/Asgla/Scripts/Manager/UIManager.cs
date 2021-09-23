using Asgla.UI.Loading;
using AsglaUI.UI;
using UnityEngine;
using System.Linq;

namespace Asgla.Manager {
    public class UIManager {

        public Main Main;

        public LoadingOverlay LoadingOverlay;

        public UIModalBox Modal;

        public UIModalBox CreateModal(GameObject rel) {
            if (Modal != null)
                return Modal;
            Modal = UIModalBoxManager.Instance.Create(rel);
            return Modal;
            //GameObject obj = Object.Instantiate(UIModalBoxManager.Instance.prefab);
            //return obj.AddComponent<UIModalBox>();
        }

        public LoadingMapOverlay CreateLoadingMap() {
            GameObject obj = Object.Instantiate(Main.Singleton.Loading);
            return obj.AddComponent<LoadingMapOverlay>();
        }

        public LoadingAssetOverlay CreateLoadingAsset() {
            GameObject obj = Object.Instantiate(Main.Singleton.Loading);
            return obj.AddComponent<LoadingAssetOverlay>();
        }

        public LoadingSceneOverlay CreateLoadingScene() {
            GameObject obj = Object.Instantiate(Main.Singleton.Loading);
            return obj.AddComponent<LoadingSceneOverlay>();
        }

        /*public void ClearChild(params Transform[] transforms) {
            foreach (Transform child in transforms.Select(t => t)) {
                Debug.LogFormat("CLEAR {0}", child.name);
                Object.Destroy(child.gameObject);
            }
        }*/

        public void ClearChild(params Transform[] transforms) {
            foreach (Transform child in from Transform transform in transforms
                                  from Transform child in transform
                                  select child) {
                Object.Destroy(child.gameObject);
            }
        }

    }
}
