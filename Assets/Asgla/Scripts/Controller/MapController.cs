using Asgla.Avatar;
using Asgla.Avatar.Monster;
using Asgla.Avatar.Player;
using Asgla.Data.Map;
using Asgla.Data.Player;
using Asgla.Map;
using Asgla.NPC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asgla.Controller {
    public class MapController : Controller {

        public MapMain Map = null;

        #region Create
        public void Create(AreaData data, GameObject obj) {
            MapMain map = obj.GetComponent<MapMain>();
            map.Data(data); //if null maybe missing MapMain component

            foreach (MapArea area in map.Areas()) {
                area.Data(data.Areas.Where(aD => aD.Name == area.gameObject.name).FirstOrDefault()); //Set MapAreaData

                if (area.NPC() != null)
                    foreach (MapAreaNPC npcAreaData in area.Data().NPCs) {
                        NPCMain npc = area.NpcById(npcAreaData.DatabaseID);
                        if (npc != null)
                            npc.Data(npcAreaData);
                    }
            }

            List<string> players = new List<string>();

            bool update = false;

            if (Main.Singleton.MapManager.Map != null) {
                Main.Singleton.AvatarManager.Player.transform.SetParent(Main.Singleton.Game.transform);

                Object.Destroy(Main.Singleton.MapManager.Map.gameObject);

                update = true;
            }

            //Above area return null or old area
            Main.Singleton.MapManager.Map = map;

            foreach (PlayerData player in map.Data().Players) {
                if (update && player.playerID == Main.Singleton.AvatarManager.Player.Id()) {
                    Main.Singleton.MapManager.UpdatePlayerArea(Main.Singleton.AvatarManager.Player, map.AreaByName(player.Area.area), player.Area.position);
                    Main.Singleton.AvatarManager.Player.UpdateData(player);
                } else {
                    Main.Singleton.AvatarManager.Create(player, map);

                    players.Add(player.playerID.ToString());
                }
            }

            if (players.Count > 0)
                Main.Singleton.Request.Send("PlayerData", players.ToArray());

            map.Data().Monsters.ForEach(mapMonster => Main.Singleton.AvatarManager.Create(mapMonster));

            Main.Singleton.UIManager.LoadingOverlay = null;
        }
        #endregion

        public void UpdatePlayerArea(Player player, MapArea area) => UpdatePlayerArea(player, area, "Spawn");

        public void UpdatePlayerArea(Player player, MapArea area, string position) {
            //Debug.LogFormat("<color=teal>[MapManager]</color> AddPlayerToArea {0} {1} {2}", player.Data().Username, area.Name(), position);

            RemovePlayerFromArea(player.Data().playerID);

            Main.AvatarManager.Players.Add(new MapAvatar {
                Entity = player,
                Area = area
            });

            if (player.Area() != null)
                player.Area().OnPlayerScaleUpdate.RemoveListener(player.Scale);

            area.OnPlayerScaleUpdate.AddListener(player.Scale);

            player.transform.SetParent(area.Players());
            player.Area(area);

            Transform Zone = area.ZoneByName(position);

            if (Zone == null) {
                Zone = area.Zones().First();
            }

            if (Zone == null) {
                if (Main.UIManager.LoadingOverlay != null)
                    Main.UIManager.LoadingOverlay.SetLoadingText("Error(Null Zone) loading map, please contact Asgla Team.");
                return;
            }

            Vector3 target = Zone.position;

            target.z = 0;

            player.transform.position = target;

            player.Position(target);
        }

        public void SetMonsterArea(Monster monster, MapArea area) {
            Debug.LogFormat("<color=teal>[MapManager]</color> SetMonsterArea {0}({1}) {2}", monster.Data().Name, monster.Id(), area.Name());

            Main.AvatarManager.Monsters.Add(new MapAvatar {
                Entity = monster,
                Area = area
            });

            //if (monster.Area() != null)
            //    monster.Area().OnPlayerScaleUpdate.RemoveListener(monster.Scale);

            //area.OnPlayerScaleUpdate.AddListener(monster.Scale);
            //monster.Scale(0.5f);

            //monster.transform.SetParent(area.Players());
            monster.Area(area);

            /*Transform Zone = area.ZoneByName(position);

            if (Zone == null) {
                Zone = area.Zones().First();
            }

            if (Zone == null) {
                if (Main.UIManager.LoadingOverlay != null)
                    Main.UIManager.LoadingOverlay.SetLoadingText("Error(Null Zone) loading map, please contact Asgla Team.");
                return;
            }

            Vector3 target = Zone.position;

            target.z = 0;*/


            Vector3 pos = new Vector3 { x = (float)monster.Data().x, y = (float)monster.Data().y, z = 0 };

            monster.transform.position = pos;

            monster.Position(pos);

            Debug.LogFormat("<color=green>[MonsterMain]</color> Vector3({0}, {1}, {2})", monster.transform.position.x, monster.transform.position.y, monster.transform.position.z);
        }

        public Player PlayerByID(int playerID) => (Player) Main.AvatarManager.Players.Where(map => map.Entity.Id() == playerID).FirstOrDefault().Entity;

        public Monster MonsterByID(int monsterID) => (Monster) Main.AvatarManager.Monsters.Where(map => map.Entity.Id() == monsterID).FirstOrDefault().Entity;

        public HashSet<AvatarMain> FindAvatars(string name) {
            HashSet<AvatarMain> avatars = new HashSet<AvatarMain>();

            avatars
                .UnionWith(
                    Main.AvatarManager.Players
                        .Select(avatar => avatar.Entity)
                        .Where(mapEntity => name == mapEntity.Area().Name()
                    )
                );

            avatars
                .UnionWith(
                    Main.AvatarManager.Monsters
                        .Select(avatar => avatar.Entity)
                        .Where(mapEntity => name == mapEntity
                        .Area().Name()
                    )
                );

            return avatars;
        }

        public void RemovePlayerFromArea(int playerID) => Main.AvatarManager.Players.RemoveAll(pm => pm.Entity.Id() == playerID);

    }
}
