using System;
using Asgla.Data.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Item {
	public class ItemRow : MonoBehaviour {

		[SerializeField] private Image icon;

		[SerializeField] private Image background;

		[SerializeField] private TextMeshProUGUI itemName;

		private string _buttonText;

		private int _databaseId;

		private ItemData _item;

		private string _send;

		private ItemListType _type;

		public int Id() {
			return _databaseId;
		}

		public ItemData Item() {
			return _item;
		}

		public ItemListType Type() {
			return _type;
		}

		public string Send() {
			return _send;
		}

		public string ButtonText() {
			return _buttonText;
		}

		public ItemRow Init(int databaseId, ItemListType type, ItemData item) {
			name = databaseId.ToString();

			_databaseId = databaseId;
			_item = item;
			_type = type;

			itemName.text = _item.name;

			Sprite sprite = _item.GetIcon;

			if (sprite == null)
				Debug.Log("Icon null {0}");
			else
				icon.sprite = _item.GetIcon;

			switch (_type) {
				case ItemListType.Equip:
					_send = "EquipItem";
					_buttonText = "Equip";
					break;
				case ItemListType.Buy:
					_send = "ShopBuy";
					_buttonText = "Buy";
					break;
				case ItemListType.Sell:
					_send = "ShopSellItem";
					_buttonText = "Sell";
					break;
				case ItemListType.Quest:
					_send = "";
					_buttonText = "Quest";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return this;
		}

		public void Click() {
			Main.Singleton.Game.WindowItemPreview.Init(this);
		}

	}
}