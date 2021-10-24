using Asgla.Data.Avatar.Helper;
using Asgla.UI;
using Asgla.UI.Item;
using AsglaUI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.Window {

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class ItemPreviewWindow : UIWindow {

		[SerializeField] private CharacterPreview _characterPreviewPrefab;

		[SerializeField] private RawImage _image;

		[SerializeField] private TextMeshProUGUI _name;

		[SerializeField] private Button _button;

		[SerializeField] private TextMeshProUGUI _buttonText;

		private CharacterPreview _characterPreview;

		private ItemSlot _slot;

		public void Init(ItemSlot slot) {
			_slot = slot;

			_name.text = _slot.Item().name;

			if (_slot.ButtonText() == "Quest") {
				_button.gameObject.SetActive(false);
			} else {
				_buttonText.text = _slot.ButtonText();
				_button.gameObject.SetActive(true);
			}

			if (_characterPreview == null)
				_characterPreview = Instantiate(_characterPreviewPrefab);

			_image.enabled = false;

			_characterPreview.SetImage(_image);

			_characterPreview.Equip(new EquipPart {
				type = _slot.Item().Type,
				bundle = _slot.Item().bundle,
				asset = _slot.Item().asset
			});

			Window().Show();
		}

		public void CloseEvent() {
			Destroy(_characterPreview.gameObject);
		}

		private void Click() {
			Main.Singleton.Request.Send(_slot.Send(), _slot.Id());
		}

		#region Unity

		protected override void OnEnable() {
			base.OnEnable();

			_button.onClick.AddListener(Click);
		}

		protected override void OnDisable() {
			base.OnDisable();

			_button.onClick.RemoveListener(Click);
		}

		#endregion

	}

}