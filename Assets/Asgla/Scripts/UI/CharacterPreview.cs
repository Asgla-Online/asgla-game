using Asgla.Data.Type;
using AssetBundles;
using CharacterCreator2D;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using static Asgla.Request.RequestAvatar;
using static AssetBundles.AssetBundleManager;

using Weapon = CharacterCreator2D.Weapon;

namespace Asgla.UI {
    public class CharacterPreview : MonoBehaviour {

        [SerializeField] private RawImage _image;
        [SerializeField] private Camera _camera;

        private RenderTexture _renderTexture;

        private GameObject _character = null;

        private Animator _animator;

        private CharacterViewer _characterView;

        /*private void Start() {
            _character = Instantiate(Main.Singleton.AvatarManager.Player.CharacterView().gameObject, transform);

            _animator = _character.GetComponent<Animator>();
            _characterView = _character.GetComponent<CharacterViewer>();

            UpdateSprite();
            _camera.gameObject.transform.position = new Vector3(_character.transform.position.x, _character.transform.position.y, -1);
        }*/

        private void UpdateSprite() {
            _renderTexture = new RenderTexture((int)_image.rectTransform.rect.width, (int)_image.rectTransform.rect.height, 24, RenderTextureFormat.ARGB32);
            _renderTexture.filterMode = FilterMode.Bilinear;
            _camera.targetTexture = _renderTexture;
            _image.texture = _renderTexture;
            _image.SetNativeSize();
        }

        public void SetImage(RawImage i) {
            _image = i;
        }

        public void Equip(EquipPart equip) {
            if (_character != null)
                Destroy(_character);

            _character = Instantiate(Main.Singleton.AvatarManager.Player.CharacterView().gameObject, transform);

            Destroy(_character.GetComponent<Animator>());

            _character.transform.localPosition = Vector3.zero;
            _character.transform.localScale = new Vector3(1, 1, 1);

            _animator = _character.GetComponent<Animator>();
            _characterView = _character.GetComponent<CharacterViewer>();

            UpdateSprite();

            StartCoroutine(AsynchronousLoad(equip));
        }

        public Animator Animator() => _animator;

        public IEnumerator AsynchronousLoad(EquipPart equip) {
            AssetBundleManager abm = new AssetBundleManager();

            abm.DisableDebugLogging(true);
            abm.SetPrioritizationStrategy(PrioritizationStrategy.PrioritizeRemote);
            abm.SetBaseUri(Main.Singleton.url_bundle);

            var manifest = abm.InitializeAsync();
            yield return manifest;

            if (!manifest.Success) {
                yield break;
            }

            AssetBundleAsync assetBundle = abm.GetBundleAsync(equip.Bundle);

            abm.RegisterDownloadProgressHandler(equip.Bundle, UpdateProgress);

            yield return assetBundle;

            if (assetBundle.AssetBundle == null) {
                Debug.LogError("<color=green>[PlayerMain]</color> assetBundle null.");
                yield break;
            }

            AssetBundleRequest asyncAsset = assetBundle.AssetBundle.LoadAssetAsync($"assets/asgla/game/items/{equip.Asset}", typeof(Part));

            Part partAsset = asyncAsset.asset as Part;

            if (partAsset == null) {
                Debug.LogErrorFormat("<color=green>[PlayerMain]</color> part null asset: assets/asgla/game/items/{0}, bundle: {1}", equip.Asset, equip.Bundle);
                yield break;
            }

            if (partAsset.category == Category.Weapon) {
                Weapon part = (Weapon)partAsset;
                part.weaponCategory = equip.Type.Weapon;

                _characterView.EquipPart(equip.Type.Equipment, part);
            } else {
                if (equip.Type.Category != Category.Class)
                    _characterView.EquipPart(equip.Type.Equipment, partAsset);
            }

            _image.enabled = true;

            abm.UnloadBundle(assetBundle.AssetBundle);

            abm.Dispose();
        }

        private void UpdateProgress(float progress) {
            if (progress > 0)
                return;
        }

    }
}
