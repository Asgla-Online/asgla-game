using System.Collections.Generic;
using Asgla.Controller;
using Asgla.Data.Avatar.Player;
using Asgla.Data.Item;
using Asgla.Data.Quest;
using Asgla.UI.Item;
using Asgla.UI.Quest;
using AsglaUI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.Window {

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class QuestWindow : UIWindow {

		[SerializeField] private Transform _info;

		[Header("Button")] [SerializeField] private Button _button;

		[SerializeField] private TextMeshProUGUI _buttonText;

		[Header("Text")] [SerializeField] private TextMeshProUGUI _title;

		[SerializeField] private TextMeshProUGUI _description;

		[Header("Slot")] [SerializeField] private QuestSlot _questSlot;

		[SerializeField] private ToggleGroup _slotGroup;

		[Header("Objective")] [SerializeField] private QuestObjectiveAndReward _objectiveSlot;

		[SerializeField] private Transform _objectiveContent;

		[Header("Reward")] [SerializeField] private QuestObjectiveAndReward _rewardSlot;

		[SerializeField] private ItemSlot _itemSlot;
		[SerializeField] private Transform _rewardAmountContent;
		[SerializeField] private Transform _rewardContent;

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
			q.Toggle().onValueChanged.AddListener(delegate { Select(q); });
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
					PlayerInventory inv = Main.Singleton.AvatarManager.Player.Data()
						.InventoryByItemId(requirement.Item.databaseId);

					int quantity = inv == null ? 0 : inv.quantity;

					AddRequirementSlot(requirement.DatabaseID, $"{quantity}/{requirement.Quantity}",
						requirement.Item.name);
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

		private void Clear() {
			UIController.ClearChild(_slotGroup.transform);
		}

		private void Clear2() {
			UIController.ClearChild(_objectiveContent, _rewardAmountContent, _rewardContent);
		}

	}

}