using System.Collections.Generic;
using System.Linq;
using Asgla.Controller;
using Asgla.Data.Item;
using Asgla.UI.Buttons;
using AsglaUI.UI;
using CharacterCreator2D;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Window {
	public abstract class ItemListWindow : UIWindow {

		[SerializeField] private ItemListType type = ItemListType.Equip;

		[Header("Groups")] [SerializeField] private ToggleGroup tabsGroup;

		[SerializeField] private Transform scrollsGroup;

		[Header("Prefabs")] [SerializeField] private UITab buttonTabPrefab;

		[SerializeField] private ButtonItem buttonItemPrefab;
		[SerializeField] private ScrollRect scrollRectPrefab;

		private readonly List<ButtonItem> _buttonItems = new List<ButtonItem>();

		protected List<Tab> Tabs;

		protected override void Start() {
			base.Start();

			bool first = true;

			UITab fUITab = null;

			UIController.ClearChild(scrollsGroup, tabsGroup.transform);

			foreach (Tab tab in Tabs) {
				ScrollRect scrollRect = Instantiate(scrollRectPrefab.gameObject, scrollsGroup.transform)
					.GetComponent<ScrollRect>();

				scrollRect.name = tab.Name;

				tab.UITab = Instantiate(buttonTabPrefab.gameObject, tabsGroup.transform).GetComponent<UITab>();

				tab.UITab.name = tab.Name;

				tab.UITab.textTarget.text = tab.Name;

				tab.UITab.targetContent = scrollRect.gameObject;
				tab.UITab.group = tabsGroup;

				tab.UITab.isOn = first;

				tab.Content = scrollRect.content;

				if (first)
					fUITab = tab.UITab;

				first = false;
			}

			if (fUITab != null)
				fUITab.isOn = true;
		}

		public void AddItem(int databaseId, ItemData item) {
			Tabs
				.FindAll(tab => tab.Category.Contains(item.Type.Category))
				.ForEach(tab => {
					_buttonItems.Add(Instantiate(buttonItemPrefab.gameObject, tab.Content).GetComponent<ButtonItem>()
						.Init(databaseId, type, item));
				});
		}

		public void RemoveItem(int databaseId) {
			_buttonItems.FindAll(item => item.Id() == databaseId).ForEach(item => { Destroy(item.gameObject); });

			_buttonItems.RemoveAll(item => item.Id() == databaseId);
		}

		protected void Order() {
			int i = 0;
			foreach (ButtonItem buttonItem in _buttonItems.OrderBy(item => item.Item().Type.Category)) {
				buttonItem.transform.SetSiblingIndex(i);
				i++;
			}
		}

		protected void Clear() {
			Tabs.ForEach(tab => UIController.ClearChild(tab.Content));
		}

		protected class Tab {

			public List<PartCategory> Category;
			public Transform Content;

			public string Name;

			public UITab UITab;

		}

	}
}