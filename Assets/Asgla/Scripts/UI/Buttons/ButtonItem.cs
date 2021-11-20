using System;
using Asgla.Data.Item;
using Asgla.Data.Type;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Buttons {
	public class ButtonItem : MonoBehaviour {

		[SerializeField] private Image itemIcon;
		[SerializeField] private Image itemGlowRarity;

		[SerializeField] private TextMeshProUGUI itemName;
		[SerializeField] private TextMeshProUGUI itemRarity;

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

		public ButtonItem Init(int databaseId, ItemListType type, ItemData item) {
			name = databaseId.ToString();

			_databaseId = databaseId;
			_type = type;
			_item = item;

			Sprite sprite = _item.GetIcon;

			if (sprite != null)
				itemIcon.sprite = sprite;

			Color rarityColor = RarityColor.GetColor(item.rarity);

			itemGlowRarity.color = rarityColor;

			itemName.text = _item.name;

			itemRarity.text = _item.rarity.ToString();
			itemRarity.color = rarityColor;

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