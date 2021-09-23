using Asgla.UI;
using Asgla.UI.Item;
using AsglaUI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static Asgla.Request.RequestAvatar;

namespace Asgla.Window {

    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(CanvasGroup))]
    public class ItemPreviewWindow : UIWindow {

        private ItemSlot _slot = null;

        private CharacterPreview _characterPreview = null;

        [SerializeField] private CharacterPreview _characterPreviewPrefab = null;

        [SerializeField] private RawImage _image = null;

        [SerializeField] private TextMeshProUGUI _name = null;

        [SerializeField] private Button _button = null;

        [SerializeField] private TextMeshProUGUI _buttonText = null;

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

        public void Init(ItemSlot slot) {
            _slot = slot;

            _name.text = _slot.Item().Name;

            if (_slot.ButtonText() == "Quest")
                _button.gameObject.SetActive(false);
            else {
                _buttonText.text = _slot.ButtonText();
                _button.gameObject.SetActive(true);
            }

            if (_characterPreview == null)
                _characterPreview = Instantiate(_characterPreviewPrefab);

            _image.enabled = false;

            _characterPreview.SetImage(_image);

            _characterPreview.Equip(new EquipPart {
                Type = _slot.Item().Type,
                Bundle = _slot.Item().Bundle,
                Asset = _slot.Item().Asset
            });

            Window().Show();
        }

        public void CloseEvent() => Destroy(_characterPreview.gameObject);

        private void Click() {
            Main.Singleton.Request.Send(_slot.Send(), _slot.Id());
        }

    }

}
