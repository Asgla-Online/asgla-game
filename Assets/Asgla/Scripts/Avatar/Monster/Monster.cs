using System.Collections;
using Asgla.Data.Avatar;
using Asgla.Data.Avatar.Helper;
using Asgla.Data.Avatar.Monster;
using AssetBundles;
using UnityEngine;
using static AssetBundles.AssetBundleManager;

namespace Asgla.Avatar.Monster {
	public class Monster : AvatarMain {

		private bool _animatorController;

		private MonsterData _data;

		private MonsterRoot _root;

		public void Init() {
			Debug.Log("<color=green>[MonsterMain]</color> Init.");
			//_data = new PlayerData();

			_rigidBody2D = GetComponent<Rigidbody2D>();
			_animator = _root.GetComponent<Animator>();
			//_characterView = _root.GetComponent<CharacterViewer>();

			//_root.GetComponent<SortingGroup>().sortingLayerName = "Default";
			//_root.GetComponent<SortingGroup>().sortingOrder = -1;

			gameObject.SetActive(true);

			_root.Avatar(this);

			_animatorController = Animator().runtimeAnimatorController != null;

			//TODO: monster data scale
			//Scale(0.5f);
		}

		public void Data(MonsterData d) {
			_data = d;
		}

		public bool AnimatorControllerExist() {
			return _animatorController;
		}

		#region Asset Bundle

		public IEnumerator AsynchronousLoad() {
			AssetBundleManager abm = new AssetBundleManager()
				.DisableDebugLogging()
				.SetPrioritizationStrategy(PrioritizationStrategy.PrioritizeRemote)
				.SetBaseUri(Main.URLBundle);

#if UNITY_EDITOR
			abm.UseSimulatedUri();
#endif

			AssetBundleManifestAsync manifest = abm.InitializeAsync();

			yield return manifest;

			if (!manifest.Success)
				yield break;

			AssetBundleAsync assetBundle = abm.GetBundleAsync($"monsters/{Data().Bundle}");

			yield return assetBundle;

			if (assetBundle.AssetBundle == null) {
				Debug.LogError("<color=green>[MonsterMain]</color> assetBundle null.");
				yield break;
			}

			AssetBundleRequest asyncAsset =
				assetBundle.AssetBundle.LoadAssetAsync($"assets/asgla/game/monsters/{Data().Asset}",
					typeof(GameObject));

			GameObject root = asyncAsset.asset as GameObject;

			if (root == null) {
				Debug.LogErrorFormat(
					"<color=green>[MonsterMain]</color> GameObject null asset: assets/asgla/game/monsters/{0}, bundle: {1}",
					Data().Asset, Data().Bundle);
				yield break;
			}

			_root = Instantiate(root, transform).GetComponent<MonsterRoot>();

			_root.transform.localPosition = Vector3.zero;
			_root.transform.localRotation = Quaternion.identity;

			Init();

			abm.UnloadBundle(assetBundle.AssetBundle);

			abm.Dispose();
		}

		#endregion

		#region Unity

		private void Start() {
			if (AnimatorControllerExist())
				Animator().Play("Idle", 0);

			_position = transform.position;

			if (_utility is null)
				Debug.LogError("Avatar Utility null");
		}

		private void OnDestroy() {
			Area().OnPlayerScaleUpdate.RemoveListener(Scale);
		}

		private void FixedUpdate() {
			if (AnimatorControllerExist())
				if (_position == (Vector2) transform.position && Animator().GetBool("IsRunning"))
					Animator().SetBool("IsRunning", false);

			Body().MovePosition(Vector2.MoveTowards(transform.position, _position,
				Time.fixedDeltaTime * ((float) Area().Speed() / 3)));

			_root.transform.position =
				new Vector3(_root.transform.position.x, _root.transform.position.y, transform.position.y);
		}

		private void OnCollisionEnter2D(Collision2D collision) {
			//Debug.Log("<color=green>[PlayerMain]</color> CollisionEnter");
			_position = transform.position;
			if (AnimatorControllerExist())
				Animator().SetBool("IsRunning", false);
		}

		#endregion

		#region Abstract

		public override void State(AvatarState state) {
			if (state != Data().State && state != AvatarState.NONE) {
				Data().State = state;
				switch (Data().State) {
					case AvatarState.NORMAL:
						if (AnimatorControllerExist())
							Animator().Play("Idle", 0);
						break;
					case AvatarState.COMBAT:
						if (AnimatorControllerExist())
							Animator().SetTrigger("IsHurting");
						break;
					case AvatarState.DEAD:
						OnDeath();
						break;
				}
			}
		}

		public override void Move(Vector2 vector) {
			base.Move(vector);

			if (AnimatorControllerExist())
				Animator().SetBool("IsRunning", true);
		}

		public override void OnDeath() {
			base.OnDeath();

			gameObject.SetActive(false);
		}

		public override int Id() {
			return Data().UniqueID;
		}

		public override int DatabaseId() {
			return Data().DatabaseID;
		}

		public override string Name() {
			return Data().Name;
		}

		public override int Level() {
			return Data().Level;
		}

		public override AvatarState State() {
			return Data().State;
		}

		public override EntityType Type() {
			return EntityType.Monster;
		}

		public MonsterData Data() {
			return _data;
		}

		#endregion

	}
}