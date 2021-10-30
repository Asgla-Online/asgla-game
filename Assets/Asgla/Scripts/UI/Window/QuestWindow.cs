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

namespace Asgla.UI.Window {

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class QuestWindow : UIWindow {

		[SerializeField] private Transform info;

		[Header("Button")] [SerializeField] private Button button;

		[SerializeField] private TextMeshProUGUI buttonText;

		[Header("Text")] [SerializeField] private TextMeshProUGUI title;

		[SerializeField] private TextMeshProUGUI description;

		[Header("Slot")] [SerializeField] private QuestSlot questSlot;

		[SerializeField] private ToggleGroup slotGroup;

		[Header("Objective")] [SerializeField] private QuestObjectiveAndReward objectiveSlot;

		[SerializeField] private Transform objectiveContent;

		[Header("Reward")] [SerializeField] private QuestObjectiveAndReward rewardSlot;

		[SerializeField] private ItemRow itemRow;
		[SerializeField] private Transform rewardAmountContent;
		[SerializeField] private Transform rewardContent;

		#region Unity

		protected override void Awake() {
			base.Awake();

			info.gameObject.SetActive(false);
		}

		#endregion

		public void Init(List<QuestData> quests) {
			Clear();
			Clear2();
			foreach (QuestData quest in quests)
				Add(quest);
		}

		private void Add(QuestData quest) {
			QuestSlot q = Instantiate(questSlot.gameObject, slotGroup.transform).GetComponent<QuestSlot>();
			q.Init(quest);
			q.Toggle().onValueChanged.AddListener(delegate { Select(q); });
			q.Toggle().group = slotGroup;
		}

		public void Select(QuestSlot slot) {
			info.gameObject.SetActive(false);

			Clear2();

			QuestData quest = slot.Quest();

			title.text = quest.Name;
			description.text = quest.Description;

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

			button.onClick.RemoveAllListeners();

			if (Main.Singleton.Game.Quest.InProgress(quest)) {
				if (Main.Singleton.Game.Quest.Check(quest)) {
					button.onClick.AddListener(delegate { Turn(quest); });
					buttonText.text = "Turn";
				} else {
					button.onClick.AddListener(delegate { Abandon(quest); });
					buttonText.text = "Abandon";
				}
			} else {
				button.onClick.AddListener(delegate { Accept(quest); });
				buttonText.text = "Accept";
			}

			info.gameObject.SetActive(true);
		}

		private void Accept(QuestData q) {
			Main.Singleton.Game.Quest.AddProgress(q);
			Main.Singleton.Request.Send("QuestAccept", q.DatabaseID);

			if (Main.Singleton.Game.Quest.Check(q)) {
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(delegate { Turn(q); });
				buttonText.text = "Turn";
			}
		}

		private void Abandon(QuestData q) {
			Main.Singleton.Request.Send("QuestAbandon", q.DatabaseID);
		}

		private void Turn(QuestData q) {
			Main.Singleton.Request.Send("QuestTurn", q.DatabaseID);
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate { Accept(q); });
			buttonText.text = "Accept";
			Main.Singleton.Game.Quest.Turn(q);
		}

		private void AddRequirementSlot(int databaseId, string amount, string objective) {
			Instantiate(objectiveSlot.gameObject, objectiveContent)
				.GetComponent<QuestObjectiveAndReward>()
				.Init(databaseId.ToString(), amount, objective, "FFFFFF");
		}

		private void AddRewardAmountSlot(string amount, string name, string color) {
			Instantiate(objectiveSlot.gameObject, rewardAmountContent)
				.GetComponent<QuestObjectiveAndReward>()
				.Init(name, amount, name, color);
		}

		private void AddRewardSlot(int databaseId, ItemData item) {
			Instantiate(itemRow.gameObject, rewardContent)
				.GetComponent<ItemRow>()
				.Init(databaseId, ItemListType.Quest, item);
		}

		private void Clear() {
			UIController.ClearChild(slotGroup.transform);
		}

		private void Clear2() {
			UIController.ClearChild(objectiveContent, rewardAmountContent, rewardContent);
		}

	}

}