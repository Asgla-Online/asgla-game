using Asgla.Data.Item;
using Asgla.Data.Player;
using Asgla.Data.Quest;
using Asgla.UI.Item;
using Asgla.UI.Quest;
using AsglaUI.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.Window {

    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(CanvasGroup))]
    public class QuestWindow : UIWindow {

        [SerializeField] private Transform _info = null;

        [Header("Button")]
        [SerializeField] private Button _button = null;
        [SerializeField] private TextMeshProUGUI _buttonText = null;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _title = null;
        [SerializeField] private TextMeshProUGUI _description = null;

        [Header("Slot")]
        [SerializeField] private QuestSlot _questSlot = null;
        [SerializeField] private ToggleGroup _slotGroup = null;

        [Header("Objective")]
        [SerializeField] private QuestObjectiveAndReward _objectiveSlot = null;
        [SerializeField] private Transform _objectiveContent = null;

        [Header("Reward")]
        [SerializeField] private QuestObjectiveAndReward _rewardSlot = null;
        [SerializeField] private ItemSlot _itemSlot = null;
        [SerializeField] private Transform _rewardAmountContent = null;
        [SerializeField] private Transform _rewardContent = null;

        #region Unity
        protected override void Awake() {
            base.Awake();

            _info.gameObject.SetActive(false);
        }
        #endregion

        public void Init(List<QuestData> quests) {
            Clear();
            Clear2();
            foreach (QuestData quest in quests)
                Add(quest);
        }

        private void Add(QuestData quest) {
            QuestSlot q = Instantiate(_questSlot.gameObject, _slotGroup.transform).GetComponent<QuestSlot>();
            q.Init(quest);
            q.Toggle().onValueChanged.AddListener(delegate {
                Select(q);
            });
            q.Toggle().group = _slotGroup;
        }

        public void Select(QuestSlot slot) {
            _info.gameObject.SetActive(false);

            Clear2();

            QuestData quest = slot.Quest();

            _title.text = quest.Name;
            _description.text = quest.Description;

            if (quest.Requirement.Count != 0)
                foreach (Requirement requirement in quest.Requirement) {
                    PlayerInventory inv = Main.Singleton.AvatarManager.Player.Data().InventoryByItemId(requirement.Item.databaseId);

                    int quantity = inv == null ? 0 : inv.quantity;

                    AddRequirementSlot(requirement.DatabaseID, $"{quantity}/{requirement.Quantity}", requirement.Item.name);
                }

            AddRewardAmountSlot(quest.Experience.ToString(), "Experience", "B4FF64");

            AddRewardAmountSlot(quest.Gold.ToString(), "Gold", "FFD964");

            foreach (Reward reward in quest.Reward)
                AddRewardSlot(reward.DatabaseID, reward.Item);

            _button.onClick.RemoveAllListeners();

            if (Main.Singleton.Game.Quest.InProgress(quest)) {
                if (Main.Singleton.Game.Quest.Check(quest)) {
                    _button.onClick.AddListener(delegate { Turn(quest); });
                    _buttonText.text = "Turn";
                } else {
                    _button.onClick.AddListener(delegate { Abandon(quest); });
                    _buttonText.text = "Abandon";
                }
            } else {
                _button.onClick.AddListener(delegate { Accept(quest); });
                _buttonText.text = "Accept";
            }

            _info.gameObject.SetActive(true);
        }

        private void Accept(QuestData q) {
            Main.Singleton.Game.Quest.AddProgress(q);
            Main.Singleton.Request.Send("QuestAccept", q.DatabaseID);

            if (Main.Singleton.Game.Quest.Check(q)) {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(delegate { Turn(q); });
                _buttonText.text = "Turn";
            }
        }

        private void Abandon(QuestData q) {
            Main.Singleton.Request.Send("QuestAbandon", q.DatabaseID);
        }

        private void Turn(QuestData q) {
            Main.Singleton.Request.Send("QuestTurn", q.DatabaseID);
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(delegate { Accept(q); });
            _buttonText.text = "Accept";
            Main.Singleton.Game.Quest.Turn(q);
        }

        private void AddRequirementSlot(int databaseId, string amount, string objective) {
            Instantiate(_objectiveSlot.gameObject, _objectiveContent)
                .GetComponent<QuestObjectiveAndReward>()
                    .Init(databaseId.ToString(), amount, objective, "FFFFFF");
        }

        private void AddRewardAmountSlot(string amount, string name, string color) {
            Instantiate(_objectiveSlot.gameObject, _rewardAmountContent)
                .GetComponent<QuestObjectiveAndReward>()
                    .Init(name, amount, name, color);
        }

        private void AddRewardSlot(int databaseId, ItemData item) {
            Instantiate(_itemSlot.gameObject, _rewardContent)
                .GetComponent<ItemSlot>()
                    .Init(databaseId, ItemListType.Quest, item);
        }

        private void Clear() => Main.Singleton.UIManager.ClearChild(_slotGroup.transform);

        private void Clear2() => Main.Singleton.UIManager.ClearChild(_objectiveContent, _rewardAmountContent, _rewardContent);

    }

}
