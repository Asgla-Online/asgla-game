using Asgla.Avatar;
using Asgla.Avatar.Player;
using Asgla.Data.Avatar;
using Asgla.Data.Map;
using Asgla.Data.Player;
using Asgla.Data.Request;
using Asgla.Skill;
using Asgla.UI.Loading;
using Asgla.Utility;
using LitJson;
using System;
using System.Collections.Generic;
#if UNITY_WEBGL
using System.Collections;
#endif
using System.Linq;
using UnityEngine;

using static Asgla.Data.Request.RequestAvatar;
using static Asgla.Data.Request.RequestDefault;
using static Asgla.Data.Request.RequestMap;
using static Asgla.Data.Request.RequestQuest;
using static Asgla.Data.Request.RequestSetting;
using static Asgla.Data.Request.RequestShop;

using static AsglaUI.UI.UISlotCooldown;

namespace Asgla.Controller {
    public class RequestController : Controller {

        private enum RequestType {
            Default = 0,

            Login = 1,

            PlayerUpdate = 2,
            Move = 3,

            JoinMap = 4,
            LeaveMap = 5,

            Chat = 6,

            EquipPart = 7,

            Notification = 8,
            Pong = 9,

            Experience = 10,

            MoveToArea = 11,

            PlayerDataLoad = 12,
            PlayerInventoryLoad = 13,
            AvatarDataUpdate = 14,

            AvatarCombat = 15,

            PlayerRespawn = 16,

            SkillLoad = 17,

            ShopLoad = 18,

            QuestLoad = 19,

            PlayerInventoryUpdate = 20,
            PlayerInventoryRemove = 21,
        }

        public void Get(string json) {
            RequestType cmd;

            try {
                JsonData request = JsonMapper.ToObject(json);

                cmd = (RequestType) JsonUtil.ParseInt(request["cmd"]);
            } catch (Exception) {
                Debug.LogFormat("<color=orange>[INVALID] 1 </color> {0}", json);
                return;
            }

            Debug.LogFormat("<color=red>[RECEIVED]</color> {0}", json);

            switch (cmd) {
                case RequestType.Login:
                    Login(JsonMapper.ToObject<LoginRequest>(json));
                    break;
                case RequestType.PlayerUpdate:
                    PlayerUpdate(JsonMapper.ToObject<PlayerUpdate>(json));
                    break;
                case RequestType.Move:
                    Move(JsonMapper.ToObject<MoveRequest>(json));
                    break;
                case RequestType.JoinMap:
                    JoinMap(JsonMapper.ToObject<JoinRoom>(json));
                    break;
                case RequestType.LeaveMap:
                    LeaveMap(JsonMapper.ToObject<LeaveRoomRequest>(json));
                    break;
                case RequestType.Chat:
                    Chat(JsonMapper.ToObject<ChatRequest>(json));
                    break;
                case RequestType.EquipPart:
                    EquipPart(JsonMapper.ToObject<EquipPart>(json));
                    break;
                case RequestType.Notification:
                    Notification(JsonMapper.ToObject<Notification>(json));
                    break;
                case RequestType.Pong:
                    break;
                case RequestType.Experience:
                    Experience(JsonMapper.ToObject<Experience>(json));
                    break;
                case RequestType.MoveToArea:
                    MoveToArea(JsonMapper.ToObject<MoveToArea>(json));
                    break;
                case RequestType.PlayerDataLoad:
                    PlayerDataLoad(JsonMapper.ToObject<DataLoad>(json));
                    break;
                case RequestType.PlayerInventoryLoad:
                    PlayerInventoryLoad(JsonMapper.ToObject<InventoryLoad>(json));
                    break;
                case RequestType.AvatarDataUpdate:
                    AvatarDataUpdate(JsonMapper.ToObject<DataUpdate>(json));
                    break;
                case RequestType.AvatarCombat:
                    AvatarCombat(JsonMapper.ToObject<Combat>(json));
                    break;
                case RequestType.PlayerRespawn:
                    PlayerRespawn(JsonMapper.ToObject<Respawn>(json));
                    break;
                case RequestType.SkillLoad:
                    SkillLoad(JsonMapper.ToObject<SkillLoad>(json));
                    break;
                case RequestType.ShopLoad:
                    ShopLoad(JsonMapper.ToObject<ShopLoad>(json));
                    break;
                case RequestType.QuestLoad:
                    QuestLoad(JsonMapper.ToObject<QuestLoad>(json));
                    break;
                case RequestType.PlayerInventoryUpdate:
                    PlayerInventoryUpdate(JsonMapper.ToObject<InventoryUpdate>(json));
                    break;
                case RequestType.PlayerInventoryRemove:
                    PlayerInventoryRemove(JsonMapper.ToObject<InventoryRemove>(json));
                    break;
                case RequestType.Default:
                default:
                    Debug.LogFormat("<color=orange>[INVALID] 2 </color> {0}", json);
                    break;
            }
        }

        public void Send(string cmd) => Send(cmd, new string[1] { "" });

        public void Send(string cmd, params object[] args) {
            RequestMake obj = new RequestMake { Cmd = cmd, Params = args.ToArray() };

            string json = JsonMapper.ToJson(obj).ToString();

            Debug.LogFormat("<color=red>[SENDING]</color> {0}", json);

            if (Main.Network.Connection == null || !Main.Network.Connection.IsOpen) {
                Main.Game.Logout();
                return;
            }

            Main.Network.Connection.Send(json);
        }

        #if UNITY_WEBGL
        private IEnumerator SchedulePing() {

            //Wait
            yield return new WaitForSecondsRealtime(30);

            Send("Ping");

            //Reschedule
            Main.StartCoroutine(SchedulePing());
        }
        #endif

        #region Request get
        private void Login(LoginRequest login) {
            if (!login.Status) {
                Main.UIManager.Modal.SetText2("Server login reply with an unknow status or false / Already logged?");
                Main.UIManager.Modal.SetActiveConfirmButton(true);
                return;
            }

            #if UNITY_WEBGL
            //Fix error "Abnormal disconnection"
            Main.StartCoroutine(SchedulePing());
            #endif

            Main.UIManager.Modal.Close();

            LoadingSceneOverlay loadingScene = Main.UIManager.CreateLoadingScene();
            loadingScene.LoadScene(Main.SceneGame);
        }

        private void PlayerUpdate(PlayerUpdate pu) {
            Main.AvatarManager.Create(pu.Player);
        }

        private void Move(MoveRequest m) {
            AvatarMain avatar = m.Entity.Avatar;

            if (avatar is null)
                return;

            avatar.Move(new Vector2((float)m.x, (float)m.y));
        }

        private void JoinMap(JoinRoom joinRoom) {
            LoadingMapOverlay loadingMap = Main.UIManager.LoadingOverlay == null ? Main.UIManager.CreateLoadingMap() : (LoadingMapOverlay)Main.UIManager.LoadingOverlay;

            Main.AvatarManager.Monsters = new List<MapAvatar>();

            loadingMap.LoadMap(joinRoom.Map);
        }

        private void LeaveMap(LeaveRoomRequest lrr) {
            Player player = Main.MapManager.PlayerByID(lrr.PlayerID);

            if (player is null)
                return;

            UnityEngine.Object.Destroy(player.gameObject);

            Main.MapManager.RemovePlayerFromArea(lrr.PlayerID);
        }

        private void Chat(ChatRequest chat) {
            //TODO: Chat
            Main.Game.Chat.ReceiveChatMessage(
                chat.Channel,
                $"<size=22>{DateTime.Now.ToShortTimeString()}</size> <b><color=#{"d9d9d9"}>{chat.Username}</color></b> <color=#{CommonColorBuffer.ColorToString(Color.white)}>{chat.Message}</color>"
            );
        }

        private void EquipPart(EquipPart e) {
            Player player = Main.MapManager.PlayerByID(e.PlayerID);

            if (player is null)
                return;

            player.Equip(e);
        }

        private void Notification(Notification n) {
            Main.Game.NotificationTop.Init(null, n.Text);
        }

        private void Pong() {
        }

        private void Experience(Experience e) {
            //Main.Game.ActionBar.GetExperienceBar().SetValueMax(e.Required);
            //Main.Game.ActionBar.GetExperienceBar().SetValue(e.Current);
        }

        private void MoveToArea(MoveToArea mta) {
            Player player = Main.MapManager.PlayerByID(mta.PlayerID);

            if (player is null)
                return;

            Main.MapManager.UpdatePlayerArea(player, Main.MapManager.Map.AreaByName(mta.Area));
        }

        private void PlayerDataLoad(DataLoad dl) {
            foreach (DataUpdate2 du in dl.Players) {
                Player player = Main.MapManager.PlayerByID(du.Data.PlayerID);

                if (player is null)
                    continue;

                player.Data(du.Data);

                if (player.Data().Part != null)
                    foreach (EquipPart e in player.Data().Part)
                        player.Equip(e);

                player.Data().Part = null;

                player.Stats(du.Stats);
            }

            Send("PlayerInventory");
        }

        private void PlayerInventoryLoad(InventoryLoad pli) {
            Player player = Main.MapManager.PlayerByID(pli.PlayerID);

            if (player is null)
                return;

            player.Inventory(pli.Inventory);
        }

        private void AvatarDataUpdate(DataUpdate s) {
            AvatarMain avatar = s.Entity.Avatar;

            if (avatar is null)
                return;

            if (s.Stats is AvatarStats)
                avatar.Stats(s.Stats);

            if (s.State != AvatarState.NONE)
                avatar.State(s.State);
        }

        private void AvatarCombat(Combat c) { //TODO: Replace with AvatarCombat
            SkillMain skill = Main.Game.ActionBar.GetSkillBySlotID(c.Skill.SlotID);

            //reset skill cooldown
            if (c.Message != null) {
                Main.Game.Chat.ReceiveChatMessage(
                    1,
                    $"<size=22>{DateTime.Now.ToShortTimeString()}</size> <b><color=#{CommonColorBuffer.ColorToString(Color.red)}>{"game"}</color></b> <color=#{CommonColorBuffer.ColorToString(Color.white)}>{c.Message}</color>"
                );

                CooldownInfo cooldownInfo = new CooldownInfo(0f, Time.time, Time.time);

                // Save that this spell is on cooldown
                if (!skill.cooldownComponent.Cooldowns().ContainsKey(c.Skill.SlotID))
                    skill.cooldownComponent.Cooldowns().Add(c.Skill.SlotID, cooldownInfo);

                // Start the coroutine
                skill.cooldownComponent.StartCooldownCoroutine(cooldownInfo);
                return;
            }

            AvatarMain from = c.Entity.Avatar; //TODO: Find Monster

            if (from is null)
                return;

            //Debug.LogFormat("AvatarCombat PlayerMain {0}", c.Skill.SlotID);

            if (from is Player p) {
                if (p.Data().Controlling) {
                    //SkillData skillInfo = skill.GetSkillData();

                    //if (skillInfo.CastType == SkillCastType.CAST)
                    //    Main.Game.CastBar.StartCasting(skillInfo, skillInfo.CastTime, Time.time + skillInfo.CastTime);

                    CooldownInfo cooldownInfo = new CooldownInfo(c.Skill.Cooldown, Time.time, (Time.time + c.Skill.Cooldown));

                    // Save that this spell is on cooldown
                    if (!skill.cooldownComponent.Cooldowns().ContainsKey(c.Skill.SlotID))
                        skill.cooldownComponent.Cooldowns().Add(c.Skill.SlotID, cooldownInfo);

                    // Start the coroutine
                    skill.cooldownComponent.StartCooldownCoroutine(cooldownInfo);
                }
            }

            //TODO: Check if target/select exist in current player if not add.

            foreach (CombatResult result in c.Result) {
                AvatarMain target = result.Entity.Avatar;

                if (target is null)
                    continue;

                Main.AvatarManager.Combat(from, target, result, c.Animation);
            }
        }

        private void PlayerRespawn(Respawn r) {
            AvatarMain target = r.Entity.Avatar;

            if (target is null)
                return;

            if (target is Player player) {
                if (player.Data().Controlling)
                    Main.Game.WindowRespawn.Hide();

                Main.MapManager.UpdatePlayerArea(player, Main.MapManager.Map.AreaByName("Enter"));

                player.ResetCharacter();
            } else {
                //TODO: function set animation idle, hide gameobject ~reset~
                target.gameObject.SetActive(true);
            }
        }

        private void SkillLoad(SkillLoad s) {
            Main.Game.ActionBar.SkillAssign(s.Skills);
        }

        private void ShopLoad(ShopLoad s) {
            Main.Game.WindowShop.Init(s.Shop.Items);
            Main.Game.WindowShop.Window().Show();
        }

        private void QuestLoad(QuestLoad q) {
            Main.Game.WindowQuest.Init(q.Quests);
            Main.Game.WindowQuest.Window().Show();
        }

        private void PlayerInventoryUpdate(InventoryUpdate i) {
            Main.AvatarManager.Player.Inventory(i.Inventory);
        }

        private void PlayerInventoryRemove(InventoryRemove i) {
            foreach (PlayerInventory inventory in i.Inventory) {
                Main.AvatarManager.Player.InventoryRemove(inventory.DatabaseID, inventory.Quantity);
            }
        }
        #endregion

    }

}