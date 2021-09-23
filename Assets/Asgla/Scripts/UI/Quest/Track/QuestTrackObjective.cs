using Asgla.Quest;
using TMPro;
using UnityEngine;

namespace Asgla.UI.Quest.Track {
    public class QuestTrackObjective : MonoBehaviour {

        private TextMeshProUGUI _text = null;

        private int _databaseId = -1;

        private string _name = null;

        private int _quantity = -1;

        [SerializeField] private GameObject _completed = null;

        [SerializeField] private GameObject _failed = null;

        #region Unity
        private void Awake() {
            _text = GetComponent<TextMeshProUGUI>();
        }
        #endregion

        public QuestTrackObjective Init(int databaseId, string n, int quantity) {
            _databaseId = databaseId;

            _name = n;

            _quantity = quantity;

            name = databaseId.ToString();

            _text.text = $"0/{quantity} {n}";

            Default();

            return this;
        }

        public void UpdateProgress(int quantity) {
            _text.text = $"{quantity}/{_quantity} {_name}";
            if (quantity >= _quantity)
                Completed();
        }

        private void Default() {
            _failed.SetActive(false);
            _completed.SetActive(false);
        }

        private void Completed() {
            _completed.SetActive(true);
            _failed.SetActive(false);
        }

        private void Failed() {
            _failed.SetActive(true);
            _completed.SetActive(false);
        }

    }
}