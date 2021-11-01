using System;
using System.Collections.Generic;
using Asgla.Avatar;
using Asgla.Avatar.Player;
using Asgla.Data.Request;
using Asgla.Skill;
using Asgla.Utility;
using AsglaUI.UI;
using BestHTTP.JSON.LitJson;
using UnityEngine;

namespace Asgla.Requests.Unity {
	public class AvatarCombat : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public RequestAvatar.CombatAnimation Animation = null;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public RequestAvatar.Entity Entity = null;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string Message = null;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public List<RequestAvatar.CombatResult> Result = null;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public RequestAvatar.CombatSkill Skill = null;

		public void onRequest(Main main, string json) {
			AvatarCombat avatarCombat = JsonMapper.ToObject<AvatarCombat>(json);

			SkillMain skill = main.Game.ActionBar.GetSkillBySlotID(avatarCombat.Skill.SlotID);

			//reset skill cooldown
			if (avatarCombat.Message != null) {
				main.Game.Chat.ReceiveChatMessage(
					1,
					$"<size=22>{DateTime.Now.ToShortTimeString()}</size> <b><color=#{CommonColorBuffer.ColorToString(Color.red)}>{"game"}</color></b> <color=#{CommonColorBuffer.ColorToString(Color.white)}>{avatarCombat.Message}</color>"
				);

				UISlotCooldown.CooldownInfo cooldownInfo = new UISlotCooldown.CooldownInfo(0f, Time.time, Time.time);

				// Save that this spell is on cooldown
				if (!skill.cooldownComponent.Cooldowns().ContainsKey(avatarCombat.Skill.SlotID))
					skill.cooldownComponent.Cooldowns().Add(avatarCombat.Skill.SlotID, cooldownInfo);

				// Start the coroutine
				skill.cooldownComponent.StartCooldownCoroutine(cooldownInfo);
				return;
			}

			AvatarMain from = avatarCombat.Entity.Avatar; //TODO: Find Monster

			if (from is null)
				return;

			//Debug.LogFormat("AvatarCombat PlayerMain {0}", c.Skill.SlotID);

			if (from is Player p)
				if (p.Data().isControlling) {
					//SkillData skillInfo = skill.GetSkillData();

					//if (skillInfo.CastType == SkillCastType.CAST)
					//    Main.Game.CastBar.StartCasting(skillInfo, skillInfo.CastTime, Time.time + skillInfo.CastTime);

					UISlotCooldown.CooldownInfo cooldownInfo = new UISlotCooldown.CooldownInfo(
						avatarCombat.Skill.Cooldown, Time.time,
						Time.time + avatarCombat.Skill.Cooldown);

					// Save that this spell is on cooldown
					if (!skill.cooldownComponent.Cooldowns().ContainsKey(avatarCombat.Skill.SlotID))
						skill.cooldownComponent.Cooldowns().Add(avatarCombat.Skill.SlotID, cooldownInfo);

					// Start the coroutine
					skill.cooldownComponent.StartCooldownCoroutine(cooldownInfo);
				}

			//TODO: Check if target/select exist in current player if not add.

			foreach (RequestAvatar.CombatResult result in avatarCombat.Result) {
				AvatarMain target = result.Entity.Avatar;

				if (target is null)
					continue;

				main.Game.AvatarController.Combat(from, target, result, avatarCombat.Animation);
			}
		}

	}
}