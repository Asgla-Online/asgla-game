using Asgla.Data.Quest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Quest {
    public class QuestSlot : MonoBehaviour {

        private QuestData _quest = null;

        [SerializeField] private Toggle _toggle = null;

        [SerializeField] private TextMeshProUGUI _name = null;

        #region Unity
        private void OnDisable() {
            _toggle.onValueChanged.RemoveAllListeners();
        }
        #endregion

        public QuestData Quest() {
            return _quest;
        }

        public void Init(QuestData quest) {
            name = quest.DatabaseID.ToString();

            _quest = quest;

            _name.text = _quest.Name;
        }

        public Toggle Toggle() => _toggle;

    }
}
