using System.Collections.Generic;
using System.Linq;
using Asgla.Controller;
using Asgla.Data.Item;
using Asgla.UI.Item;
using AsglaUI.UI;
using CharacterCreator2D;
using UnityEngine;

namespace Asgla.UI.Window {
	public abstract class ItemListMain : UIWindow {

		[SerializeField] private ItemListType type = ItemListType.Equip;

		[SerializeField] protected Transform all;

		//[SerializeField] protected Transform @class;
		[SerializeField] protected Transform armor;
		[SerializeField] protected Transform weapon;
		[SerializeField] protected Transform helm;

		[SerializeField] private ItemRow row;

		[SerializeField] private List<ItemRow> slots;

		public void AddItem(int databaseId, ItemData item) {
			AddSlot(databaseId, item, all);
			switch (item.Type.Category) {
				case PartCategory.Armor:
					AddSlot(databaseId, item, armor);
					break;
				case PartCategory.Boots:
					break;
				case PartCategory.Ear:
					break;
				case PartCategory.Eyebrow:
					break;
				case PartCategory.Eyes:
					break;
				case PartCategory.FacialHair:
					break;
				case PartCategory.Gloves:
					break;
				case PartCategory.Hair:
					break;
				case PartCategory.Helmet:
					AddSlot(databaseId, item, helm);
					break;
				case PartCategory.Mouth:
					break;
				case PartCategory.Nose:
					break;
				case PartCategory.Pants:
					break;
				case PartCategory.SkinDetails:
					break;
				case PartCategory.Weapon:
					AddSlot(databaseId, item, weapon);
					break;
				case PartCategory.Cape:
					break;
				case PartCategory.Skirt:
					break;
				case PartCategory.BodySkin:
					break;
				//case PartCategory.Class:
				//	AddSlot(databaseId, item, _class);
				//	break;
			}
		}

		public void RemoveItem(int databaseId) {
			ItemRow itemRow = slots.FirstOrDefault(s => s.Id() == databaseId);
			if (itemRow != null)
				Destroy(itemRow.gameObject);
		}

		private void AddSlot(int databaseId, ItemData item, Transform content) {
			slots.Add(Instantiate(row.gameObject, content)
				.GetComponent<ItemRow>()
				.Init(databaseId, type, item));
		}

		protected void Order() {
			int i = 0;
			foreach (ItemRow itemSlot in from s in slots orderby s.Item().Type.Category select s) {
				//Debug.LogFormat("{0} {1}", slot.Item().Name, i);
				itemSlot.transform.SetSiblingIndex(i);
				i++;
			}
		}

		protected void Clear() {
			UIController.ClearChild(all.transform, armor.transform, weapon.transform, helm.transform);
		}

	}
}