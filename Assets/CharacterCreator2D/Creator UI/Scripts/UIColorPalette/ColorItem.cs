using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CharacterCreator2D.UI {
	[RequireComponent(typeof(Image))]
	public class ColorItem : MonoBehaviour, IPointerClickHandler {

		/// <summary>
		/// Color value loaded by this ColorItem.
		/// </summary>
		[ReadOnly] public Color color;

		/// <summary>
		/// Is ColorItem editable?
		/// </summary>
		[ReadOnly] public bool editable;

		public void OnPointerClick(PointerEventData eventData) {
			UIColorPalette uipalette = this.transform.GetComponentInParent<UIColorPalette>();
			if (uipalette == null)
				return;

			if (eventData.button == PointerEventData.InputButton.Right) {
				if (editable)
					uipalette.RemoveFromCustomPalette(this.color);
			} else
				uipalette.color = this.color;
		}

		/// <summary>
		/// Initialize ColorItem with a given Color to load.
		/// </summary>
		/// <param name="color">Color to be loaded by this ColorItem</param>
		public void Initialize(Color color) {
			this.color = color;
			this.GetComponent<Image>().color = color;
		}

	}
}