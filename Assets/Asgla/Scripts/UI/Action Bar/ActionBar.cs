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
			Player player = Main.Singleton.Game.AvatarController.Player;

			SkillData skillInfo = slot.GetSkillData();

			if (player.Target() == null) {
				if (player.Targets().Count == 0 && skillInfo.Target != SkillTarget.SELF) {
					Main.Singleton.Game.Chat.ChatMessage("Warning", "select target");
					return;
				}

				if (skillInfo.Target == SkillTarget.SELF)
					player.Target(player);
			}

			if (player.Target().Type() == EntityType.Player && player.Area().IsSafe() &&
			    skillInfo.Target != SkillTarget.SELF) {
				Main.Singleton.Game.Chat.ChatMessage("Warning", "safe area, please no");
				return;
			}

			if (Main.Singleton.Game.CastBar.IsCasting) {
				Main.Singleton.Game.Chat.ChatMessage("Warning", "casting.. wait");
				return;
			}

			if (player.Target() != null) {
				if (!player.IsNear(player.Target(), skillInfo.Range)) {
					Main.Singleton.Game.Chat.ChatMessage("Warning", "target distance not in range");
					return;
				}
			} else {
				Debug.LogFormat("Player target is null/empty selected new from targets list {0} - {1}", player.Id(),
					player.Data().username);
				player.Target(GameUtil.FindTarget(1, player.Area().Scale(), skillInfo.Range, player,
						Main.Singleton.Game.AreaController.FindAvatars(player.Area().Name()), new HashSet<AvatarMain>())
					.First());
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