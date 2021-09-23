using Asgla.Data.Quest;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Asgla.UI.Quest.Track {
    public class QuestTrackProgress : MonoBehaviour {

        private QuestData _quest;

        [SerializeField] private TextMeshProUGUI _text = null;

        [SerializeField] private Transform _content;

        private readonly List<QuestTrackObjective> _objectives = new List<QuestTrackObjective>();

        public QuestTrackProgress Init(QuestData quest, QuestTrackObjective objective) {
            _quest = quest;

            _text.text = quest.Name;

            name = quest.DatabaseID.ToString();

            _objectives.AddRange(quest.Requirement.Select(requirement => Instantiate(objective.gameObject, _content)
                .GetComponent<QuestTrackObjective>().Init(requirement.DatabaseID, requirement.Item.Name, requirement.Quantity)));
            
            return this;
        }

        public QuestData Quest() => _quest;

        public QuestTrackObjective Get(int databaseId) => _objectives.Where(objective => objective.gameObject.name == databaseId.ToString()).FirstOrDefault();

    }
}