using System;
using System.Collections.Generic;
using System.Linq;
using Asgla.Avatar;
using Asgla.Avatar.Monster;
using Asgla.Avatar.Player;
using Asgla.Data.Avatar.Helper;
using Asgla.Data.Skill;
using Asgla.Skill;
using Asgla.Utility;
using UnityEngine;

namespace Asgla.UI.Action_Bar {
	public class ActionBar : MonoBehaviour {

		//[SerializeField] private ExperienceBar _experience;

		[SerializeField] private List<SkillMain> _slots;

		//public ExperienceBar GetExperienceBar() => _experience;

		public SkillMain GetSkillBySlotID(int slotID) {
			return _slots.Where(s => s.ID == slotID).FirstOrDefault();
		}

		public void SkillAssign(List<SkillData> skills) {
			foreach (SkillData data in skills) {
				SkillMain skill = GetSkillBySlotID(data.SlotID);

				if (skill == null) {
					Debug.Log("<color=orange>[ActionBar] </color> SkillMain null");
					return;
				}

				skill.Assign(data);

				skill.onClick.AddListener(OnSkillClick);
			}
		}

		public void OnSkillClick(SkillMain slot) {
			Player player = Main.Singleton.AvatarManager.Player;

			SkillData skillInfo = slot.GetSkillData();

			if (player.Target() == null) {
				if (player.Targets().Count == 0 && skillInfo.Target != SkillTarget.SELF) {
					//TODO: Chat
					Main.Singleton.Game.Chat.ReceiveChatMessage(1,
						string.Format("<size=22>{1}</size> <b><color=#{0}>{3}</color></b> <color=#{2}>{4}</color>",
							CommonColorBuffer.ColorToString(Color.red), DateTime.Now.ToShortTimeString(),
							CommonColorBuffer.ColorToString(Color.white), "game", "select target"));
					return;
				}

				if (skillInfo.Target == SkillTarget.SELF)
					player.Target(player);
			}

			if (player.Target().Type() == EntityType.PLAYER && player.Area().IsSafe() &&
			    skillInfo.Target != SkillTarget.SELF) {
				//TODO: Chat
				Main.Singleton.Game.Chat.ReceiveChatMessage(1,
					string.Format("<size=22>{1}</size> <b><color=#{0}>{3}</color></b> <color=#{2}>{4}</color>",
						CommonColorBuffer.ColorToString(Color.green), DateTime.Now.ToShortTimeString(),
						CommonColorBuffer.ColorToString(Color.white), "game", "safe area, please no"));
				return;
			}

			if (Main.Singleton.Game.CastBar.IsCasting) {
				//TODO: Chat
				Main.Singleton.Game.Chat.ReceiveChatMessage(1,
					string.Format("<size=22>{1}</size> <b><color=#{0}>{3}</color></b> <color=#{2}>{4}</color>",
						"ffa500", DateTime.Now.ToShortTimeString(), CommonColorBuffer.ColorToString(Color.white),
						"game", "casting.. wait"));
				return;
			}

			if (player.Target() != null) {
				if (!player.IsNear(player.Target(), skillInfo.Range)) {
					//TODO: Chat
					Main.Singleton.Game.Chat.ReceiveChatMessage(1,
						string.Format("<size=22>{1}</size> <b><color=#{0}>{3}</color></b> <color=#{2}>{4}</color>",
							"ffa500", DateTime.Now.ToShortTimeString(), CommonColorBuffer.ColorToString(Color.white),
							"game", "target distance not in range"));
					return;
				}
			} else {
				Debug.LogFormat("Player target is null/empty selected new from targets list {0} - {1}", player.Id(),
					player.Data().username);
				player.Target(GameUtil.FindTarget(1, player.Area().Scale(), skillInfo.Range, player,
					Main.Singleton.MapManager.FindAvatars(player.Area().Name()), new HashSet<AvatarMain>()).First());
			}

			if (slot.cooldownComponent.IsOnCooldown)
				return;

			if (skillInfo.Cooldown > 0f)
				slot.cooldownComponent.StartCooldown(skillInfo.SlotID, skillInfo.Cooldown);

			List<string> targets = player.Targets().Select(e => $"{e.Id()}:{(int) e.Type()}").ToList();

			string target = "";

			switch (player.Target()) {
				case Player p:
					target = $"{p.Id()}:{(int) p.Type()}";
					break;
				case Monster m:
					target = $"{m.Id()}:{(int) m.Type()}";
					break;
			}

			//Main.Singleton.Request.Send("Combat", slot.ID, target, string.Join(",", targets));

			switch (skillInfo.CastType) {
				case SkillCastType.CAST:
					Main.Singleton.Game.CastBar.StartCasting(skillInfo, skillInfo.CastTime,
						Time.time + skillInfo.CastTime, target, string.Join(",", targets));
					break;
				default:
					Main.Singleton.Request.Send("Combat", slot.ID, target, string.Join(",", targets));
					break;
			}
		}

	}
}