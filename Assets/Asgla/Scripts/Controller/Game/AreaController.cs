using System.Collections.Generic;
using System.Linq;
using Asgla.Avatar;
using Asgla.Avatar.Monster;
using Asgla.Avatar.Player;
using Asgla.Data.Area;
using Asgla.Data.Avatar.Player;
using UnityEngine;
using AreaLocal = Asgla.Area.AreaLocal;

namespace Asgla.Controller.Game {
	public class AreaController : Controller {

		public Area.Area Map;

		#region Create

		public void Create(AreaData data, GameObject obj) {
			Area.Area map = obj.GetComponent<Area.Area>();
			map.Data(data); //if null maybe missing MapMain component

			foreach (AreaLocal area in map.Areas()) {
				area.Data(data.locals.FirstOrDefault(aD => aD.name == area.gameObject.name)); //Set MapAreaData

				/*if (area.NPC() != null)
					foreach (AreaLocalNpc npcAreaData in area.Data().npcs) {
						NPCMain npc = area.NpcById(npcAreaData.databaseId);
						if (npc != null)
							npc.Data(npcAreaData);
					}*/
			}

			List<string> players = new List<string>();

			bool update = false;

			if (Main.Game.AreaController.Map != null) {
				Main.Game.AvatarController.Player.transform.SetParent(Main.Game.transform);

				Object.Destroy(Main.Game.AreaController.Map.gameObject);

				update = true;
			}

			//Above area return null or old area
			Main.Game.AreaController.Map = map;

			foreach (PlayerData player in map.Data().players)
				if (update && player.playerID == Main.Game.AvatarController.Player.Id()) {
					Main.Game.AreaController.UpdatePlayerArea(Main.Game.AvatarController.Player,
						map.AreaByName(player.area.area), player.area.point);
					Main.Game.AvatarController.Player.UpdateData(player);
				} else {
					Main.Game.AvatarController.Create(player, map);

					players.Add(player.playerID.ToString());
				}

			if (players.Count > 0)
				Main.Request.Send("PlayerData", players.ToArray());

			/*map.Data().monsters.ForEach(mapMonster => Main.Game.AvatarManager.Create(mapMonster));*/

			Main.UIManager.LoadingOverlay = null;
		}

		#endregion

		public void UpdatePlayerArea(Player player, AreaLocal areaLocal) {
			UpdatePlayerArea(player, areaLocal, "Spawn");
		}

		public void UpdatePlayerArea(Player player, AreaLocal areaLocal, string position) {
			//Debug.LogFormat("<color=teal>[MapManager]</color> AddPlayerToArea {0} {1} {2}", player.Data().Username, area.Name(), position);

			RemovePlayerFromArea(player.Data().playerID);

			Main.Game.AvatarController.Players.Add(new AreaAvatar {
				entity = player,
				areaLocal = areaLocal
			});

			if (player.Area() != null)
				player.Area().OnPlayerScaleUpdate.RemoveListener(player.Scale);

			areaLocal.OnPlayerScaleUpdate.AddListener(player.Scale);

			player.transform.SetParent(areaLocal.Players());
			player.Area(areaLocal);

			Transform Zone = areaLocal.ZoneByName(position);

			if (Zone == null)
				Zone = areaLocal.Zones().First();

			if (Zone == null) {
				if (Main.UIManager.LoadingOverlay != null)
					Main.UIManager.LoadingOverlay.SetLoadingText(
						"Error(Null Zone) loading map, please contact Asgla Team.");
				return;
			}

			Vector3 target = Zone.position;

			target.z = 0;

			player.transform.position = target;

			player.Position(target);
		}

		public void SetMonsterArea(Monster monster, AreaLocal areaLocal) {
			Debug.LogFormat("<color=teal>[MapManager]</color> SetMonsterArea {0}({1}) {2}", monster.Data().Name,
				monster.Id(), areaLocal.Name());

			Main.Game.AvatarController.Monsters.Add(new AreaAvatar {
				entity = monster,
				areaLocal = areaLocal
			});

			//if (monster.Area() != null)
			//    monster.Area().OnPlayerScaleUpdate.RemoveListener(monster.Scale);

			//area.OnPlayerScaleUpdate.AddListener(monster.Scale);
			//monster.Scale(0.5f);

			//monster.transform.SetParent(area.Players());
			monster.Area(areaLocal);

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


			Vector3 pos = new Vector3 {x = (float) monster.Data().x, y = (float) monster.Data().y, z = 0};

			monster.transform.position = pos;

			monster.Position(pos);

			Debug.LogFormat("<color=green>[MonsterMain]</color> Vector3({0}, {1}, {2})", monster.transform.position.x,
				monster.transform.position.y, monster.transform.position.z);
		}

		public Player PlayerByID(int playerID) {
			return (Player) Main.Game.AvatarController.Players.FirstOrDefault(map => map.entity.Id() == playerID)
				?.entity;
		}

		public Monster MonsterByID(int monsterID) {
			return (Monster) Main.Game.AvatarController.Monsters.FirstOrDefault(map => map.entity.Id() == monsterID)
				?.entity;
		}

		public HashSet<AvatarMain> FindAvatars(string name) {
			List<AreaAvatar> areaAvatars = new List<AreaAvatar>()
				.Concat(Main.Game.AvatarController.Players)
				.Concat(Main.Game.AvatarController.Monsters)
				//TODO: NPC
				//TODO: PET
				.ToList();

			return new HashSet<AvatarMain>(areaAvatars
				.Select(avatar => avatar.entity)
				.Where(mapEntity => name == mapEntity.Area().Name()).ToList());
		}

		public void RemovePlayerFromArea(int playerID) {
			Main.Game.AvatarController.Players.RemoveAll(pm => pm.entity.Id() == playerID);
		}

	}
}