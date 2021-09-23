using Asgla.Avatar;
using Asgla.Avatar.Monster;
using Asgla.Avatar.Player;
using Asgla.Data.Avatar;
using Asgla.Data.Map;
using Asgla.Data.Monster;
using Asgla.Data.Player;
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

using static Asgla.Request.RequestAvatar;
using static Asgla.Request.RequestDefault;
using static Asgla.Request.RequestMap;
using static Asgla.Request.RequestQuest;
using static Asgla.Request.RequestSetting;
using static Asgla.Request.RequestShop;

using static AsglaUI.UI.UISlotCooldown;

namespace Asgla.Request {
    public class RequestHandler {

        public Main Main = null;

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
                case RequestType.Login: {
                    LoginRequest login = JsonMapper.ToObject<LoginRequest>(json);

                    if (!login.Status) {
                        Main.UIManager.Modal.SetText2("Server login reply with an unknow status or false / Already logged?");
                        Main.UIManager.Modal.SetActiveConfirmButton(true);
                        return;
                    }

                    #if UNITY_WEBGL
                    Main.StartCoroutine(SchedulePing());
                    #endif

                    Main.UIManager.Modal.Close();

                    LoadingSceneOverlay loadingScene = Main.UIManager.CreateLoadingScene();
                    loadingScene.LoadScene(Main.SceneGame);
                    break;
                }
                case RequestType.PlayerUpdate: {
                    PlayerUpdate pu = JsonMapper.ToObject<PlayerUpdate>(json);
                    Main.AvatarManager.Create(pu.Player);
                    break;
                }
                case RequestType.Move: {
                    MoveRequest m = JsonMapper.ToObject<MoveRequest>(json);

                    AvatarMain avatar = m.Entity.Avatar;

                    if (avatar is null)
                        return;

                    avatar.Move(new Vector2((float) m.x, (float) m.y));
                    break;
                }
                case RequestType.JoinMap: {
                    LoadingMapOverlay loadingMap = Main.UIManager.LoadingOverlay == null ? Main.UIManager.CreateLoadingMap() : (LoadingMapOverlay) Main.UIManager.LoadingOverlay;
                    Main.AvatarManager.Monsters = new List<MapAvatar>();
                    loadingMap.LoadMap(JsonMapper.ToObject<JoinRoom>(json).Map);
                    break;
                }
                case RequestType.LeaveMap: {
                    LeaveRoomRequest lrr = JsonMapper.ToObject<LeaveRoomRequest>(json);

                    Player player = Main.MapManager.PlayerByID(lrr.PlayerID);

                    if (player is null)
                        return;

                    UnityEngine.Object.Destroy(player.gameObject);

                    Main.MapManager.RemovePlayerFromArea(lrr.PlayerID);
                    break;
                }
                case RequestType.Chat: {
                    ChatRequest chat = JsonMapper.ToObject<ChatRequest>(json);

                    //TODO: Chat
                    Main.Game.Chat.ReceiveChatMessage(
                        chat.Channel,
                        $"<size=22>{DateTime.Now.ToShortTimeString()}</size> <b><color=#{"d9d9d9"}>{chat.Username}</color></b> <color=#{CommonColorBuffer.ColorToString(Color.white)}>{chat.Message}</color>"
                    );
                    break;
                }
                case RequestType.EquipPart: {
                    EquipPart e = JsonMapper.ToObject<EquipPart>(json);

                    Player player = Main.MapManager.PlayerByID(e.PlayerID);

                    if (player is null)
                        return;

                    player.Equip(e);
                    break;
                }
                case RequestType.Notification: {
                    Notification n = JsonMapper.ToObject<Notification>(json);
                    Main.Game.NotificationTop.Init(null, n.Text);
                    break;
                }
                case RequestType.Pong: {
                    break;
                }
                case RequestType.Experience: {
                    Experience e = JsonMapper.ToObject<Experience>(json);
                    //Main.Game.ActionBar.GetExperienceBar().SetValueMax(e.Required);
                    //Main.Game.ActionBar.GetExperienceBar().SetValue(e.Current);
                    break;
                }
                case RequestType.MoveToArea: {
                    MoveToArea mta = JsonMapper.ToObject<MoveToArea>(json);

                    Player player = Main.MapManager.PlayerByID(mta.PlayerID);

                    if (player is null)
                        return;

                    Main.MapManager.UpdatePlayerArea(player, Main.MapManager.Map.AreaByName(mta.Area));
                    break;
                }
                case RequestType.PlayerDataLoad: {
                    DataLoad dl = JsonMapper.ToObject<DataLoad>(json);

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
                    break;
                }
                case RequestType.PlayerInventoryLoad: {
                    InventoryLoad pli = JsonMapper.ToObject<InventoryLoad>(json);

                    Player player = Main.MapManager.PlayerByID(pli.PlayerID);

                    if (player is null)
                        return;

                    player.Inventory(pli.Inventory);
                    break;
                }
                case RequestType.AvatarDataUpdate: {
                    DataUpdate s = JsonMapper.ToObject<DataUpdate>(json);

                    AvatarMain avatar = s.Entity.Avatar;

                    if (avatar is null)
                        return;

                    if (s.Stats is AvatarStats)
                        avatar.Stats(s.Stats);

                    if (s.State != AvatarState.NONE)
                        avatar.State(s.State);
                    break;
                }
                case RequestType.AvatarCombat: { //TODO: Replace with AvatarCombat
                    Combat c = JsonMapper.ToObject<Combat>(json);

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
                    break;
                }
                case RequestType.PlayerRespawn: {
                    Respawn r = JsonMapper.ToObject<Respawn>(json);

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
                    break;
                }
                case RequestType.SkillLoad: {
                    SkillLoad s = JsonMapper.ToObject<SkillLoad>(json);
                    Main.Game.ActionBar.SkillAssign(s.Skills);
                    break;
                }
                case RequestType.ShopLoad: {
                    ShopLoad s = JsonMapper.ToObject<ShopLoad>(json);

                    Main.Game.WindowShop.Init(s.Shop.Items);
                    Main.Game.WindowShop.Window().Show();
                    break;
                }
                case RequestType.QuestLoad: {
                    QuestLoad q = JsonMapper.ToObject<QuestLoad>(json);

                    Main.Game.WindowQuest.Init(q.Quests);
                    Main.Game.WindowQuest.Window().Show();
                    break;
                }
                case RequestType.PlayerInventoryUpdate: {
                    InventoryUpdate i = JsonMapper.ToObject<InventoryUpdate>(json);

                    Main.AvatarManager.Player.Inventory(i.Inventory);
                    break;
                }
                case RequestType.PlayerInventoryRemove: {
                    InventoryRemove i = JsonMapper.ToObject<InventoryRemove>(json);

                    foreach (PlayerInventory inventory in i.Inventory) {
                        Main.AvatarManager.Player.InventoryRemove(inventory.DatabaseID, inventory.Quantity);
                    }
                    break;
                }
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
        //Fix error "Abnormal disconnection"
        private IEnumerator SchedulePing() {

            //Wait
            yield return new WaitForSecondsRealtime(30);

            Send("Ping");

            //Reschedule
            Main.StartCoroutine(SchedulePing());
        }
        #endif

    }

    public class RequestMake {
        public string Cmd = "Login";

        public object[] Params;
    }

}