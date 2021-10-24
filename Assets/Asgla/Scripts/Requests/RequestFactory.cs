using System;
using System.Collections.Generic;

namespace Asgla.Requests {

	public static class RequestFactory {

		private static readonly Dictionary<int, string> Requests = new Dictionary<int, string> {
			{0, "Default"},
			{1, "Login"},
			{2, "PlayerUpdate"},
			{3, "Move"},
			{4, "JoinMap"},
			{5, "LeaveMap"},
			{6, "Chat"},
			{7, "EquipPart"},
			{8, "Notification"},
			{9, "Pong"},
			{10, "Experience"},
			{11, "MoveToLocal"},
			{12, "PlayerDataLoad"},
			{13, "PlayerInventoryLoad"},
			{14, "AvatarDataUpdate"},
			{15, "AvatarCombat"},
			{16, "PlayerRespawn"},
			{17, "SkillLoad"},
			{18, "ShopLoad"},
			{19, "QuestLoad"},
			{20, "PlayerInventoryUpdate"},
			{21, "PlayerInventoryRemove"}
		};

		public static IRequest Create(int command) {
			Type objectType = Type.GetType("Asgla.Requests.Unity." + Requests[command]) ??
			                  Type.GetType("Asgla.Requests.Unity.Default");

			return Activator.CreateInstance(objectType!) as IRequest;
		}

	}

}