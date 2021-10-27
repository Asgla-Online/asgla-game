using System;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[Serializable]
	public enum UITooltipTextEffectType {

		Shadow,
		Outline

	}

	[Serializable]
	public enum OverrideTextAlignment {

		No,
		Left,
		Center,
		Right

	}

	[Serializable]
	public class UITooltipTextEffect {

		public UITooltipTextEffectType Effect;
		public Color EffectColor;
		public Vector2 EffectDistance;
		public bool UseGraphicAlpha;

		public UITooltipTextEffect() {
			Effect = UITooltipTextEffectType.Shadow;
			EffectColor = new Color(0f, 0f, 0f, 128f);
			EffectDistance = new Vector2(1f, -1f);
			UseGraphicAlpha = true;
		}

	}

	[Serializable]
	public class UITooltipLineStyle : IComparable<UITooltipLineStyle> {

		public string Name;
		public Font TextFont;
		public FontStyle TextFontStyle;
		public int TextFontSize;
		public float TextFontLineSpacing;
		public OverrideTextAlignment OverrideTextAlignment;
		public Color TextFontColor;
		public UITooltipTextEffect[] TextEffects;

		public bool DisplayName = true;

		public UITooltipLineStyle() {
			Defaults();
		}

		public UITooltipLineStyle(bool displayName) {
			Defaults();
			DisplayName = displayName;
		}

		public int CompareTo(UITooltipLineStyle other) {
			return Name.CompareTo(other.Name);
		}

		private void Defaults() {
			Name = "";
			TextFont = FontData.defaultFontData.font;
			TextFontStyle = FontData.defaultFontData.fontStyle;
			TextFontSize = FontData.defaultFontData.fontSize;
			TextFontLineSpacing = FontData.defaultFontData.lineSpacing;
			OverrideTextAlignment = OverrideTextAlignment.No;
			TextFontColor = Color.white;
			TextEffects = new UITooltipTextEffect[0];
		}

	}
}