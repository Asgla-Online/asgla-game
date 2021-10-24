using System.Collections;
using System.Collections.Generic;
using Asgla.Data.Avatar;
using Asgla.Data.Entity;
using Asgla.Data.Player;
using Asgla.Data.Type;
using Asgla.Utility;
using AssetBundles;
using CharacterCreator2D;
using UnityEngine;
using UnityEngine.Rendering;
using static AssetBundles.AssetBundleManager;
using Weapon = CharacterCreator2D.Weapon;

namespace Asgla.Avatar.Player {
	public class Player : AvatarMain {

		[SerializeField] private PlayerCharacter _character;
		private readonly List<int> _loadingEquip = new List<int>();

		private CharacterViewer _characterView;

		private PlayerData _data;
		private bool _loadingBodyStart;

		//Loading stuff
		private int _loadingCount;

		private AvatarMain _target;

		private HashSet<AvatarMain> _targets;

		public void Data(PlayerData d) {
			_data = d;
		}

		public void UpdateData(PlayerData d) {
			//int PlayerID
			//int DatabaseID

			if (d.username != Data().username && !string.IsNullOrEmpty(d.username))
				Data().username = d.username;

			//EquipPart Ear
			//EquipPart Eye
			//EquipPart Hai
			//EquipPart Mouth
			//EquipPart Nose

			if (d.colorSkin != Data().colorSkin && !string.IsNullOrEmpty(d.colorSkin)) {
				Data().colorSkin = d.colorSkin;
				CharacterView().SetPartColor(Equipment.BodySkin, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorSkin));
			}

			if (d.colorEye != Data().colorEye && !string.IsNullOrEmpty(d.colorEye)) {
				Data().colorEye = d.colorEye;
				CharacterView().SetPartColor(Equipment.Eye, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorEye));
			}

			if (d.colorHair != Data().colorHair && !string.IsNullOrEmpty(d.colorHair)) {
				Data().colorHair = d.colorHair;
				CharacterView().SetPartColor(Equipment.Hair, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorHair));
			}

			if (d.colorMouth != Data().colorMouth && !string.IsNullOrEmpty(d.colorMouth)) {
				Data().colorMouth = d.colorMouth;
				CharacterView().SetPartColor(Equipment.Mouth, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorMouth));
			}

			if (d.colorNose != Data().colorNose && !string.IsNullOrEmpty(d.colorNose)) {
				Data().colorNose = d.colorNose;
				CharacterView().SetPartColor(Equipment.Nose, ColorCode.Color1,
					CommonColorBuffer.StringToColor(Data().colorNose));
			}

			if (d.Area != Data().Area && d.Area != null)
				Data().Area = d.Area;

			//int x
			//int y

			if (d.level != Data().level && d.level > 0)
				Data().level = d.level;

			//bool Away
			//bool Controlling

			State(d.state);
		}

		public void Inventory(List<PlayerInventory> list) {
			Data().inventory = list;
			Main.Singleton.Game.WindowInventory.Init(Data().inventory);
		}

		public void Inventory(PlayerInventory i) {
			Data().inventory.Add(i);
			Main.Singleton.Game.WindowInventory.AddItem(i.databaseId, i.item);

			Main.Singleton.Game.Quest.CheckAll();
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

			Main.Singleton.Game.Quest.CheckAll();
		}

		public void Equip(EquipPart equip) {
			if (equip.type == null || equip.type.Category != Category.Class) {
				equip.uniqueId = _loadingCount++;
				_loadingEquip.Add(equip.uniqueId);
				StartCoroutine(AsynchronousLoad(equip));
			}
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
			return _data;
		}

		public AvatarMain Target() {
			return _target;
		}

		public HashSet<AvatarMain> Targets() {
			return _targets;
		}

		#region Asset Bundle

		public IEnumerator AsynchronousLoad(EquipPart equip) {
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

			/*if (equip.Type is null) {
			    foreach (string objet in assetBundle.AssetBundle.GetAllAssetNames()) {
			        Debug.LogFormat("<color=green>[PlayerMain]</color> GetAllAssetNames {0}", objet);
			    }
			    foreach (Object objet in assetBundle.AssetBundle.LoadAllAssets()) {
			        Debug.LogFormat("<color=green>[PlayerMain]</color> LoadAllAssets {0}", objet);
			    }
			}*/

			AssetBundleRequest asyncAsset =
				assetBundle.AssetBundle.LoadAssetAsync($"assets/asgla/game/items/{equip.asset}", typeof(Part));

			Part partAsset = asyncAsset.asset as Part;

			if (partAsset == null) {
				Debug.LogErrorFormat(
					"<color=green>[PlayerMain]</color> part null asset: assets/asgla/game/items/{0}, bundle: {1}",
					equip.asset, equip.bundle);
				yield break;
			}

			if (equip.type == null) {
				_characterView.EquipPart(equip.equipment, partAsset);
			} else {
				_loadingBodyStart = true;

				switch (partAsset.category) {
					case Category.Weapon: {
						Weapon part = (Weapon) partAsset;
						part.weaponCategory = equip.type.Weapon;

						_characterView.EquipPart(equip.type.Equipment, part);
						break;
					}
					default:
						_characterView.EquipPart(equip.type.Equipment, partAsset);
						break;
				}
			}

			_loadingEquip.Remove(equip.uniqueId);

			abm.UnloadBundle(assetBundle.AssetBundle);

			abm.Dispose();

			if (_loadingEquip.Count == 0 && _loadingBodyStart) {
				_character.GetComponent<SortingGroup>().sortingLayerName = "Avatar";
				_character.GetComponent<SortingGroup>().sortingOrder = 1;
				Utility().LoadFlame.SetActive(false);
			} else {
				_character.GetComponent<SortingGroup>().sortingLayerName = "Default";
				_character.GetComponent<SortingGroup>().sortingOrder = -1;
				Utility().LoadFlame.SetActive(true);
			}
		}

		#endregion

		#region Unity

		private void Awake() {
			_data = new PlayerData();

			_rigidBody2D = GetComponent<Rigidbody2D>();
			_animator = _character.GetComponent<Animator>();
			_characterView = _character.GetComponent<CharacterViewer>();

			_character.GetComponent<SortingGroup>().sortingLayerName = "Default";
			_character.GetComponent<SortingGroup>().sortingOrder = -1;

			_character.Avatar(this);

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

			_character.transform.position = new Vector3(_character.transform.position.x,
				_character.transform.position.y, transform.position.y);
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