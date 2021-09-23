using Asgla.Utility;
using TMPro;
using UnityEngine;

namespace Asgla.UI.Quest {
    public enum QuestOARColor : int {
        Requirement,
        Gold,
        Experience,
    }

    public class QuestObjectiveAndReward : MonoBehaviour {

        public const string Requirement = "9d9d9dff";
        public const string Gold = "ffffffff";
        public const string Experience = "1eff00ff";

        public static string GetHexColor(QuestOARColor r) {
            switch (r) {
                case QuestOARColor.Requirement: return Requirement;
                case QuestOARColor.Experience: return Experience;
                case QuestOARColor.Gold: return Gold;
                default: return Requirement;
            }
        }

        [SerializeField] private TextMeshProUGUI _amount = null;

        [SerializeField] private TextMeshProUGUI _objective = null;

        public void Init(string n, string amout, string objective, string color) {
            name = n.ToString();

            _amount.text = amout;
            _objective.text = objective;

            _objective.color = CommonColorBuffer.StringToColor(color);
        }

    }
}
