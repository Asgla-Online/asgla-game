using System.Collections.Generic;
using System.Linq;
using Asgla.Data.Item;
using Asgla.UI.Item;
using AsglaUI.UI;
using CharacterCreator2D;
using UnityEngine;

namespace Asgla.Window {
	public abstract class ItemListMain : UIWindow {

		[SerializeField] private ItemListType _type = ItemListType.Equip;

		[SerializeField] protected Transform _all;
		[SerializeField] protected Transform _class;
		[SerializeField] protected Transform _armor;
		[SerializeField] protected Transform _weapon;
		[SerializeField] protected Transform _helm;

		[SerializeField] private ItemSlot _slot;

		[SerializeField] private List<ItemSlot> _slots;

		public void AddItem(int databaseId, ItemData item) {
			AddSlot(databaseId, item, _all);
			switch (item.Type.Category) {
				case PartCategory.Armor:
					AddSlot(databaseId, item, _armor);
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
					AddSlot(databaseId, item, _helm);
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
					AddSlot(databaseId, item, _weapon);
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
			ItemSlot slot = _slots.Where(s => s.Id() == databaseId).FirstOrDefault();
			if (slot != null)
				Destroy(slot.gameObject);
		}

		private void AddSlot(int databaseId, ItemData item, Transform content) {
			_slots.Add(Instantiate(_slot.gameObject, content)
				.GetComponent<ItemSlot>()
				.Init(databaseId, _type, item));
		}

		protected void Order() {
			int i = 0;
			foreach (ItemSlot slot in from s in _slots orderby s.Item().Type.Category select s) {
				//Debug.LogFormat("{0} {1}", slot.Item().Name, i);
				slot.transform.SetSiblingIndex(i);
				i++;
			}
		}

		protected void Clear() {
			Main.Singleton.UIManager.ClearChild(_all.transform, _armor.transform, _weapon.transform, _helm.transform);
		}

	}
}