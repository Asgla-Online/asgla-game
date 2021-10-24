using System.Collections.Generic;
using System.Linq;
using Asgla.Data.Quest;
using TMPro;
using UnityEngine;

namespace Asgla.UI.Quest.Track {
	public class QuestTrackProgress : MonoBehaviour {

		[SerializeField] private TextMeshProUGUI _text;

		[SerializeField] private Transform _content;

		private readonly List<QuestTrackObjective> _objectives = new List<QuestTrackObjective>();

		private QuestData _quest;

		public QuestTrackProgress Init(QuestData quest, QuestTrackObjective objective) {
			_quest = quest;

			_text.text = quest.Name;

			name = quest.DatabaseID.ToString();

			_objectives.AddRange(quest.Requirement.Select(requirement => Instantiate(objective.gameObject, _content)
				.GetComponent<QuestTrackObjective>()
				.Init(requirement.DatabaseID, requirement.Item.name, requirement.Quantity)));

			return this;
		}

		public QuestData Quest() {
			return _quest;
		}

		public QuestTrackObjective Get(int databaseId) {
			return _objectives.Where(objective => objective.gameObject.name == databaseId.ToString()).FirstOrDefault();
		}

	}
}