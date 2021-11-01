using System.Collections;
using System.Collections.Generic;
using Asgla.Data.Avatar;
using Asgla.Data.Avatar.Helper;
using Asgla.Data.Avatar.Player;
using Asgla.Utility;
using AssetBundles;
using CharacterCreator2D;
using UnityEngine;
using UnityEngine.Rendering;
using static AssetBundles.AssetBundleManager;

namespace Asgla.Avatar.Player {
	public class Player : AvatarMain {

		[SerializeField] private PlayerCharacter character;

		public PlayerData data;
		private readonly List<int> _loadingEquip = new List<int>();

		private CharacterViewer _characterView;

		//Loading stuff
		private int _loadingCount;

		private AvatarMain _target;

		private HashSet<AvatarMain> _targets;

		public void Data(PlayerData playerData) {
			data = playerData;
		}

		public void UpdateData(PlayerData playerData) {
			UpdateDataBody(playerData);

			if (playerData.area != Data().area && playerData.area != null)
				Data().area = playerData.area;

			if (playerData.level != Data().level && playerData.level > 0)
				Data().level = playerData.level;

			State(playerData.state);
		}

		public void UpdateDataBody(PlayerData playerData) {
			Equip(Data().Ear);
			Equip(Data().Eye);
			Equip(Data().Hair);
			Equip(Data().Mouth);
			Equip(Data().Nose);

			if (playerData.colorSkin != Data().colorSkin && !string.IsNullOrEmpty(playerData.colorSkin)) {
				Data().colorSkin = playerData.colorSkin;
				CharacterView().SetPartColor(SlotCategory.BodySkin, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorSkin));
			}

			if (playerData.colorEye != Data().colorEye && !string.IsNullOrEmpty(playerData.colorEye)) {
				Data().colorEye = playerData.colorEye;
				CharacterView().SetPartColor(SlotCategory.Eyes, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorEye));
			}

			if (playerData.colorHair != Data().colorHair && !string.IsNullOrEmpty(playerData.colorHair)) {
				Data().colorHair = playerData.colorHair;
				CharacterView().SetPartColor(SlotCategory.Hair, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorHair));
			}

			if (playerData.colorMouth != Data().colorMouth && !string.IsNullOrEmpty(playerData.colorMouth)) {
				Data().colorMouth = playerData.colorMouth;
				CharacterView().SetPartColor(SlotCategory.Mouth, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorMouth));
			}

			if (playerData.colorNose != Data().colorNose && !string.IsNullOrEmpty(playerData.colorNose)) {
				Data().colorNose = playerData.colorNose;
				CharacterView().SetPartColor(SlotCategory.Nose, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorNose));
			}
		}

		public void Inventory(List<PlayerInventory> list) {
			Data().inventory = list;
			Main.Singleton.Game.WindowInventory.Init(Data().inventory);
		}

		public void Inventory(PlayerInventory i) {
			Data().inventory.Add(i);
			Main.Singleton.Game.WindowInventory.AddItem(i.databaseId, i.item);

			Main.Singleton.Game.QuestController.CheckAll();
		}

		public void InventoryRemove(int databaseId, int quantity) {
			PlayerInventory inventory = Data().InventoryById(databaseId);

			int newQuantity = inventory.quantity - quantity;

			if (newQuantity > 0) {
				Debug.Log("1");
				inventory.DecreaseQuantity(quantity);
				//TODO: WindowInventory item quantity update
			} else {
				Data().inventory.Remove(inventory);
				Main.Singleton.Game.WindowInventory.RemoveItem(databaseId);
			}

			Main.Singleton.Game.QuestController.CheckAll();
		}

		public void Equip(EquipPart equip) {
			/*if (equip.type == null || equip.type.Category != Category.Class) {
				equip.uniqueId = _loadingCount++;
				_loadingEquip.Add(equip.uniqueId);
				StartCoroutine(AsynchronousLoad(equip));
			}*/
		}

		public void Equip2(EquipPart equip) {
			equip.uniqueId = _loadingCount++;
			_loadingEquip.Add(equip.uniqueId);
			StartCoroutine(AsynchronousLoad(equip));
		}

		public void ResetCharacter() {
			Animator().Rebind();
		}

		public void Emote(int i) {
			if (i - 1 < 0)
				_characterView.ResetEmote();
			else
				_characterView.Emote((EmotionType) i - 1);
		}

		public void SetAimAngle(float f) {
			Animator().SetFloat("Aim", f);
		}

		public void Target(AvatarMain target) {
			_target = target;
			_targets.Add(target);
		}

		public void TargetReset() {
			foreach (AvatarMain avatar in _targets) {
				if (avatar == null)
					_targets.Remove(avatar);
				avatar.Unselect();
			}

			_target = null;
			_targets = new HashSet<AvatarMain>();
		}

		public void TargetAdd(AvatarMain target) {
			_targets.Add(target);
		}

		public void MoveByClick(Vector2 vector) {
			Move(vector);

			if (Data().isControlling)
				Main.Singleton.Request.Send("Move", vector.x.ToString().Replace(",", "."),
					vector.y.ToString().Replace(",", "."));
		}

		public CharacterViewer CharacterView() {
			return _characterView;
		}

		public PlayerData Data() {
			return data;
		}

		public AvatarMain Target() {
			return _target;
		}

		public HashSet<AvatarMain> Targets() {
			return _targets;
		}

		#region Asset Bundle

		private IEnumerator AsynchronousLoad(EquipPart equip) {
			AssetBundleManager abm = new AssetBundleManager();

			abm.DisableDebugLogging();
			abm.SetPrioritizationStrategy(PrioritizationStrategy.PrioritizeRemote);
			abm.SetBaseUri(Main.Singleton.url_bundle);

			AssetBundleManifestAsync manifest = abm.InitializeAsync();

			yield return manifest;

			if (!manifest.Success)
				yield break;

			AssetBundleAsync assetBundle = abm.GetBundleAsync(equip.bundle);

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

			CharacterView().EquipPart(equip.type.Equipment, partAsset);

			_loadingEquip.Remove(equip.uniqueId);

			abm.UnloadBundle(assetBundle.AssetBundle);

			abm.Dispose();

			if (_loadingEquip.Count == 0) {
				character.GetComponent<SortingGroup>().sortingLayerName = "Avatar";
				character.GetComponent<SortingGroup>().sortingOrder = 1;
				Utility().LoadFlame.SetActive(false);
			} else {
				character.GetComponent<SortingGroup>().sortingLayerName = "Default";
				character.GetComponent<SortingGroup>().sortingOrder = -1;
				Utility().LoadFlame.SetActive(true);
			}
		}

		#endregion

		#region Unity

		private void Awake() {
			data = new PlayerData();

			_rigidBody2D = GetComponent<Rigidbody2D>();
			_animator = character.GetComponent<Animator>();
			_characterView = character.GetComponent<CharacterViewer>();

			character.GetComponent<SortingGroup>().sortingLayerName = "Default";
			character.GetComponent<SortingGroup>().sortingOrder = -1;

			character.Avatar(this);

			_utility.LoadFlame.SetActive(true);

			_targets = new HashSet<AvatarMain>();
		}

		private void Start() {
			Animator().Play("Idle", 0);
			_position = transform.position;

			if (_utility is null)
				Debug.LogError("Avatar Utility null");
		}

		private void OnDestroy() {
			Area().OnPlayerScaleUpdate.RemoveListener(Scale);
		}

		private void FixedUpdate() {
			if (_position == (Vector2) transform.position && Animator().GetBool("IsRunning"))
				Animator().SetBool("IsRunning", false);

			Body().MovePosition(Vector2.MoveTowards(transform.position, _position,
				Time.fixedDeltaTime * (float) Area().Speed()));

			character.transform.position = new Vector3(character.transform.position.x,
				character.transform.position.y, transform.position.y);
		}

		private void OnCollisionEnter2D(Collision2D collision) {
			Debug.Log("<color=green>[PlayerMain]</color> CollisionEnter");
			_position = transform.position;
			Animator().SetBool("IsRunning", false);
		}

		#endregion

		#region Abstract

		public override void Stats(AvatarStats stats) {
			if (Data().isControlling) {
				if (stats.HealthMax >= 0)
					Main.Singleton.Game.UnitFramePlayer.Health.SetValueMax(stats.HealthMax);

				if (stats.Health >= 0)
					Main.Singleton.Game.UnitFramePlayer.Health.SetValue(stats.Health);

				if (stats.EnergyMax >= 0)
					Main.Singleton.Game.UnitFramePlayer.Energy.SetValueMax(stats.EnergyMax);

				if (stats.Energy >= 0)
					Main.Singleton.Game.UnitFramePlayer.Energy.SetValue(stats.Energy);
			}

			base.Stats(stats);
		}

		public override void State(AvatarState state) {
			if (state != Data().state && state != AvatarState.NONE) {
				Data().state = state;
				switch (Data().state) {
					case AvatarState.NORMAL:
						Animator().Play("Idle", 0);
						break;
					case AvatarState.COMBAT:
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

			Animator().SetBool("IsRunning", true);
		}

		public override void OnDeath() {
			base.OnDeath();

			Main.Singleton.Game.WindowRespawn.Show();
		}

		public override void Attack(AvatarMain avatar, int damage) {
			TargetAdd(avatar);

			if (avatar is Player player)
				player.TargetAdd(this);

			base.Attack(avatar, damage);
		}

		public override void Unselect() {
			base.Unselect();

			_characterView.TintColor = Color.white;
			_characterView.RepaintTintColor();
		}

		public override int Id() {
			return Data().playerID;
		}

		public override int DatabaseId() {
			return Data().databaseID;
		}

		public override string Name() {
			return Data().username;
		}

		public override int Level() {
			return Data().level;
		}

		public override AvatarState State() {
			return Data().state;
		}

		public override EntityType Type() {
			return EntityType.PLAYER;
		}

		#endregion

	}
}