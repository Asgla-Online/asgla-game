using System.Collections;
using Asgla.Data.Avatar.Helper;
using AssetBundles;
using CharacterCreator2D;
using UnityEngine;
using UnityEngine.UI;
using static AssetBundles.AssetBundleManager;

namespace Asgla.UI {
	public class CharacterPreview : MonoBehaviour {

		[SerializeField] private RawImage _image;
		[SerializeField] private Camera _camera;

		private Animator _animator;

		private GameObject _character;

		private CharacterViewer _characterView;

		private RenderTexture _renderTexture;

		/*private void Start() {
		    _character = Instantiate(Main.Singleton.AvatarManager.Player.CharacterView().gameObject, transform);

		    _animator = _character.GetComponent<Animator>();
		    _characterView = _character.GetComponent<CharacterViewer>();

		    UpdateSprite();
		    _camera.gameObject.transform.position = new Vector3(_character.transform.position.x, _character.transform.position.y, -1);
		}*/

		private void UpdateSprite() {
			_renderTexture = new RenderTexture((int) _image.rectTransform.rect.width,
				(int) _image.rectTransform.rect.height, 24, RenderTextureFormat.ARGB32);
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

			_character = Instantiate(Main.Singleton.Game.AvatarController.Player.CharacterView().gameObject, transform);

			Destroy(_character.GetComponent<Animator>());

			_character.transform.localPosition = Vector3.zero;
			_character.transform.localScale = new Vector3(1, 1, 1);

			_animator = _character.GetComponent<Animator>();
			_characterView = _character.GetComponent<CharacterViewer>();

			UpdateSprite();

			StartCoroutine(AsynchronousLoad(equip));
		}

		public Animator Animator() {
			return _animator;
		}

		public IEnumerator AsynchronousLoad(EquipPart equip) {
			AssetBundleManager abm = new AssetBundleManager();

			abm.DisableDebugLogging();
			abm.SetPrioritizationStrategy(PrioritizationStrategy.PrioritizeRemote);
			abm.SetBaseUri(Main.URLBundle);

			AssetBundleManifestAsync manifest = abm.InitializeAsync();
			yield return manifest;

			if (!manifest.Success)
				yield break;

			AssetBundleAsync assetBundle = abm.GetBundleAsync(equip.bundle);

			abm.RegisterDownloadProgressHandler(equip.bundle, UpdateProgress);

			yield return assetBundle;

			if (assetBundle.AssetBundle == null) {
				Debug.LogError("<color=green>[PlayerMain]</color> assetBundle null.");
				yield break;
			}

			AssetBundleRequest asyncAsset =
				assetBundle.AssetBundle.LoadAssetAsync($"assets/asgla/game/items/{equip.asset}", typeof(Part));

			Part partAsset = asyncAsset.asset as Part;

			if (partAsset == null) {
				Debug.LogErrorFormat(
					"<color=green>[PlayerMain]</color> part null asset: assets/asgla/game/items/{0}, bundle: {1}",
					equip.asset, equip.bundle);
				yield break;
			}

			_characterView.EquipPart(equip.type.Equipment, partAsset);

			/*if (partAsset.category == PartCategory.Weapon) {
				Weapon part = (Weapon) partAsset;
				part.weaponCategory = equip.type.Weapon;

				_characterView.EquipPart(equip.type.Equipment, part);
			} else {
				if (equip.type.Category != PartCategory.Class)
					_characterView.EquipPart(equip.type.Equipment, partAsset);
			}*/

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