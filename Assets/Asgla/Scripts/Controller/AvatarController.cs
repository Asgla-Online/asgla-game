using System.Collections.Generic;
using System.Linq;
using Asgla.Avatar;
using Asgla.Avatar.Monster;
using Asgla.Avatar.Player;
using Asgla.Data.Area;
using Asgla.Data.Avatar.Helper;
using Asgla.Data.Avatar.Player;
using Asgla.Data.Skill;
using Asgla.Data.Type;
using Asgla.Map;
using CharacterCreator2D;
using UnityEngine;
using static Asgla.Data.Request.RequestAvatar;
using Random = System.Random;

namespace Asgla.Controller {

	public class AvatarController : Controller {

		public Main Main = null;
		public List<AreaAvatar> Monsters = null;

		public Player Player;

		public List<AreaAvatar> Players = null;

		public void SelectTarget(AvatarMain target) {
			Player.Target(target);

			Debug.LogFormat("Select {0}", target.Id());

			target.Utility().SmallUnitFrameOne.gameObject.SetActive(true);

			target.Utility().SmallUnitFrameOne.SetLevel(target.Level());

			target.Utility().SmallUnitFrameOne.Health.SetValueMax(target.Stats().HealthMax);
			target.Utility().SmallUnitFrameOne.Health.SetValue(target.Stats().Health);

			target.Utility().SmallUnitFrameOne.Energy.SetValueMax(target.Stats().EnergyMax);
			target.Utility().SmallUnitFrameOne.Energy.SetValue(target.Stats().Energy);
		}

		public void Combat(AvatarMain from, AvatarMain target, CombatResult result, CombatAnimation animation) {
			if (animation.EffectData != null) {
				if (from.Animator().runtimeAnimatorController != null) {
					string[] animAr = animation.Animation.Split(',');

					int toSkip = new Random().Next(0, animAr.Count());

					from.Animator().Play(animAr.Skip(toSkip).Take(1).First(), 0);
				}

				switch (animation.Target) {
					case SkillTarget.SELF:
						//from.Attack(from, result.Damage);
						from.Damaged(result.Damage, result.Type);
						//targets.Add(from);
						Main.StartCoroutine(Main.GameAsset.SpawmAsset(animation.EffectData.Prefab, from, from));
						break;
					case SkillTarget.TARGET:
					case SkillTarget.ALLIES:
						//from.Attack(target, result.Damage);
						target.Damaged(result.Damage, result.Type);
						//targets.Add(target);
						Main.StartCoroutine(Main.GameAsset.SpawmAsset(animation.EffectData.Prefab, from, target));
						break;
				}
			} else {
				Debug.Log("effectData null");
			}
		}

		#region Create

		public void Create(PlayerData playerData, MapMain map) {
			MapArea area = map.AreaByName(playerData.area.area);

			Debug.LogFormat("A {0}", Main.PlayerPrefab.gameObject);

			GameObject clone = Object.Instantiate(Main.PlayerPrefab.gameObject, area.Players());

			Debug.LogFormat("B {0}", clone);

			Player player = clone.GetComponent<Player>();

			player.Data(playerData);

			player.Avatar().name = playerData.playerID.ToString();

			player.Utility().SetTitle("Explosion");
			player.Utility().SetName(player.Data().username);
			player.Utility().SetGuild("Asgla Team");

			player.UpdateDataBody(playerData);

			player.Equip2(new EquipPart {
				asset = "armor/armor1/armor1.asset",

				bundle = "items/armor/armor1",

				//equipment = Equipment.Armor,

				playerId = -1,

				type = new TypeItemData {
					Category = PartCategory.Armor,
					Equipment = SlotCategory.Armor,

					Icon = "armor",

					Name = "armor 1",

					//Weapon = null,
				},

				uniqueId = 1
			});

			player.Equip2(new EquipPart {
				asset = "weapon/one handed/alice/alice.asset",

				bundle = "items/weapon/one handed/alice",

				//equipment = Equipment.MainHand,

				playerId = -1,

				type = new TypeItemData {
					Category = PartCategory.Weapon,
					Equipment = SlotCategory.MainHand,

					Icon = "weapon",

					Name = "weapon 1",

					//Weapon = Weapon.,
				},

				uniqueId = 1
			});

			//player.CharacterView().EquipPart(SlotCategory.Armor , Main.Singleton.part1);
			//player.CharacterView().EquipPart(SlotCategory.MainHand , Main.Singleton.part2);

			if (playerData.isControlling) {
				Player = player;

				Main.Singleton.Game.CinemachineVirtual.Follow = player.Avatar().transform;

				Object.Destroy(Main.Singleton.Game.Camera.GetComponent<AudioListener>());
				player.Avatar().AddComponent<AudioListener>();
			}

			Main.MapManager.UpdatePlayerArea(player, area, player.Data().area.point);

			if (playerData.x == 0 || playerData.y == 0)
				return;

			Vector2 pos = new Vector2 {x = (float) playerData.x, y = (float) playerData.y};

			player.transform.position = pos;
			player.Position(pos);

			player.transform.localPosition = Vector3.zero;
		}

		public void Create(AreaLocalMonster data) {
			//Debug.LogFormat("<color=purple>[PlayerManager]</color> CreatePlayer {0} {1}", data.PlayerID, data.Username);

			GameObject clone = Object.Instantiate(Main.MonsterPrefab.gameObject, data.Area.Monsters());

			Monster monster = clone.GetComponent<Monster>();

			if (monster == null)
				return;

			monster.gameObject.SetActive(false);

			monster.Data(data.monster);
			monster.Stats(data.stats);

			monster.Avatar().name = monster.Data().UniqueID.ToString();

			monster.Utility().SetName(monster.Data().Name);

			Main.StartCoroutine(monster.AsynchronousLoad());

			Main.MapManager.SetMonsterArea(monster, data.Area); //TODO: set unique id
		}

		#endregion

	}
}