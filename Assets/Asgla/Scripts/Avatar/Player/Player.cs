using Asgla.Data.Avatar;
using Asgla.Data.Entity;
using Asgla.Data.Player;
using Asgla.Data.Type;
using AssetBundles;
using CharacterCreator2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using static Asgla.Request.RequestAvatar;
using static AssetBundles.AssetBundleManager;

using Weapon = CharacterCreator2D.Weapon;

namespace Asgla.Avatar.Player {
    public class Player : AvatarMain {

        private PlayerData _data;

        [SerializeField] private PlayerCharacter _character;

        private CharacterViewer _characterView;

        private AvatarMain _target = null;

        private HashSet<AvatarMain> _targets = null;

        //Loading stuff
        private int _loadingCount = 0;
        private List<int> _loadingEquip = new List<int>();
        private bool _loadingBodyStart = false;

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
            _position = (Vector2) transform.position;

            if (_utility is null)
                Debug.LogError("Avatar Utility null");
        }

        private void OnDestroy() {
            Area().OnPlayerScaleUpdate.RemoveListener(Scale);
        }

        private void FixedUpdate() {
            if (_position == (Vector2)transform.position && Animator().GetBool("IsRunning")) {
                Animator().SetBool("IsRunning", false);
            }

            Body().MovePosition(Vector2.MoveTowards(transform.position, _position, Time.fixedDeltaTime * (float) Area().Speed()));

            _character.transform.position = new Vector3(_character.transform.position.x, _character.transform.position.y, transform.position.y);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            Debug.Log("<color=green>[PlayerMain]</color> CollisionEnter");
            _position = transform.position;
            Animator().SetBool("IsRunning", false);
        }
        #endregion

        public void Data(PlayerData d) => _data = d;

        public void UpdateData(PlayerData d) {
            //int PlayerID
            //int DatabaseID

            if (d.Username != Data().Username && !string.IsNullOrEmpty(d.Username))
                Data().Username = d.Username;

            //EquipPart Ear
            //EquipPart Eye
            //EquipPart Hai
            //EquipPart Mouth
            //EquipPart Nose

            if (d.ColorSkin != Data().ColorSkin && !string.IsNullOrEmpty(d.ColorSkin)) {
                Data().ColorSkin = d.ColorSkin;
                CharacterView().SetPartColor(Equipment.BodySkin, ColorCode.Color1, CommonColorBuffer.StringToColor(Data().ColorSkin));
            }

            if (d.ColorEye != Data().ColorEye && !string.IsNullOrEmpty(d.ColorEye)) {
                Data().ColorEye = d.ColorEye;
                CharacterView().SetPartColor(Equipment.Eye, ColorCode.Color1, CommonColorBuffer.StringToColor(Data().ColorEye));
            }

            if (d.ColorHair != Data().ColorHair && !string.IsNullOrEmpty(d.ColorHair)) {
                Data().ColorHair = d.ColorHair;
                CharacterView().SetPartColor(Equipment.Hair, ColorCode.Color1, CommonColorBuffer.StringToColor(Data().ColorHair));
            }

            if (d.ColorMouth != Data().ColorMouth && !string.IsNullOrEmpty(d.ColorMouth)) {
                Data().ColorMouth = d.ColorMouth;
                CharacterView().SetPartColor(Equipment.Mouth, ColorCode.Color1, CommonColorBuffer.StringToColor(Data().ColorMouth));
            }

            if (d.ColorNose != Data().ColorNose && !string.IsNullOrEmpty(d.ColorNose)) {
                Data().ColorNose = d.ColorNose;
                CharacterView().SetPartColor(Equipment.Nose, ColorCode.Color1, CommonColorBuffer.StringToColor(Data().ColorNose));
            }

            if (d.Area != Data().Area && d.Area != null)
                Data().Area = d.Area;

            //int x
            //int y

            if (d.Level != Data().Level && d.Level > 0)
                Data().Level = d.Level;

            //bool Away
            //bool Controlling

            State(d.State);
        }

        public void Inventory(List<PlayerInventory> list) {
            Data().Inventory = list;
            Main.Singleton.Game.WindowInventory.Init(Data().Inventory);
        }

        public void Inventory(PlayerInventory i) {
            Data().Inventory.Add(i);
            Main.Singleton.Game.WindowInventory.AddItem(i.DatabaseID, i.Item);
            
            Main.Singleton.Game.Quest.CheckAll();
        }

        public void InventoryRemove(int databaseId, int quantity) {
            PlayerInventory inventory = Data().InventoryById(databaseId);

            int newQuantity = inventory.Quantity - quantity;

            if (newQuantity > 0) {
                Debug.Log("1");
                inventory.DecreaseQuantity(quantity);
                //TODO: WindowInventory item quantity update
            } else {
                Data().Inventory.Remove(inventory);
                Main.Singleton.Game.WindowInventory.RemoveItem(databaseId);
            }

            Main.Singleton.Game.Quest.CheckAll();
        }

        public void Equip(EquipPart equip) {
            if (equip.Type == null || equip.Type.Category != Category.Class) {
                equip.UniqueID = _loadingCount++;
                _loadingEquip.Add(equip.UniqueID);
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
                _characterView.Emote((EmotionType)i - 1);
        }

        public void SetAimAngle(float f) => Animator().SetFloat("Aim", f);

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

        public void TargetAdd(AvatarMain target) => _targets.Add(target);

        public void MoveByClick(Vector2 vector) {
            Move(vector);

            if (Data().Controlling)
                Main.Singleton.Request.Send("Move", vector.x.ToString().Replace(",", "."), vector.y.ToString().Replace(",", "."));
        }

        #region Abstract
        public override void Stats(AvatarStats stats) {
            if (Data().Controlling) {
                if (stats.HealthMax >= 0) {
                    Main.Singleton.Game.UnitFramePlayer.Health.SetValueMax(stats.HealthMax);
                }

                if (stats.Health >= 0) {
                    Main.Singleton.Game.UnitFramePlayer.Health.SetValue(stats.Health);
                }

                if (stats.EnergyMax >= 0) {
                    Main.Singleton.Game.UnitFramePlayer.Energy.SetValueMax(stats.EnergyMax);
                }

                if (stats.Energy >= 0) {
                    Main.Singleton.Game.UnitFramePlayer.Energy.SetValue(stats.Energy);
                }
            }

            base.Stats(stats);
        }

        public override void State(AvatarState state) {
            if (state != Data().State && state != AvatarState.NONE) {
                Data().State = state;
                switch (Data().State) {
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

        public override int Id() => Data().PlayerID;

        public override int DatabaseId() => Data().DatabaseID;

        public override string Name() => Data().Username;

        public override int Level() => Data().Level;

        public override AvatarState State() => Data().State;

        public override EntityType Type() => EntityType.PLAYER;
        #endregion

        public CharacterViewer CharacterView() => _characterView;

        public PlayerData Data() => _data;

        public AvatarMain Target() => _target;

        public HashSet<AvatarMain> Targets() => _targets;

        #region Asset Bundle
        public IEnumerator AsynchronousLoad(EquipPart equip) {
            AssetBundleManager abm = new AssetBundleManager();

            abm.DisableDebugLogging(true);
            abm.SetPrioritizationStrategy(PrioritizationStrategy.PrioritizeRemote);
            abm.SetBaseUri(Main.Singleton.url_bundle);

            var manifest = abm.InitializeAsync();

            yield return manifest;

            if (!manifest.Success)
                yield break;

            AssetBundleAsync assetBundle = abm.GetBundleAsync(equip.Bundle);

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

            AssetBundleRequest asyncAsset = assetBundle.AssetBundle.LoadAssetAsync($"assets/asgla/game/items/{equip.Asset}", typeof(Part));

            Part partAsset = asyncAsset.asset as Part;

            if (partAsset == null) {
                Debug.LogErrorFormat("<color=green>[PlayerMain]</color> part null asset: assets/asgla/game/items/{0}, bundle: {1}", equip.Asset, equip.Bundle);
                yield break;
            }

            if (equip.Type == null) {
                _characterView.EquipPart(equip.Equipment, partAsset);
            } else {
                _loadingBodyStart = true;

                switch (partAsset.category) {
                    case Category.Weapon: {
                        Weapon part = (Weapon)partAsset;
                        part.weaponCategory = equip.Type.Weapon;

                        _characterView.EquipPart(equip.Type.Equipment, part);
                        break;
                    }
                    default:
                        _characterView.EquipPart(equip.Type.Equipment, partAsset);
                        break;
                }
            }

            _loadingEquip.Remove(equip.UniqueID);

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

    }
}