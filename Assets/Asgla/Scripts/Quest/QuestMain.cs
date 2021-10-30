using System.Collections.Generic;
using System.Linq;
using Asgla.Data.Avatar.Player;
using Asgla.Data.Quest;
using Asgla.Scenes;
using Asgla.UI.Quest.Track;

namespace Asgla.Quest {
	public class QuestMain {

		private readonly List<QuestData> _completed = new List<QuestData>();

		private readonly List<QuestData> _progress = new List<QuestData>();

		private readonly Game Game;

		public QuestMain(Game game) {
			Game = game;
		}

		public bool InProgress(QuestData quest) {
			return _progress.Where(q => q.DatabaseID == quest.DatabaseID).FirstOrDefault() != null;
		}

		public void AddProgress(QuestData quest) {
			_progress.Add(quest);

			if (Game.QuestTrack.Get(quest.DatabaseID) == null)
				Game.QuestTrack.Add(quest);
		}

		/**/
		public void RemoveProgress(QuestData quest) {
			_progress.Remove(quest);

			if (Game.QuestTrack.Get(quest.DatabaseID) != null)
				Game.QuestTrack.Remove(quest.DatabaseID);
		}

		public void CompleteProgress(QuestData quest) {
			_progress.Remove(quest);
			_completed.Add(quest);
		}

		public void Turn(QuestData quest) {
			_completed.Remove(quest);
			RemoveProgress(quest);
		}

		public bool Check(QuestData quest) {
			if (quest.Requirement.Count == 0)
				return true;

			return CheckReq(quest);
		}

		public bool CheckAll() {
			if (_progress.Count == 0)
				return false;

			foreach (QuestData quest in _progress) {
				if (quest.Requirement.Count == 0)
					return true;

				if (!CheckReq(quest))
					return false;
			}

			return true;
		}

		private bool CheckReq(QuestData quest) {
			foreach ((Requirement requirement, PlayerInventory inventory) in
				from Requirement requirement in quest.Requirement
				let inventory = Main.Singleton.AvatarManager.Player.Data()
					.InventoryByItemId(requirement.Item.databaseId)
				select (requirement, inventory)) {
				if (inventory == null)
					return false;

				QuestTrackProgress progress = Game.QuestTrack.Get(quest.DatabaseID);
				if (progress != null) {
					QuestTrackObjective objective = progress.Get(requirement.DatabaseID);
					if (objective != null) {
						objective.UpdateProgress(inventory.quantity);

						if (inventory.quantity < requirement.Quantity)
							return false;
					}
				}
			}

			return true;
		}

	}
}