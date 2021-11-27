using Asgla.Data.Avatar.Helper;
using Asgla.UI.Buttons;
using AsglaUI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Window {

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class ItemPreviewWindow : UIWindow {

		[SerializeField] private CharacterPreview characterPreviewPrefab;

		[SerializeField] private RawImage itemPreview;

		[SerializeField] private TextMeshProUGUI itemName;

		[SerializeField] private Button button;

		[SerializeField] private TextMeshProUGUI buttonText;

		private CharacterPreview _characterPreview;

		private ButtonItem _row;

		public void Init(ButtonItem row) {
			_row = row;

			//Color rarityColor = RarityColor.GetColor(_row.Item().rarity);

			itemName.text = _row.Item().name;

			if (_row.ButtonText() == "Quest") {
				button.gameObject.SetActive(false);
			} else {
				buttonText.text = _row.ButtonText();
				button.gameObject.SetActive(true);
			}

			if (_characterPreview == null)
				_characterPreview = Instantiate(characterPreviewPrefab);

			itemPreview.enabled = false;

			_characterPreview.SetImage(itemPreview);

			_characterPreview.Equip(new EquipPart {
				type = _row.Item().Type,
				bundle = _row.Item().bundle,
				asset = _row.Item().asset
			});

			Window().Show();
		}

		public void CloseEvent() {
			Destroy(_characterPreview.gameObject);
		}

		private void Click() {
			Main.Singleton.Request.Send(_row.Send(), _row.Id());
		}

		#region Unity

		protected override void OnEnable() {
			base.OnEnable();

			button.onClick.AddListener(Click);
		}

		protected override void OnDisable() {
			base.OnDisable();

			button.onClick.RemoveListener(Click);
		}

		#endregion

	}

}